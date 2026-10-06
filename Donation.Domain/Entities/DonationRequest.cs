using Donation.Domain.Enums;

namespace Donation.Domain.Entities;

public class DonationRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Guid DonorId { get; set; }
    public Guid? PickupAddressId { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public string Description { get; set; } = string.Empty;
    public int EstimatedItemCount { get; set; }
    public DonationRequestStatus Status { get; set; } = DonationRequestStatus.Submitted;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Organization Organization { get; set; } = null!;
    public Donor Donor { get; set; } = null!;
    public Address? PickupAddress { get; set; }
    public ICollection<DonationRequestPhoto> Photos { get; set; } = new List<DonationRequestPhoto>();
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
