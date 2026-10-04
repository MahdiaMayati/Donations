using FluentValidation;

namespace Donation.Application.Features.Colors.Commands.UpdateColorsBulk;

public sealed class UpdateColorsBulkCommandValidator : AbstractValidator<UpdateColorsBulkCommand>
{
    public UpdateColorsBulkCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one color is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

            item.RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            item.RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .Must(code => !string.IsNullOrWhiteSpace(code)).WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");
        });
    }
}
