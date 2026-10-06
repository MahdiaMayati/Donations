using Donation.Application.DTOs.ItemPhoto.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.ItemPhotos.Common;

internal static class ItemPhotoMapper
{
    public static ItemPhotoResponse Map(ItemPhoto photo) => new()
    {
        Id = photo.Id,
        ItemId = photo.ItemId,
        Url = photo.Url,
        CreatedAt = photo.CreatedAt
    };
}
