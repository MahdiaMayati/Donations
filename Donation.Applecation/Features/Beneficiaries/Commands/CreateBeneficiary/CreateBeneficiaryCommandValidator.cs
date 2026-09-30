using FluentValidation;

namespace Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;

public sealed class CreateBeneficiaryCommandValidator : AbstractValidator<CreateBeneficiaryCommand>
{
    public CreateBeneficiaryCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .NotEmpty().WithMessage("AddressId is required.");

        RuleFor(x => x.IdPhotoUrl)
            .Cascade(CascadeMode.Stop)
            .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("IdPhotoUrl is required.")
            .MaximumLength(1000).WithMessage("IdPhotoUrl must not exceed 1000 characters.");
    }
}
