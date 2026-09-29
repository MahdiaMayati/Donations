using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Address.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Addresses.Queries.GetAddressById;

public sealed class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, AddressResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAddressByIdQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AddressResponse?> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var address = await _context.Addresses
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (address is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin && address.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only view your own addresses.");
        }

        return address;
    }
}
