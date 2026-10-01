namespace Donation.Application.DTOs.Warehouse.Request;

public sealed class UpdateWarehouseRequest
{
    public Guid OrganizationId { get; set; }
    public Guid AddressId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
