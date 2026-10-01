using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Warehouse.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Warehouses.Queries.GetAllWarehouses;

public sealed class GetAllWarehousesQueryHandler
    : IRequestHandler<GetAllWarehousesQuery, PaginatedResult<WarehouseResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllWarehousesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<WarehouseResponse>> Handle(
        GetAllWarehousesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Warehouses
            .AsNoTracking()
            .Where(w => !w.IsDeleted);

        // Optional: omit / null / empty Guid → all warehouses (soft-delete + pagination still apply).
        if (request.OrganizationId is { } organizationId && organizationId != Guid.Empty)
        {
            query = query.Where(w => w.OrganizationId == organizationId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(w => w.Name.ToLower().Contains(term));
        }

        return await query
            .OrderBy(w => w.Name)
            .Select(w => new WarehouseResponse
            {
                Id = w.Id,
                OrganizationId = w.OrganizationId,
                AddressId = w.AddressId,
                Name = w.Name,
                IsActive = w.IsActive,
                IsDeleted = w.IsDeleted
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
