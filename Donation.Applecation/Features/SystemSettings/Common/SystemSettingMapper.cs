using Donation.Application.DTOs.SystemSetting.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.SystemSettings.Common;

internal static class SystemSettingMapper
{
    public static SystemSettingResponse Map(SystemSetting entity) => new()
    {
        Id = entity.Id,
        OrganizationId = entity.OrganizationId,
        Key = entity.Key,
        Value = entity.Value,
        Type = entity.Type,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
