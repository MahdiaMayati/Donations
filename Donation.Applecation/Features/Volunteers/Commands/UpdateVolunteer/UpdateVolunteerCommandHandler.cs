using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Volunteer.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Volunteers.Commands.UpdateVolunteer;

public sealed class UpdateVolunteerCommandHandler : IRequestHandler<UpdateVolunteerCommand, VolunteerResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateVolunteerCommandHandler> _logger;

    public UpdateVolunteerCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateVolunteerCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<VolunteerResponse?> Handle(UpdateVolunteerCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var volunteer = await _context.Volunteers
            .Include(v => v.User)
            .Include(v => v.Organization)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (volunteer is null)
        {
            return null;
        }

        EnsureCanManage(volunteer);

        if (request.OrganizationId.HasValue)
        {
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == request.OrganizationId.Value && !o.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Organization not found.");

            volunteer.OrganizationId = organization.Id;
            volunteer.Organization = organization;
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            volunteer.Status = VolunteerMapping.AllowedStatuses
                .First(s => s.Equals(request.Status.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (request.Days is not null)
        {
            volunteer.Days = request.Days.Trim();
        }

        if (request.HoursCount.HasValue)
        {
            volunteer.HoursCount = request.HoursCount.Value;
        }

        if (request.Hobbies is not null)
        {
            volunteer.Hobbies = string.IsNullOrWhiteSpace(request.Hobbies) ? null : request.Hobbies.Trim();
        }

        if (request.Skills is not null)
        {
            volunteer.Skills = string.IsNullOrWhiteSpace(request.Skills) ? null : request.Skills.Trim();
        }

        Address? address = await _context.Addresses
            .Where(a => a.UserId == volunteer.UserId)
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
                address = new Address { UserId = volunteer.UserId };
                _context.Addresses.Add(address);
            }

            address.AreaId = request.Address.AreaId;
            address.Street = request.Address.Street.Trim();
            address.Details = request.Address.Details.Trim();
            address.Latitude = request.Address.Latitude;
            address.Longitude = request.Address.Longitude;
        }

        await _context.SaveChangesAsync(cancellationToken);

        Address? addressWithLocation = null;
        if (address is not null)
        {
            addressWithLocation = await _context.Addresses
                .AsNoTracking()
                .Include(a => a.Area)
                    .ThenInclude(ar => ar.City)
                .FirstOrDefaultAsync(a => a.Id == address.Id, cancellationToken);
        }
        else
        {
            addressWithLocation = await _context.Addresses
                .AsNoTracking()
                .Include(a => a.Area)
                    .ThenInclude(ar => ar.City)
                .Where(a => a.UserId == volunteer.UserId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        _logger.LogInformation("Volunteer updated with Id {VolunteerId}", volunteer.Id);

        return VolunteerMapping.ToResponse(volunteer, volunteer.User, volunteer.Organization, addressWithLocation);
    }

    private void EnsureCanManage(Volunteer volunteer)
    {
        if (_currentUser.IsAdmin)
        {
            return;
        }

        if (volunteer.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only manage your own volunteer profile.");
        }
    }
}
