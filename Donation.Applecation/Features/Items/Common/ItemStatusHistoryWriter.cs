using Donation.Application.Abstractions.Persistence;
using Donation.Domain.Entities;
using Donation.Domain.Enums;

namespace Donation.Application.Features.Items.Common;

internal static class ItemStatusHistoryWriter
{
    public static void Append(
        IAppDbContext context,
        Item item,
        ItemSortingStatus? oldStatus,
        ItemSortingStatus newStatus,
        Guid? changedByUserId,
        DateTime changedAt)
    {
        context.ItemStatusHistories.Add(new ItemStatusHistory
        {
            ItemId = item.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedAt = changedAt,
            ChangedByUserId = changedByUserId
        });
    }
}
