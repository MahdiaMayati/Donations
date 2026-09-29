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
            .Where(v => v.Id == request.Id)
            .Select(v => new VolunteerResponse
            {
                Id = v.Id,
                UserId = v.UserId,
                Status = v.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (volunteer is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin && volunteer.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only view your own volunteer profile.");
        }

        return volunteer;
    }
}
