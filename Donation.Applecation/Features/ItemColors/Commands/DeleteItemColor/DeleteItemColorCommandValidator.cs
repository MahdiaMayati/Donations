using FluentValidation;

namespace Donation.Application.Features.ItemColors.Commands.DeleteItemColor;

public sealed class DeleteItemColorCommandValidator : AbstractValidator<DeleteItemColorCommand>
{
    public DeleteItemColorCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId is required.");

        RuleFor(x => x.ColorId)
            .NotEmpty().WithMessage("ColorId is required.");
    }
}
