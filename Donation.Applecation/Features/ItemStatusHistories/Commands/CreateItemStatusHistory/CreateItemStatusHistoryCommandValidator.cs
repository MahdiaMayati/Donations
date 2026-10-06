using FluentValidation;

namespace Donation.Application.Features.ItemStatusHistories.Commands.CreateItemStatusHistory;

public sealed class CreateItemStatusHistoryCommandValidator
    : AbstractValidator<CreateItemStatusHistoryCommand>
{
    public CreateItemStatusHistoryCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("NewStatus is invalid.");

        RuleFor(x => x.OldStatus)
            .IsInEnum().WithMessage("OldStatus is invalid.")
            .When(x => x.OldStatus.HasValue);
    }
}
