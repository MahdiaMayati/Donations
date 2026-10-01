using FluentValidation;

namespace Donation.Application.Features.ItemCategories.Commands.DeleteItemCategory;

public sealed class DeleteItemCategoryCommandValidator : AbstractValidator<DeleteItemCategoryCommand>
{
    public DeleteItemCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
