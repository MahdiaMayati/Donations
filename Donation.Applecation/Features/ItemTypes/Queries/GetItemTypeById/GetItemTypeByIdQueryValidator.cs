using FluentValidation;

namespace Donation.Application.Features.ItemTypes.Queries.GetItemTypeById;

public sealed class GetItemTypeByIdQueryValidator : AbstractValidator<GetItemTypeByIdQuery>
{
    public GetItemTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
