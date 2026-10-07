using Donation.Application.DTOs.ItemStatusHistory.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.ItemStatusHistories.Common;

internal static class ItemStatusHistoryMapper
{
    public static ItemStatusHistoryResponse Map(ItemStatusHistory history) => new()
    {
        Id = history.Id,
        ItemId = history.ItemId,
        OldStatus = history.OldStatus?.ToString(),
        NewStatus = history.NewStatus.ToString(),
        ChangedAt = history.ChangedAt,
        ChangedByUserId = history.ChangedByUserId
    };
}
