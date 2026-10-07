using Donation.Domain.Enums;

namespace Donation.Application.DTOs.Item.Request;

public sealed class CreateItemRequest
{
    /// <summary>Optional when <see cref="DonationRequestId"/> is set; resolved from the donation request if omitted.</summary>
    public Guid? OrganizationId { get; set; }
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
    public ItemAvailabilityStatus? AvailabilityStatus { get; set; }
    public string? SorterNotes { get; set; }
    public DateTime? ReceivedAt { get; set; }
}
