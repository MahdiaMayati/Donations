using FluentValidation;

namespace Donation.Application.Features.Colors.Commands.DeleteColorsBulk;

public sealed class DeleteColorsBulkCommandValidator : AbstractValidator<DeleteColorsBulkCommand>
{
    public DeleteColorsBulkCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("At least one id is required.");
    }
}
