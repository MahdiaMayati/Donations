using Donation.Application.DTOs.ItemColor.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.ItemColors.Common;

internal static class ItemColorMapper
{
    public static ItemColorResponse Map(ItemColor link) => new()
    {
        ItemId = link.ItemId,
        ColorId = link.ColorId,
        ColorName = link.Color?.Name,
        ColorCode = link.Color?.Code
    };
}
