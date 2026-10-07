using Donation.Domain.Enums;

namespace Donation.Application.DTOs.Item.Request;

public sealed class UpdateItemSortingStatusRequest
{
    public ItemSortingStatus SortingStatus { get; set; }
}
