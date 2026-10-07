using Donation.Application.DTOs.DonationRequest.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.DonationRequests.Common;

internal static class DonationRequestMapper
{
    public static DonationRequestResponse Map(DonationRequest entity) => new()
    {
        Id = entity.Id,
        OrganizationId = entity.OrganizationId,
        DonorId = entity.DonorId,
        PickupAddressId = entity.PickupAddressId,
        DeliveryMethod = entity.DeliveryMethod.ToString(),
        Description = entity.Description,
        EstimatedItemCount = entity.EstimatedItemCount,
        Status = entity.Status.ToString(),
        SubmittedAt = entity.SubmittedAt,
        IsDeleted = entity.IsDeleted,
        DeletedAt = entity.DeletedAt,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Photos = entity.Photos
            .OrderBy(p => p.CreatedAt)
            .Select(p => new DonationRequestPhotoResponse
            {
                Id = p.Id,
                Url = p.Url,
                CreatedAt = p.CreatedAt
            })
            .ToList(),
        Items = entity.Items
            .Where(i => !i.IsDeleted)
            .OrderBy(i => i.CreatedAt)
            .Select(MapItem)
            .ToList()
    };

    private static DonationRequestItemResponse MapItem(Item item) => new()
    {
        Id = item.Id,
        OrganizationId = item.OrganizationId,
        ItemTypeId = item.ItemTypeId,
        MaterialId = item.MaterialId,
        Barcode = item.Barcode,
        TargetGender = item.TargetGender.ToString(),
        AgeGroup = item.AgeGroup.ToString(),
        Size = item.Size,
        Season = item.Season.ToString(),
        Condition = item.Condition.ToString(),
        SortingStatus = item.SortingStatus.ToString(),
        AvailabilityStatus = item.AvailabilityStatus.ToString(),
        CreatedAt = item.CreatedAt,
        Photos = item.Photos
            .OrderBy(p => p.CreatedAt)
            .Select(p => new ItemPhotoResponse
            {
                Id = p.Id,
                Url = p.Url,
                CreatedAt = p.CreatedAt
            })
            .ToList(),
        ColorIds = item.ItemColors.Select(ic => ic.ColorId).ToList()
    };
}
