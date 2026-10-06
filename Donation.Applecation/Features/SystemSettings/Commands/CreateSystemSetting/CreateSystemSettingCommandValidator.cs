using FluentValidation;

namespace Donation.Application.Features.SystemSettings.Commands.CreateSystemSetting;

public sealed class CreateSystemSettingCommandValidator : AbstractValidator<CreateSystemSettingCommand>
{
    public CreateSystemSettingCommandValidator()
    {
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
