using FluentValidation;

namespace Donation.Application.Features.Items.Commands.CreateItem;

public sealed class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.DonationRequestId)
            .NotEmpty().WithMessage("DonationRequestId is required.");

        RuleFor(x => x.ItemTypeId)
            .NotEmpty().WithMessage("ItemTypeId is required.");

        RuleFor(x => x.Size)
            .Cascade(CascadeMode.Stop)
            .Must(size => !string.IsNullOrWhiteSpace(size)).WithMessage("Size is required.")
            .MaximumLength(50).WithMessage("Size must not exceed 50 characters.");

        RuleFor(x => x.Barcode)
            .MaximumLength(100).WithMessage("Barcode must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Barcode));

        RuleFor(x => x.SorterNotes)
            .MaximumLength(2000).WithMessage("SorterNotes must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SorterNotes));

        RuleFor(x => x.TargetGender)
            .IsInEnum().WithMessage("TargetGender is invalid.");

        RuleFor(x => x.AgeGroup)
            .IsInEnum().WithMessage("AgeGroup is invalid.");

        RuleFor(x => x.Season)
            .IsInEnum().WithMessage("Season is invalid.");

        RuleFor(x => x.Condition)
            .IsInEnum().WithMessage("Condition is invalid.");

        RuleFor(x => x.AvailabilityStatus)
            .IsInEnum().WithMessage("AvailabilityStatus is invalid.")
            .When(x => x.AvailabilityStatus.HasValue);
    }
}
