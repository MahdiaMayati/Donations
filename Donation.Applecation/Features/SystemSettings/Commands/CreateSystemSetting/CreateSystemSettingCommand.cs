using Donation.Application.DTOs.SystemSetting.Response;
using MediatR;

namespace Donation.Application.Features.SystemSettings.Commands.CreateSystemSetting;

public sealed record CreateSystemSettingCommand(
    Guid? OrganizationId,
    string Key,
    string Value,
    string Type) : IRequest<SystemSettingResponse>;
