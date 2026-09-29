using FluentValidation;

namespace Donation.Application.Features.Organizations.Queries.GetAllOrganizations;

public sealed class GetAllOrganizationsQueryValidator : AbstractValidator<GetAllOrganizationsQuery>
{
    public GetAllOrganizationsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
    }
}
