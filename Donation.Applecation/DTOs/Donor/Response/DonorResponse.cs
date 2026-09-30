namespace Donation.Application.DTOs.Donor.Response;

public sealed class DonorResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string PreferredContactMethod { get; set; } = string.Empty;
    public DonorAddressResponse? Address { get; set; }
}
