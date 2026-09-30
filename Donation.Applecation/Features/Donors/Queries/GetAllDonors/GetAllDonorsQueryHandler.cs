using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Donors.Queries.GetAllDonors;

public sealed class GetAllDonorsQueryHandler : IRequestHandler<GetAllDonorsQuery, IReadOnlyList<DonorResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllDonorsQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<DonorResponse>> Handle(GetAllDonorsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var query = _context.Donors
            .AsNoTracking()
            .Include(d => d.User)
            .AsQueryable();

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(d => d.UserId == _currentUser.UserId.Value);
        }

        var donors = await query
            .OrderByDescending(d => d.Id)
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
