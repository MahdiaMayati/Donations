using Donation.Application.DTOs.Color.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.Colors.Common;

internal static class ColorMapper
{
    public static ColorResponse Map(Color color) => new()
    {
        Id = color.Id,
        Name = color.Name,
        Code = color.Code,
        IsDeleted = color.IsDeleted
    };
}
