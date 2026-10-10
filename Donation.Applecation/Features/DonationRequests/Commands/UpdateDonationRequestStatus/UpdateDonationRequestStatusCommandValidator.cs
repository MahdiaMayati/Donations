using FluentValidation;

namespace Donation.Application.Features.DonationRequests.Commands.UpdateDonationRequestStatus;

public sealed class UpdateDonationRequestStatusCommandValidator
    : AbstractValidator<UpdateDonationRequestStatusCommand>
{
    public UpdateDonationRequestStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
