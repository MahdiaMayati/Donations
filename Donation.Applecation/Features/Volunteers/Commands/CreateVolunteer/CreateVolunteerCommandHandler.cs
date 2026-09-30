using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Volunteer.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Volunteers.Commands.CreateVolunteer;

public sealed class CreateVolunteerCommandHandler : IRequestHandler<CreateVolunteerCommand, VolunteerResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateVolunteerCommandHandler> _logger;

    public CreateVolunteerCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateVolunteerCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<VolunteerResponse> Handle(CreateVolunteerCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var userId = _currentUser.UserId.Value;

        var exists = await _context.Volunteers
            .AnyAsync(v => v.UserId == userId, cancellationToken);

        if (exists)
        {
            throw new ConflictException("A volunteer profile already exists for this user.");
        }

        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == request.OrganizationId && !o.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Organization not found.");

        var areaExists = await _context.Areas
            .AnyAsync(a => a.Id == request.Address.AreaId, cancellationToken);

        if (!areaExists)
        {
            throw new NotFoundException("Area not found.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

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

        var status = VolunteerMapping.NormalizeStatus(request.Status);
        // Preserve casing from allowed list.
        status = VolunteerMapping.AllowedStatuses
            .First(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));

        var volunteer = new Volunteer
        {
            UserId = userId,
            OrganizationId = organization.Id,
            Status = status,
            Days = request.Days.Trim(),
            HoursCount = request.HoursCount,
            Hobbies = string.IsNullOrWhiteSpace(request.Hobbies) ? null : request.Hobbies.Trim(),
            Skills = string.IsNullOrWhiteSpace(request.Skills) ? null : request.Skills.Trim(),
            IsDeleted = false,
            DeletedAt = null
        };

        _context.Volunteers.Add(volunteer);
        await _context.SaveChangesAsync(cancellationToken);

        var addressWithLocation = await _context.Addresses
            .AsNoTracking()
            .Include(a => a.Area)
                .ThenInclude(ar => ar.City)
            .FirstAsync(a => a.Id == address.Id, cancellationToken);

        _logger.LogInformation("Volunteer created with Id {VolunteerId}", volunteer.Id);

        return VolunteerMapping.ToResponse(volunteer, user, organization, addressWithLocation);
    }
}
