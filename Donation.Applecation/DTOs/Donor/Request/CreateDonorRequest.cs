namespace Donation.Application.DTOs.Donor.Request;

public sealed class CreateDonorRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PreferredContactMethod { get; set; } = string.Empty;
    public DonorAddressRequest Address { get; set; } = new();
}
