using FluentValidation;

namespace Donation.Application.Features.FamilyMembers.Commands.CreateFamilyMember;

public sealed class CreateFamilyMemberCommandValidator : AbstractValidator<CreateFamilyMemberCommand>
{
    public CreateFamilyMemberCommandValidator()
    {
        RuleFor(x => x.HeadOfHouseholdId)
            .NotEmpty().WithMessage("HeadOfHouseholdId is required.");

        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("FullName is required.")
            .MaximumLength(200).WithMessage("FullName must not exceed 200 characters.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow.Date.AddDays(1)).WithMessage("DateOfBirth cannot be in the future.");

        RuleFor(x => x.ClothingSize)
            .Cascade(CascadeMode.Stop)
            .Must(size => !string.IsNullOrWhiteSpace(size)).WithMessage("ClothingSize is required.")
            .MaximumLength(20).WithMessage("ClothingSize must not exceed 20 characters.");

        RuleFor(x => x.ShoeSize)
            .Cascade(CascadeMode.Stop)
            .Must(size => !string.IsNullOrWhiteSpace(size)).WithMessage("ShoeSize is required.")
            .MaximumLength(20).WithMessage("ShoeSize must not exceed 20 characters.");
    }
}
