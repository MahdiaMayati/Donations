using Donation.Application.DTOs.Material.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.Materials.Common;

internal static class MaterialMapper
{
    public static MaterialResponse Map(Material material) => new()
    {
        Id = material.Id,
        Name = material.Name,
        IsDeleted = material.IsDeleted
    };
}
