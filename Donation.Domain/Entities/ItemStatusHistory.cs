using Donation.Domain.Enums;

namespace Donation.Domain.Entities;

public class ItemStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ItemId { get; set; }
    public ItemSortingStatus? OldStatus { get; set; }
    public ItemSortingStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public Guid? ChangedByUserId { get; set; }

    public Item Item { get; set; } = null!;
    public User? ChangedByUser { get; set; }
}
