using FluentValidation;

namespace Donation.Application.Features.Items.Commands.UpdateItemSortingStatus;

public sealed class UpdateItemSortingStatusCommandValidator
    : AbstractValidator<UpdateItemSortingStatusCommand>
{
    public UpdateItemSortingStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.SortingStatus)
            .IsInEnum().WithMessage("SortingStatus is invalid.");
    }
}
