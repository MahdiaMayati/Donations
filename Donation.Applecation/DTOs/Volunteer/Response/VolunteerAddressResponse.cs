namespace Donation.Application.DTOs.Volunteer.Response;

public sealed class VolunteerAddressResponse
{
    public Guid Id { get; set; }
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
