using FluentValidation;
using Donation.Application.Common.Pagination;

namespace Donation.Application.Features.Organizations.Queries.GetAllOrganizations;

public sealed class GetAllOrganizationsQueryValidator : AbstractValidator<GetAllOrganizationsQuery>
{
    public GetAllOrganizationsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("page must be at least 1.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, PaginationRequest.MaxLimit)
            .WithMessage($"limit must be between 1 and {PaginationRequest.MaxLimit}.");
    }
}
