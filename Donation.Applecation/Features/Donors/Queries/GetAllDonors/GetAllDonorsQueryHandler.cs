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

        var query = _context.Donors.AsNoTracking();

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(d => d.UserId == _currentUser.UserId.Value);
        }

        return await query
            .OrderByDescending(d => d.Id)
            .Select(d => new DonorResponse
            {
                Id = d.Id,
                UserId = d.UserId
            })
            .ToListAsync(cancellationToken);
    }
}
