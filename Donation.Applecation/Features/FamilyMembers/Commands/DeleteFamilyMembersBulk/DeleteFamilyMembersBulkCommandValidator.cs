using FluentValidation;

namespace Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMembersBulk;

public sealed class DeleteFamilyMembersBulkCommandValidator : AbstractValidator<DeleteFamilyMembersBulkCommand>
{
    public DeleteFamilyMembersBulkCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("At least one Id is required.");

        RuleForEach(x => x.Ids)
            .NotEmpty().WithMessage("Each Id is required.");
    }
}
