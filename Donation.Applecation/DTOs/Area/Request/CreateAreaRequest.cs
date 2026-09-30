namespace Donation.Application.DTOs.Area.Request;

public class CreateAreaRequest
{
    public Guid CityId { get; set; }
    public string Name { get; set; } = string.Empty;
}
