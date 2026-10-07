using Donation.Domain.Enums;

namespace Donation.Domain.Entities;

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Guid DonationRequestId { get; set; }
    public Guid ItemTypeId { get; set; }
    public Guid? MaterialId { get; set; }
    public Guid? StorageLocationId { get; set; }
    public Guid? SortedByUserId { get; set; }
    public string? Barcode { get; set; }
    public TargetGender TargetGender { get; set; }
    public AgeGroup AgeGroup { get; set; }
    public string Size { get; set; } = string.Empty;
    public Season Season { get; set; }
    public ItemCondition Condition { get; set; }
    public ItemSortingStatus SortingStatus { get; set; } = ItemSortingStatus.PendingReview;
    public ItemAvailabilityStatus AvailabilityStatus { get; set; } = ItemAvailabilityStatus.PendingReview;
    public string? SorterNotes { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Organization Organization { get; set; } = null!;
    public DonationRequest DonationRequest { get; set; } = null!;
    public ItemType ItemType { get; set; } = null!;
    public Material? Material { get; set; }
    public User? SortedByUser { get; set; }
    public StorageLocation? StorageLocation { get; set; }
    public ICollection<ItemPhoto> Photos { get; set; } = new List<ItemPhoto>();
    public ICollection<ItemColor> ItemColors { get; set; } = new List<ItemColor>();
    public ICollection<ItemStatusHistory> StatusHistory { get; set; } = new List<ItemStatusHistory>();
}
