using FluentValidation;

namespace Donation.Application.Features.Beneficiaries.Commands.RestoreBeneficiary;

public sealed class RestoreBeneficiaryCommandValidator : AbstractValidator<RestoreBeneficiaryCommand>
{
    public RestoreBeneficiaryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
