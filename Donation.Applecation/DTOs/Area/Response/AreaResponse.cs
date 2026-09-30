namespace Donation.Application.DTOs.Area.Response;

public class AreaResponse
{
    public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public string Name { get; set; } = string.Empty;
}
