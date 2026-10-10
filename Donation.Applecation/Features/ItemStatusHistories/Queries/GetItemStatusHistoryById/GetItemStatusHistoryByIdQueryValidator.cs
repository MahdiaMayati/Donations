using FluentValidation;

namespace Donation.Application.Features.ItemStatusHistories.Queries.GetItemStatusHistoryById;

public sealed class GetItemStatusHistoryByIdQueryValidator
    : AbstractValidator<GetItemStatusHistoryByIdQuery>
{
    public GetItemStatusHistoryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
