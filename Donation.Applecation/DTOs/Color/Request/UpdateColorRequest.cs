namespace Donation.Application.DTOs.Color.Request;

public sealed class UpdateColorRequest
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
