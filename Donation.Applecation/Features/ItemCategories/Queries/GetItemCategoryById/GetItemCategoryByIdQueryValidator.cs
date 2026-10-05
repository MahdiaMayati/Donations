using FluentValidation;

namespace Donation.Application.Features.ItemCategories.Queries.GetItemCategoryById;

public sealed class GetItemCategoryByIdQueryValidator : AbstractValidator<GetItemCategoryByIdQuery>
{
    public GetItemCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
