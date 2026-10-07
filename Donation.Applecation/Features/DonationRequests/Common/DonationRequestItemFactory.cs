using Donation.Application.DTOs.DonationRequest.Request;
using Donation.Domain.Entities;
using Donation.Domain.Enums;

namespace Donation.Application.Features.DonationRequests.Common;

internal static class DonationRequestItemFactory
{
    public static Item CreateItem(
        DonationRequestItemRequest request,
        Guid organizationId,
        DateTime now,
        Guid? currentUserId)
    {
        var item = new Item
        {
            OrganizationId = organizationId,
            ItemTypeId = request.ItemTypeId,
            MaterialId = request.MaterialId is Guid mid && mid != Guid.Empty ? mid : null,
            Barcode = NormalizeBarcode(request.Barcode),
            TargetGender = request.TargetGender,
            AgeGroup = request.AgeGroup,
            Size = request.Size.Trim(),
            Season = request.Season,
            Condition = request.Condition,
            SortingStatus = ItemSortingStatus.PendingReview,
            AvailabilityStatus = ItemAvailabilityStatus.PendingReview,
            CreatedAt = now,
            IsDeleted = false
        };

        ApplyColors(item, request.ColorIds);

        item.StatusHistory.Add(new ItemStatusHistory
        {
            OldStatus = null,
            NewStatus = ItemSortingStatus.PendingReview,
            ChangedAt = now,
            ChangedByUserId = currentUserId
        });

        return item;
    }

    public static void ApplyItemFields(Item item, DonationRequestItemRequest request, DateTime now)
    {
        item.ItemTypeId = request.ItemTypeId;
        item.MaterialId = request.MaterialId is Guid mid && mid != Guid.Empty ? mid : null;
        item.Barcode = NormalizeBarcode(request.Barcode);
        item.TargetGender = request.TargetGender;
        item.AgeGroup = request.AgeGroup;
        item.Size = request.Size.Trim();
        item.Season = request.Season;
        item.Condition = request.Condition;
        item.UpdatedAt = now;
    }

    public static void ApplyColors(Item item, IEnumerable<Guid> colorIds)
    {
        var desired = colorIds.Where(id => id != Guid.Empty).Distinct().ToHashSet();
        var existing = item.ItemColors.ToList();

        foreach (var link in existing.Where(ic => !desired.Contains(ic.ColorId)))
        {
            item.ItemColors.Remove(link);
        }

        var present = item.ItemColors.Select(ic => ic.ColorId).ToHashSet();
        foreach (var colorId in desired.Where(id => !present.Contains(id)))
        {
            item.ItemColors.Add(new ItemColor { ColorId = colorId });
        }
    }

    public static string? NormalizeBarcode(string? barcode)
        => string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();
}
