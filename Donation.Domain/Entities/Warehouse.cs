namespace Donation.Domain.Entities;

public class Warehouse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Guid AddressId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }

    public Organization Organization { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public ICollection<StorageLocation> StorageLocations { get; set; } = new List<StorageLocation>();
}
