namespace Donation.Application.DTOs.ItemStatusHistory.Response;

public sealed class ItemStatusHistoryResponse
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public Guid? ChangedByUserId { get; set; }
}
