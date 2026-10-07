using Donation.Domain.Enums;

namespace Donation.Application.DTOs.DonationRequest.Request;

public sealed class UpdateDonationRequestRequest
{
    public Guid? PickupAddressId { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public string Description { get; set; } = string.Empty;
    public int EstimatedItemCount { get; set; }
    public List<string> RequestPhotoUrls { get; set; } = new();
    public List<DonationRequestItemRequest> Items { get; set; } = new();
}
