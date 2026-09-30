using FluentValidation;

namespace Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMember;

public sealed class UpdateFamilyMemberCommandValidator : AbstractValidator<UpdateFamilyMemberCommand>
{
    public UpdateFamilyMemberCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.BeneficiaryId)
            .NotEmpty().WithMessage("BeneficiaryId is required.");

        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("FullName is required.")
            .MaximumLength(200).WithMessage("FullName must not exceed 200 characters.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow.Date.AddDays(1)).WithMessage("BirthDate cannot be in the future.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender is invalid.");

        RuleFor(x => x.ClothingSize)
            .IsInEnum().WithMessage("ClothingSize is invalid.");

        RuleFor(x => x.ShoeSize)
            .Cascade(CascadeMode.Stop)
            .Must(size => !string.IsNullOrWhiteSpace(size)).WithMessage("ShoeSize is required.")
            .MaximumLength(20).WithMessage("ShoeSize must not exceed 20 characters.");
    }
}
