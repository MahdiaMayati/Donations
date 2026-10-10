namespace Donation.Application.DTOs.DonationRequest.Response;

public sealed class DonationRequestPhotoResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
