using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Donors.Queries.GetDeletedDonors;

public sealed class GetDeletedDonorsQueryHandler : IRequestHandler<GetDeletedDonorsQuery, IReadOnlyList<DonorResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDeletedDonorsQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<DonorResponse>> Handle(
        GetDeletedDonorsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException("Only Admin users can view soft-deleted donors.");
        }

        var donors = await _context.Donors
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(d => d.User)
            .Where(d => d.IsDeleted)
            .OrderByDescending(d => d.DeletedAt)
            .ThenByDescending(d => d.Id)
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

        return donors
            .Select(d => DonorMapping.ToResponse(
                d,
                d.User,
                addressByUser.GetValueOrDefault(d.UserId)))
            .ToList();
    }
}
