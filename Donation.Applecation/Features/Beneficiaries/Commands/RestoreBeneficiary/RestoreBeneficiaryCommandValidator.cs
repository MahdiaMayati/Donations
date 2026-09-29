using FluentValidation;

namespace Donation.Application.Features.Beneficiaries.Commands.RestoreBeneficiary;

public sealed class RestoreBeneficiaryCommandValidator : AbstractValidator<RestoreBeneficiaryCommand>
{
    public RestoreBeneficiaryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");
    }
}
