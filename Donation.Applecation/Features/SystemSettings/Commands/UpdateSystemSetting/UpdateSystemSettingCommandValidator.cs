using FluentValidation;

namespace Donation.Application.Features.SystemSettings.Commands.UpdateSystemSetting;

public sealed class UpdateSystemSettingCommandValidator : AbstractValidator<UpdateSystemSettingCommand>
{
    public UpdateSystemSettingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Key)
            .Cascade(CascadeMode.Stop)
            .Must(k => !string.IsNullOrWhiteSpace(k)).WithMessage("Key is required.")
            .MaximumLength(200);

        RuleFor(x => x.Value)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Value is required.")
            .MaximumLength(4000);

        RuleFor(x => x.Type)
            .Cascade(CascadeMode.Stop)
            .Must(t => !string.IsNullOrWhiteSpace(t)).WithMessage("Type is required.")
            .MaximumLength(100);
    }
}
