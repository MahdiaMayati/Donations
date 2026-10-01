using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Donor.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Donors.Queries.GetDeletedDonors;

public sealed class GetDeletedDonorsQueryHandler : IRequestHandler<GetDeletedDonorsQuery, PaginatedResult<DonorResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDeletedDonorsQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedResult<DonorResponse>> Handle(
        GetDeletedDonorsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException("Only Admin users can view soft-deleted donors.");
        }

        var query = _context.Donors
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(d => d.User)
            .Where(d => d.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(d =>
                (d.User.Email != null && d.User.Email.ToLower().Contains(term)) ||
                d.User.FirstName.ToLower().Contains(term) ||
                d.User.LastName.ToLower().Contains(term));
        }

        query = query
            .OrderByDescending(d => d.DeletedAt)
            .ThenByDescending(d => d.Id);

        var page = request.Page < 1 ? PaginationRequest.DefaultPage : request.Page;
        var limit = request.Limit < 1
            ? PaginationRequest.DefaultLimit
            : Math.Min(request.Limit, PaginationRequest.MaxLimit);

        var totalItems = await query.CountAsync(cancellationToken);

        var donors = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var userIds = donors.Select(d => d.UserId).Distinct().ToList();

        var addresses = await _context.Addresses
            .AsNoTracking()
            .Include(a => a.Area)
                .ThenInclude(ar => ar.City)
            .Where(a => userIds.Contains(a.UserId))
            .ToListAsync(cancellationToken);

        var addressByUser = addresses
            .GroupBy(a => a.UserId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.Id).First());

        var items = donors
            .Select(d => DonorMapping.ToResponse(
                d,
                d.User,
                addressByUser.GetValueOrDefault(d.UserId)))
            .ToList();

        return PaginatedResult<DonorResponse>.Create(items, page, limit, totalItems);
    }
}
