namespace Donation.Application.DTOs.Material.Response;

public sealed class MaterialResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}
