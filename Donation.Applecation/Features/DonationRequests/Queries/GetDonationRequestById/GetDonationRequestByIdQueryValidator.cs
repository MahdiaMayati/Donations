using FluentValidation;

namespace Donation.Application.Features.DonationRequests.Queries.GetDonationRequestById;

public sealed class GetDonationRequestByIdQueryValidator : AbstractValidator<GetDonationRequestByIdQuery>
{
    public GetDonationRequestByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
