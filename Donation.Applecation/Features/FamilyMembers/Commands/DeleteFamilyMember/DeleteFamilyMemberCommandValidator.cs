using FluentValidation;

namespace Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMember;

public sealed class DeleteFamilyMemberCommandValidator : AbstractValidator<DeleteFamilyMemberCommand>
{
    public DeleteFamilyMemberCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");
    }
}
