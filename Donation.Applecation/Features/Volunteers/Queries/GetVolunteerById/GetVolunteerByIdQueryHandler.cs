using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Volunteer.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Volunteers.Queries.GetVolunteerById;

public sealed class GetVolunteerByIdQueryHandler : IRequestHandler<GetVolunteerByIdQuery, VolunteerResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVolunteerByIdQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VolunteerResponse?> Handle(GetVolunteerByIdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var volunteer = await _context.Volunteers
            .AsNoTracking()
            .Include(v => v.User)
            .Include(v => v.Organization)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (volunteer is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin && volunteer.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only view your own volunteer profile.");
        }

        var address = await _context.Addresses
            .AsNoTracking()
            .Include(a => a.Area)
                .ThenInclude(ar => ar.City)
            .Where(a => a.UserId == volunteer.UserId)
            .OrderByDescending(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return VolunteerMapping.ToResponse(volunteer, volunteer.User, volunteer.Organization, address);
    }
}
