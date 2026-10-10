using Donation.Domain.Enums;

namespace Donation.Application.DTOs.ItemStatusHistory.Request;

public sealed class CreateItemStatusHistoryRequest
{
    public Guid ItemId { get; set; }
    public ItemSortingStatus NewStatus { get; set; }
    public ItemSortingStatus? OldStatus { get; set; }
}
