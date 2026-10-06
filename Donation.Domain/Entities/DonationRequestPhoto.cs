namespace Donation.Domain.Entities;

public class DonationRequestPhoto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DonationRequestId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DonationRequest DonationRequest { get; set; } = null!;
}
