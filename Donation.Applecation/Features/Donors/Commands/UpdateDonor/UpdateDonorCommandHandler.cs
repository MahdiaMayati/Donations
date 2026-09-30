using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Donors.Commands.UpdateDonor;

public sealed class UpdateDonorCommandHandler : IRequestHandler<UpdateDonorCommand, DonorResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserIdentityService _userIdentity;
    private readonly ILogger<UpdateDonorCommandHandler> _logger;

    public UpdateDonorCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        IUserIdentityService userIdentity,
        ILogger<UpdateDonorCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _userIdentity = userIdentity;
        _logger = logger;
    }

    public async Task<DonorResponse?> Handle(UpdateDonorCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var donor = await _context.Donors
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (donor is null)
        {
            return null;
        }

        EnsureCanManage(donor);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == donor.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var (firstName, lastName) = DonorMapping.SplitFullName(request.FullName);
            user.FirstName = firstName;
            user.LastName = lastName;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            await _userIdentity.EnsureEmailAvailableAsync(request.Email, user.Id, cancellationToken);
            user.Email = request.Email.Trim();
            user.UserName = request.Email.Trim();
        }

        if (request.PhoneNumber is not null)
        {
            user.PhoneNumber = request.PhoneNumber.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.PreferredContactMethod))
        {
            user.PreferredContactMethod = request.PreferredContactMethod.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            await _userIdentity.ChangePasswordAsync(user.Id, request.Password, cancellationToken);
        }

        Address? address = await _context.Addresses
            .Where(a => a.UserId == donor.UserId)
            .OrderByDescending(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (request.Address is not null)
        {
            var areaExists = await _context.Areas
                .AnyAsync(a => a.Id == request.Address.AreaId, cancellationToken);

            if (!areaExists)
            {
                throw new NotFoundException("Area not found.");
            }

            if (address is null)
            {
                address = new Address { UserId = donor.UserId };
                _context.Addresses.Add(address);
            }

            address.AreaId = request.Address.AreaId;
            address.Street = request.Address.Street.Trim();
            address.Details = request.Address.Details.Trim();
            address.Latitude = request.Address.Latitude;
            address.Longitude = request.Address.Longitude;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Donor updated with Id {DonorId}", donor.Id);

        user = await _context.Users.AsNoTracking()
            .FirstAsync(u => u.Id == donor.UserId, cancellationToken);

        Address? addressWithLocation = null;
        if (address is not null)
        {
            addressWithLocation = await _context.Addresses
                .AsNoTracking()
                .Include(a => a.Area)
                    .ThenInclude(ar => ar.City)
                .FirstOrDefaultAsync(a => a.Id == address.Id, cancellationToken);
        }

        return DonorMapping.ToResponse(donor, user, addressWithLocation);
    }

    private void EnsureCanManage(Donor donor)
    {
        if (_currentUser.IsAdmin)
        {
            return;
        }

        if (donor.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only manage your own donor profile.");
        }
    }
}
