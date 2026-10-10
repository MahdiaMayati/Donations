using FluentValidation;

namespace Donation.Application.Features.Colors.Queries.GetColorById;

public sealed class GetColorByIdQueryValidator : AbstractValidator<GetColorByIdQuery>
{
    public GetColorByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
