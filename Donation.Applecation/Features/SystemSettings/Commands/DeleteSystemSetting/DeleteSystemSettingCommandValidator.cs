using FluentValidation;

namespace Donation.Application.Features.SystemSettings.Commands.DeleteSystemSetting;

public sealed class DeleteSystemSettingCommandValidator : AbstractValidator<DeleteSystemSettingCommand>
{
    public DeleteSystemSettingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
