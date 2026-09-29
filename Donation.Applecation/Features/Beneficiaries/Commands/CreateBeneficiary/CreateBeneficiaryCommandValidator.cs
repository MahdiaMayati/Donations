using FluentValidation;

namespace Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;

public sealed class CreateBeneficiaryCommandValidator : AbstractValidator<CreateBeneficiaryCommand>
{
    public CreateBeneficiaryCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .GreaterThan(0).WithMessage("AddressId must be greater than zero.");

        RuleFor(x => x.IdPhotoUrl)
            .Cascade(CascadeMode.Stop)
            .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("IdPhotoUrl is required.")
            .MaximumLength(1000).WithMessage("IdPhotoUrl must not exceed 1000 characters.");
    }
}
