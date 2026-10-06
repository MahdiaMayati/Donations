using MediatR;

namespace Donation.Application.Features.SystemSettings.Commands.DeleteSystemSetting;

public sealed record DeleteSystemSettingCommand(Guid Id) : IRequest<bool>;
