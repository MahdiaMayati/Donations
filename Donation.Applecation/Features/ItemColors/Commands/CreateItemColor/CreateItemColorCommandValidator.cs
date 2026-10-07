using FluentValidation;

namespace Donation.Application.Features.ItemColors.Commands.CreateItemColor;

public sealed class CreateItemColorCommandValidator : AbstractValidator<CreateItemColorCommand>
{
    public CreateItemColorCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId is required.");

        RuleFor(x => x.ColorId)
            .NotEmpty().WithMessage("ColorId is required.");
    }
}
