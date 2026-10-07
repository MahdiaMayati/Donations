using FluentValidation;

namespace Donation.Application.Features.ItemStatusHistories.Commands.DeleteItemStatusHistory;

public sealed class DeleteItemStatusHistoryCommandValidator
    : AbstractValidator<DeleteItemStatusHistoryCommand>
{
    public DeleteItemStatusHistoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
