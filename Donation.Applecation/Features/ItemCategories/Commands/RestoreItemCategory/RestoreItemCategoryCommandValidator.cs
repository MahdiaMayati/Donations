using FluentValidation;

namespace Donation.Application.Features.ItemCategories.Commands.RestoreItemCategory;

public sealed class RestoreItemCategoryCommandValidator : AbstractValidator<RestoreItemCategoryCommand>
{
    public RestoreItemCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
