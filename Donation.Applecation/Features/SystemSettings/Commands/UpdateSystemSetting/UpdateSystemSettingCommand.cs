using Donation.Application.DTOs.SystemSetting.Response;
using MediatR;

namespace Donation.Application.Features.SystemSettings.Commands.UpdateSystemSetting;

public sealed record UpdateSystemSettingCommand(
    Guid Id,
    Guid? OrganizationId,
    string Key,
    string Value,
    string Type) : IRequest<SystemSettingResponse?>;
