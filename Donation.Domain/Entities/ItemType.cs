using Donation.Domain.Common;

namespace Donation.Domain.Entities;

public class ItemType : BaseEntity
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal OutfitUnits { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ItemCategory Category { get; set; } = null!;
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
