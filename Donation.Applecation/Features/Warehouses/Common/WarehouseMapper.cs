using Donation.Application.DTOs.Warehouse.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.Warehouses.Common;

internal static class WarehouseMapper
{
    public static WarehouseResponse Map(Warehouse warehouse) => new()
    {
        Id = warehouse.Id,
        OrganizationId = warehouse.OrganizationId,
        AddressId = warehouse.AddressId,
        Name = warehouse.Name,
        IsActive = warehouse.IsActive,
        IsDeleted = warehouse.IsDeleted
    };
}
