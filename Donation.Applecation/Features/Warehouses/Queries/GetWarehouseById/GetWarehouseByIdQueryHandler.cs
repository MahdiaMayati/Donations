using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.Warehouse.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Warehouses.Queries.GetWarehouseById;

public sealed class GetWarehouseByIdQueryHandler
    : IRequestHandler<GetWarehouseByIdQuery, WarehouseResponse?>
{
    private readonly IAppDbContext _context;

    public GetWarehouseByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseResponse?> Handle(
        GetWarehouseByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .Where(w => w.Id == request.Id && !w.IsDeleted)
            .Select(w => new WarehouseResponse
            {
                Id = w.Id,
                OrganizationId = w.OrganizationId,
                AddressId = w.AddressId,
                Name = w.Name,
                IsActive = w.IsActive,
                IsDeleted = w.IsDeleted
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
