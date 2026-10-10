namespace Donation.Application.DTOs.Material.Request;

public sealed class BulkMaterialIdsRequest
{
    public List<Guid> Ids { get; set; } = new();
}
