using Donation.Domain.Enums;

namespace Donation.Application.DTOs.DonationRequest.Request;

public sealed class CreateDonationRequestRequest
{
    public Guid OrganizationId { get; set; }
    /// <summary>Optional for Admin creating on behalf of a donor; donors use their own profile.</summary>
    public Guid? DonorId { get; set; }
    public Guid? PickupAddressId { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public string Description { get; set; } = string.Empty;
    public int EstimatedItemCount { get; set; }
    public List<string> RequestPhotoUrls { get; set; } = new();
    public List<DonationRequestItemRequest> Items { get; set; } = new();
}
