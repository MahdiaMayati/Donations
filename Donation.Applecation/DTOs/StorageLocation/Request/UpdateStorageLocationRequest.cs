namespace Donation.Application.DTOs.StorageLocation.Request;

public sealed class UpdateStorageLocationRequest
{
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
}
