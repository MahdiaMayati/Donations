using FluentValidation;

namespace Donation.Application.Features.Materials.Queries.GetMaterialById;

public sealed class GetMaterialByIdQueryValidator : AbstractValidator<GetMaterialByIdQuery>
{
    public GetMaterialByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
