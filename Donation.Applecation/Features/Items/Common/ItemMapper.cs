using Donation.Application.DTOs.Item.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.Items.Common;

internal static class ItemMapper
{
    public static ItemResponse Map(Item item) => new()
    {
        Id = item.Id,
        OrganizationId = item.OrganizationId,
        DonationRequestId = item.DonationRequestId,
        ItemTypeId = item.ItemTypeId,
        MaterialId = item.MaterialId,
        StorageLocationId = item.StorageLocationId,
        SortedByUserId = item.SortedByUserId,
        Barcode = item.Barcode,
        TargetGender = item.TargetGender.ToString(),
        AgeGroup = item.AgeGroup.ToString(),
        Size = item.Size,
        Season = item.Season.ToString(),
        Condition = item.Condition.ToString(),
        SortingStatus = item.SortingStatus.ToString(),
        AvailabilityStatus = item.AvailabilityStatus.ToString(),
        SorterNotes = item.SorterNotes,
        ReceivedAt = item.ReceivedAt,
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt,
        IsDeleted = item.IsDeleted,
        DeletedAt = item.DeletedAt,
        ColorIds = item.ItemColors.Select(ic => ic.ColorId).ToList(),
        Photos = item.Photos
            .OrderBy(p => p.CreatedAt)
            .Select(p => new ItemPhotoSummaryResponse
            {
                Id = p.Id,
                Url = p.Url,
                CreatedAt = p.CreatedAt
            })
            .ToList()
    };
}
