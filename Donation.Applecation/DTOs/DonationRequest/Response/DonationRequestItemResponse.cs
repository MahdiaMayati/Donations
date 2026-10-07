namespace Donation.Application.DTOs.DonationRequest.Response;

public sealed class DonationRequestItemResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ItemTypeId { get; set; }
    public Guid? MaterialId { get; set; }
    public string? Barcode { get; set; }
    public string TargetGender { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string SortingStatus { get; set; } = string.Empty;
    public string AvailabilityStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<ItemPhotoResponse> Photos { get; set; } = Array.Empty<ItemPhotoResponse>();
    public IReadOnlyList<Guid> ColorIds { get; set; } = Array.Empty<Guid>();
}
