using FluentValidation;

namespace Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMember;

public sealed class DeleteFamilyMemberCommandValidator : AbstractValidator<DeleteFamilyMemberCommand>
{
    public DeleteFamilyMemberCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
