namespace Donation.Application.DTOs.StorageLocation.Response;

public sealed class StorageLocationResponse
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
}
