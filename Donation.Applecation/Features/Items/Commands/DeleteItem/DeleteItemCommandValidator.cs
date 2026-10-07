using FluentValidation;

namespace Donation.Application.Features.Items.Commands.DeleteItem;

public sealed class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
