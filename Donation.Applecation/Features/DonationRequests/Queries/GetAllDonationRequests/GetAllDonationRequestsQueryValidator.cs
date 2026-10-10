using FluentValidation;

namespace Donation.Application.Features.DonationRequests.Queries.GetAllDonationRequests;

public sealed class GetAllDonationRequestsQueryValidator : AbstractValidator<GetAllDonationRequestsQuery>
{
    public GetAllDonationRequestsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Limit).InclusiveBetween(1, 100);
    }
}
