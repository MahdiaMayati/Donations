using FluentValidation;

namespace Donation.Application.Features.TaskTypes.Commands.CreateTaskType;

public sealed class CreateTaskTypeCommandValidator : AbstractValidator<CreateTaskTypeCommand>
{
    public CreateTaskTypeCommandValidator()
    {
        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .Must(code => !string.IsNullOrWhiteSpace(code)).WithMessage("Code is required.")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
