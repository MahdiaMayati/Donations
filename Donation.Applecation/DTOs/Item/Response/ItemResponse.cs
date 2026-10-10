namespace Donation.Application.DTOs.Item.Response;

public sealed class ItemResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid DonationRequestId { get; set; }
    public Guid ItemTypeId { get; set; }
    public Guid? MaterialId { get; set; }
    public Guid? StorageLocationId { get; set; }
    public Guid? SortedByUserId { get; set; }
    public string? Barcode { get; set; }
    public string TargetGender { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string SortingStatus { get; set; } = string.Empty;
    public string AvailabilityStatus { get; set; } = string.Empty;
    public string? SorterNotes { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public IReadOnlyList<Guid> ColorIds { get; set; } = Array.Empty<Guid>();
    public IReadOnlyList<ItemPhotoSummaryResponse> Photos { get; set; } = Array.Empty<ItemPhotoSummaryResponse>();
}

public sealed class ItemPhotoSummaryResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
