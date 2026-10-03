using FluentValidation;

namespace Donation.Application.Features.Materials.Commands.UpdateMaterialsBulk;

public sealed class UpdateMaterialsBulkCommandValidator : AbstractValidator<UpdateMaterialsBulkCommand>
{
    public UpdateMaterialsBulkCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one material is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

            item.RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        });
    }
}
