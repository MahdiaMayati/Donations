using FluentValidation;

namespace Donation.Application.Features.TaskTypes.Queries.GetTaskTypeById;

public sealed class GetTaskTypeByIdQueryValidator : AbstractValidator<GetTaskTypeByIdQuery>
{
    public GetTaskTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
