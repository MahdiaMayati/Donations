using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Volunteer.Response;
using Donation.Domain.Entities;
using Donation.Domain.Enums;
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

        var volunteer = new Volunteer
        {
            UserId = userId,
            Status = VolunteerStatus.Pending
        };

        _context.Volunteers.Add(volunteer);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Volunteer created with Id {VolunteerId}", volunteer.Id);

        return new VolunteerResponse
        {
            Id = volunteer.Id,
            UserId = volunteer.UserId,
            Status = volunteer.Status
        };
    }
}
