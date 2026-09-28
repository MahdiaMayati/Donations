using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Volunteers.Commands.DeleteVolunteer;

public sealed class DeleteVolunteerCommandHandler : IRequestHandler<DeleteVolunteerCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteVolunteerCommandHandler> _logger;

    public DeleteVolunteerCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<DeleteVolunteerCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteVolunteerCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var volunteer = await _context.Volunteers
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (volunteer is null)
        {
            return false;
        }

        EnsureCanManage(volunteer);

        _context.Volunteers.Remove(volunteer);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Volunteer deleted with Id {VolunteerId}", request.Id);

        return true;
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
