using FluentValidation;

namespace Donation.Application.Features.Beneficiaries.Commands.UpdateBeneficiary;

public sealed class UpdateBeneficiaryCommandValidator : AbstractValidator<UpdateBeneficiaryCommand>
{
    public UpdateBeneficiaryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");

        RuleFor(x => x.AddressId)
            .GreaterThan(0).WithMessage("AddressId must be greater than zero.");

        RuleFor(x => x.IdPhotoUrl)
            .Cascade(CascadeMode.Stop)
            .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("IdPhotoUrl is required.")
            .MaximumLength(1000).WithMessage("IdPhotoUrl must not exceed 1000 characters.");

        RuleFor(x => x.VerificationStatus)
            .IsInEnum()
            .When(x => x.VerificationStatus.HasValue);
    }
}
