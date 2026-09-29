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
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (volunteer is null)
        {
            return null;
        }

        // Owner may view but cannot change Status; only Admin/SuperAdmin can update.
        if (!_currentUser.IsAdmin)
        {
            if (volunteer.UserId != _currentUser.UserId)
            {
                throw new ForbiddenException("You can only manage your own volunteer profile.");
            }

            throw new ForbiddenException("Only administrators can update volunteer status.");
        }

        volunteer.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Volunteer updated with Id {VolunteerId}", volunteer.Id);

        return new VolunteerResponse
        {
            Id = volunteer.Id,
            UserId = volunteer.UserId,
            Status = volunteer.Status
        };
    }
}
