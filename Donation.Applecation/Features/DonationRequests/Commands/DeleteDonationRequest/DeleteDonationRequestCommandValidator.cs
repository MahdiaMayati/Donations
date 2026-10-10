using FluentValidation;

namespace Donation.Application.Features.DonationRequests.Commands.DeleteDonationRequest;

public sealed class DeleteDonationRequestCommandValidator : AbstractValidator<DeleteDonationRequestCommand>
{
    public DeleteDonationRequestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
