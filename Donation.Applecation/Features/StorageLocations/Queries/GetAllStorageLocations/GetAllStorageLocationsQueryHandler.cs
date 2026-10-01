using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.StorageLocation.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.StorageLocations.Queries.GetAllStorageLocations;

public sealed class GetAllStorageLocationsQueryHandler
    : IRequestHandler<GetAllStorageLocationsQuery, PaginatedResult<StorageLocationResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllStorageLocationsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<StorageLocationResponse>> Handle(
        GetAllStorageLocationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.StorageLocations.AsNoTracking();

        // Optional: omit / null / empty Guid → all storage locations (pagination still applies).
        if (request.WarehouseId is { } warehouseId && warehouseId != Guid.Empty)
        {
            query = query.Where(s => s.WarehouseId == warehouseId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(s => s.Code.ToLower().Contains(term));
        }

        return await query
            .OrderBy(s => s.Code)
            .Select(s => new StorageLocationResponse
            {
                Id = s.Id,
                WarehouseId = s.WarehouseId,
                Code = s.Code
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
