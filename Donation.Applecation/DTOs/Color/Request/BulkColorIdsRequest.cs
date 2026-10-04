namespace Donation.Application.DTOs.Color.Request;

public sealed class BulkColorIdsRequest
{
    public List<Guid> Ids { get; set; } = new();
}
