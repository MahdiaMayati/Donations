using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Volunteer.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Volunteers.Queries.GetAllVolunteers;

public sealed class GetAllVolunteersQueryHandler
    : IRequestHandler<GetAllVolunteersQuery, PaginatedResult<VolunteerResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllVolunteersQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedResult<VolunteerResponse>> Handle(
        GetAllVolunteersQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var query = _context.Volunteers.AsNoTracking();

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(v => v.UserId == _currentUser.UserId.Value);
        }

        return await query
            .OrderByDescending(v => v.Id)
            .Select(v => new VolunteerResponse
            {
                Id = v.Id,
                UserId = v.UserId,
                Status = v.Status
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
