using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Address.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Addresses.Queries.GetAllAddresses;

public sealed class GetAllAddressesQueryHandler : IRequestHandler<GetAllAddressesQuery, IReadOnlyList<AddressResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllAddressesQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<AddressResponse>> Handle(GetAllAddressesQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var query = _context.Addresses.AsNoTracking();

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(a => a.UserId == _currentUser.UserId.Value);
        }

        if (request.AreaId.HasValue)
        {
            query = query.Where(a => a.AreaId == request.AreaId.Value);
        }

        return await query
            .OrderByDescending(a => a.Id)
            .Select(a => new AddressResponse
            {
                Id = a.Id,
                AreaId = a.AreaId,
                UserId = a.UserId,
                Street = a.Street,
                Details = a.Details,
                Latitude = a.Latitude,
                Longitude = a.Longitude
            })
            .ToListAsync(cancellationToken);
    }
}
