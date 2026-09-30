using FluentValidation;

namespace Donation.Application.Features.Beneficiaries.Commands.UpdateBeneficiary;

public sealed class UpdateBeneficiaryCommandValidator : AbstractValidator<UpdateBeneficiaryCommand>
{
    public UpdateBeneficiaryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.AddressId)
            .NotEmpty().WithMessage("AddressId is required.");

        RuleFor(x => x.IdPhotoUrl)
            .Cascade(CascadeMode.Stop)
            .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("IdPhotoUrl is required.")
            .MaximumLength(1000).WithMessage("IdPhotoUrl must not exceed 1000 characters.");

        RuleFor(x => x.VerificationStatus)
            .IsInEnum()
            .When(x => x.VerificationStatus.HasValue);
    }
}
