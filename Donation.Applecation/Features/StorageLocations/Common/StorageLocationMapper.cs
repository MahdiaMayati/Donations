using Donation.Application.DTOs.StorageLocation.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.StorageLocations.Common;

internal static class StorageLocationMapper
{
    public static StorageLocationResponse Map(StorageLocation location) => new()
    {
        Id = location.Id,
        WarehouseId = location.WarehouseId,
        Code = location.Code
    };
}
