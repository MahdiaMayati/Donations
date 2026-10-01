namespace Donation.Application.DTOs.StorageLocation.Request;

public sealed class CreateStorageLocationRequest
{
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
}
