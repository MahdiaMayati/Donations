using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Volunteer.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Volunteers.Commands.RestoreVolunteer;

public sealed class RestoreVolunteerCommandHandler : IRequestHandler<RestoreVolunteerCommand, VolunteerResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<RestoreVolunteerCommandHandler> _logger;

    public RestoreVolunteerCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<RestoreVolunteerCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<VolunteerResponse?> Handle(RestoreVolunteerCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException("Only Admin users can restore soft-deleted volunteers.");
        }

        var volunteer = await _context.Volunteers
            .IgnoreQueryFilters()
            .Include(v => v.User)
            .Include(v => v.Organization)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (volunteer is null || !volunteer.IsDeleted)
        {
            return null;
        }

        var activeExists = await _context.Volunteers
            .AnyAsync(v => v.UserId == volunteer.UserId && !v.IsDeleted, cancellationToken);

        if (activeExists)
        {
            throw new ConflictException(
                "Cannot restore this volunteer because an active volunteer profile already exists for the same user.");
        }

        volunteer.IsDeleted = false;
        volunteer.DeletedAt = null;

        await _context.SaveChangesAsync(cancellationToken);

        var address = await _context.Addresses
            .AsNoTracking()
            .Include(a => a.Area)
                .ThenInclude(ar => ar.City)
            .Where(a => a.UserId == volunteer.UserId)
            .OrderByDescending(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);

        _logger.LogInformation("Volunteer restored with Id {VolunteerId}", volunteer.Id);

        return VolunteerMapping.ToResponse(volunteer, volunteer.User, volunteer.Organization, address);
    }
}
