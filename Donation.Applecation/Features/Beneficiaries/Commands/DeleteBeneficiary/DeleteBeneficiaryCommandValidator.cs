using FluentValidation;

namespace Donation.Application.Features.Beneficiaries.Commands.DeleteBeneficiary;

public sealed class DeleteBeneficiaryCommandValidator : AbstractValidator<DeleteBeneficiaryCommand>
{
    public DeleteBeneficiaryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
