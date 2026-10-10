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

        var query = _context.Volunteers
            .AsNoTracking()
            .Include(v => v.User)
            .Include(v => v.Organization)
            .AsQueryable();

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(v => v.UserId == _currentUser.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(v =>
                (v.User.FirstName + " " + v.User.LastName).ToLower().Contains(term)
                || (v.User.Email != null && v.User.Email.ToLower().Contains(term))
                || v.Status.ToLower().Contains(term)
                || (v.Skills != null && v.Skills.ToLower().Contains(term)));
        }

        var page = request.Page < 1 ? PaginationRequest.DefaultPage : request.Page;
        var limit = request.Limit < 1
            ? PaginationRequest.DefaultLimit
            : Math.Min(request.Limit, PaginationRequest.MaxLimit);

        var totalItems = await query.CountAsync(cancellationToken);

        var volunteers = await query
            .OrderByDescending(v => v.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var userIds = volunteers.Select(v => v.UserId).Distinct().ToList();

        var addresses = await _context.Addresses
            .AsNoTracking()
            .Include(a => a.Area)
                .ThenInclude(ar => ar.City)
            .Where(a => userIds.Contains(a.UserId))
            .ToListAsync(cancellationToken);

        var addressByUser = addresses
            .GroupBy(a => a.UserId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.Id).First());

        var items = volunteers
            .Select(v => VolunteerMapping.ToResponse(
                v,
                v.User,
                v.Organization,
                addressByUser.GetValueOrDefault(v.UserId)))
            .ToList();

        return PaginatedResult<VolunteerResponse>.Create(items, page, limit, totalItems);
    }
}
