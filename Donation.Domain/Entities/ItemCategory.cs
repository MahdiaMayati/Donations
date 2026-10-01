using Donation.Domain.Common;

namespace Donation.Domain.Entities;

public class ItemCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<ItemType> ItemTypes { get; set; } = new List<ItemType>();
}
