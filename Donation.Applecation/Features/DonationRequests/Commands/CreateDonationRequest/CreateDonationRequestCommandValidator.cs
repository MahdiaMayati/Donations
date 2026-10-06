using Donation.Domain.Enums;
using FluentValidation;

namespace Donation.Application.Features.DonationRequests.Commands.CreateDonationRequest;

public sealed class CreateDonationRequestCommandValidator : AbstractValidator<CreateDonationRequestCommand>
{
    public CreateDonationRequestCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();

        RuleFor(x => x.DeliveryMethod)
            .IsInEnum().WithMessage("DeliveryMethod is invalid.");

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .Must(d => !string.IsNullOrWhiteSpace(d)).WithMessage("Description is required.")
            .MaximumLength(2000);

        RuleFor(x => x.EstimatedItemCount)
            .GreaterThan(0).WithMessage("EstimatedItemCount must be greater than 0.");

        RuleFor(x => x.PickupAddressId)
            .NotEmpty()
            .When(x => x.DeliveryMethod == DeliveryMethod.VolunteerPickup)
            .WithMessage("PickupAddressId is required when DeliveryMethod is VolunteerPickup.");

        RuleFor(x => x.PickupAddressId)
            .Empty()
            .When(x => x.DeliveryMethod == DeliveryMethod.SelfDropOff)
            .WithMessage("PickupAddressId must be empty when DeliveryMethod is SelfDropOff.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.RequestPhotoUrls)
            .Cascade(CascadeMode.Stop)
            .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("Request photo URL cannot be empty.")
            .MaximumLength(2048);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemTypeId).NotEmpty();
            item.RuleFor(i => i.TargetGender).IsInEnum();
            item.RuleFor(i => i.AgeGroup).IsInEnum();
            item.RuleFor(i => i.Season).IsInEnum();
            item.RuleFor(i => i.Condition).IsInEnum();

            item.RuleFor(i => i.Barcode)
                .MaximumLength(100)
                .When(i => !string.IsNullOrWhiteSpace(i.Barcode));

            item.RuleFor(i => i.Size)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Size is required.")
                .MaximumLength(50);

            item.RuleFor(i => i.ColorIds)
                .NotEmpty().WithMessage("At least one ColorId is required per item.");

            item.RuleForEach(i => i.ColorIds)
                .NotEmpty().WithMessage("ColorId cannot be empty.");

            item.RuleForEach(i => i.PhotoUrls)
                .Cascade(CascadeMode.Stop)
                .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("Item photo URL cannot be empty.")
                .MaximumLength(2048);
        });
    }
}
