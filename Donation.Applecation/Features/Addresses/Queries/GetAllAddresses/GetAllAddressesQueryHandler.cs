using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Address.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Addresses.Queries.GetAllAddresses;

public sealed class GetAllAddressesQueryHandler : IRequestHandler<GetAllAddressesQuery, PaginatedResult<AddressResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllAddressesQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedResult<AddressResponse>> Handle(GetAllAddressesQuery request, CancellationToken cancellationToken)
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

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(a =>
                a.Street.ToLower().Contains(term) ||
                a.Details.ToLower().Contains(term));
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
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
