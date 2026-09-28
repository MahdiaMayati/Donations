namespace Donation.Application.DTOs.Area.Request;

public class UpdateAreaRequest
{
    public int CityId { get; set; }
    public string Name { get; set; } = string.Empty;
}
