namespace Donation.Application.DTOs.Material.Request;

public sealed class UpdateMaterialRequest
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
