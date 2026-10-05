using Donation.Application.DTOs.ItemCategory.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.ItemCategories;

public static class ItemCategoryMappings
{
    public static ItemCategoryResponse ToResponse(ItemCategory entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
