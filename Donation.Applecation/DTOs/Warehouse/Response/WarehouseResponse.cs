namespace Donation.Application.DTOs.Warehouse.Response;

public sealed class WarehouseResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid AddressId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
