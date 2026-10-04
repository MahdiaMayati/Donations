namespace Donation.Application.DTOs.Color.Request;

public sealed class CreateColorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
