using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Donors.Commands.CreateDonor;

public sealed class CreateDonorCommandHandler : IRequestHandler<CreateDonorCommand, DonorResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserIdentityService _userIdentity;
    private readonly ILogger<CreateDonorCommandHandler> _logger;

    public CreateDonorCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        IUserIdentityService userIdentity,
        ILogger<CreateDonorCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _userIdentity = userIdentity;
        _logger = logger;
    }

    public async Task<DonorResponse> Handle(CreateDonorCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var userId = _currentUser.UserId.Value;

        var exists = await _context.Donors
            .AnyAsync(d => d.UserId == userId, cancellationToken);

        if (exists)
        {
            throw new ConflictException("A donor profile already exists for this user.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var areaExists = await _context.Areas
            .AnyAsync(a => a.Id == request.Address.AreaId, cancellationToken);

        if (!areaExists)
        {
            throw new NotFoundException("Area not found.");
        }

        await _userIdentity.EnsureEmailAvailableAsync(request.Email, userId, cancellationToken);

        var (firstName, lastName) = DonorMapping.SplitFullName(request.FullName);
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = request.Email.Trim();
        user.UserName = request.Email.Trim();
        user.PhoneNumber = request.PhoneNumber.Trim();
        user.PreferredContactMethod = request.PreferredContactMethod.Trim();

        var address = new Address
        {
            AreaId = request.Address.AreaId,
            UserId = userId,
            Street = request.Address.Street.Trim(),
            Details = request.Address.Details.Trim(),
            Latitude = request.Address.Latitude,
            Longitude = request.Address.Longitude
        };
        _context.Addresses.Add(address);

        var donor = new Donor
        {
            UserId = userId,
            IsDeleted = false,
            DeletedAt = null
        };
        _context.Donors.Add(donor);

        await _context.SaveChangesAsync(cancellationToken);
        await _userIdentity.ChangePasswordAsync(userId, request.Password, cancellationToken);

        // Refresh user so PasswordHash presence is accurate for masked response.
        user = await _context.Users.AsNoTracking()
            .FirstAsync(u => u.Id == userId, cancellationToken);

        _logger.LogInformation("Donor created with Id {DonorId} for User {UserId}", donor.Id, userId);

        return DonorMapping.ToResponse(donor, user, address);
    }
}
