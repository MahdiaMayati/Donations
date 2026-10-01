namespace Donation.Domain.Entities;

public class StorageLocation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;

    public Warehouse Warehouse { get; set; } = null!;
}
