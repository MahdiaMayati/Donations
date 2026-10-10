using Donation.Application.DTOs.SystemSetting.Response;
using MediatR;

namespace Donation.Application.Features.SystemSettings.Queries.GetSystemSettingById;

public sealed record GetSystemSettingByIdQuery(Guid Id) : IRequest<SystemSettingResponse?>;
