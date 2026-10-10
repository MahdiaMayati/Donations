using Donation.Application.DTOs.ItemType.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.ItemTypes;

public static class ItemTypeMappings
{
    public static ItemTypeResponse ToResponse(ItemType entity) => new()
    {
        Id = entity.Id,
        CategoryId = entity.CategoryId,
        CategoryName = entity.Category?.Name ?? string.Empty,
        Name = entity.Name,
        OutfitUnits = entity.OutfitUnits,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
