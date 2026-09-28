using FluentValidation;

namespace Donation.Application.Features.Addresses.Commands.UpdateAddress;

public sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");

        RuleFor(x => x.AreaId)
            .GreaterThan(0).WithMessage("AreaId must be greater than zero.");

        RuleFor(x => x.Street)
            .Cascade(CascadeMode.Stop)
            .Must(street => !string.IsNullOrWhiteSpace(street)).WithMessage("Street is required.")
            .MaximumLength(300).WithMessage("Street must not exceed 300 characters.");

        RuleFor(x => x.Details)
            .Cascade(CascadeMode.Stop)
            .Must(details => !string.IsNullOrWhiteSpace(details)).WithMessage("Details is required.")
            .MaximumLength(500).WithMessage("Details must not exceed 500 characters.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
    }
}
