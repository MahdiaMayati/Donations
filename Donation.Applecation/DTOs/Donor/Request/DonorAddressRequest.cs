namespace Donation.Application.DTOs.Donor.Request;

public sealed class DonorAddressRequest
{
    public Guid AreaId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
