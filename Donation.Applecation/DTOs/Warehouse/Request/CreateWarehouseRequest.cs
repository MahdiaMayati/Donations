namespace Donation.Application.DTOs.Warehouse.Request;

public sealed class CreateWarehouseRequest
{
    public Guid OrganizationId { get; set; }
    public Guid AddressId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
