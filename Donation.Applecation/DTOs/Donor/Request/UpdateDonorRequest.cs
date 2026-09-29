namespace Donation.Application.DTOs.Donor.Request;

/// <summary>
/// Partial update contract: null / omitted fields are left unchanged.
/// </summary>
public sealed class UpdateDonorRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public string? PreferredContactMethod { get; set; }
    public DonorAddressRequest? Address { get; set; }
}
