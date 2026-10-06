namespace Donation.Application.DTOs.DonationRequest.Response;

public sealed class DonationRequestResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid DonorId { get; set; }
    public Guid? PickupAddressId { get; set; }
    public string DeliveryMethod { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EstimatedItemCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<DonationRequestPhotoResponse> Photos { get; set; } = Array.Empty<DonationRequestPhotoResponse>();
    public IReadOnlyList<DonationRequestItemResponse> Items { get; set; } = Array.Empty<DonationRequestItemResponse>();
}
