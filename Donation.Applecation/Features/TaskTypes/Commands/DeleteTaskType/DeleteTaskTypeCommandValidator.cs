using FluentValidation;

namespace Donation.Application.Features.TaskTypes.Commands.DeleteTaskType;

public sealed class DeleteTaskTypeCommandValidator : AbstractValidator<DeleteTaskTypeCommand>
{
    public DeleteTaskTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
