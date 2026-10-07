using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.SystemSetting.Response;
using MediatR;

namespace Donation.Application.Features.SystemSettings.Queries.GetAllSystemSettings;

public sealed record GetAllSystemSettingsQuery(
    string? Type,
    Guid? OrganizationId,
    int Page,
    int Limit,
    string? Search) : IRequest<PaginatedResult<SystemSettingResponse>>;
