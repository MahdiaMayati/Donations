using FluentValidation;

namespace Donation.Application.Features.ItemCategories.Commands.CreateItemCategory;

public sealed class CreateItemCategoryCommandValidator : AbstractValidator<CreateItemCategoryCommand>
{
    public CreateItemCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}
