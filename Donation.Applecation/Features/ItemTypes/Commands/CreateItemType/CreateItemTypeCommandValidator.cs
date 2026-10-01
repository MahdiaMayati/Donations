using FluentValidation;

namespace Donation.Application.Features.ItemTypes.Commands.CreateItemType;

public sealed class CreateItemTypeCommandValidator : AbstractValidator<CreateItemTypeCommand>
{
    public CreateItemTypeCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        RuleFor(x => x.OutfitUnits)
            .GreaterThan(0).WithMessage("OutfitUnits must be greater than 0.");
    }
}
