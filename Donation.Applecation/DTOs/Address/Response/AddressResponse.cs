namespace Donation.Application.DTOs.Address.Response;

public class AddressResponse
{
    public int Id { get; set; }
    public int AreaId { get; set; }
    public Guid UserId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
}
