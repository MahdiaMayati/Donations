using FluentValidation;

namespace Donation.Application.Features.Donors.Commands.CreateDonor;

public sealed class CreateDonorCommandValidator : AbstractValidator<CreateDonorCommand>
{
    private static readonly string[] AllowedContactMethods = ["WhatsApp", "Call", "SMS"];

    public CreateDonorCommandValidator()
    {
        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("FullName is required.")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("PhoneNumber is required.")
            .MaximumLength(30);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.PreferredContactMethod)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("PreferredContactMethod is required.")
            .Must(v => AllowedContactMethods.Contains(v.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("PreferredContactMethod must be WhatsApp, Call, or SMS.");

        RuleFor(x => x.Address).NotNull().WithMessage("Address is required.");

        RuleFor(x => x.Address.AreaId)
            .GreaterThan(0).WithMessage("Address.AreaId must be greater than zero.")
            .When(x => x.Address is not null);

        RuleFor(x => x.Address.Street)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Address.Street is required.")
            .MaximumLength(300)
            .When(x => x.Address is not null);

        RuleFor(x => x.Address.Details)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Address.Details is required.")
            .MaximumLength(500)
            .When(x => x.Address is not null);

        RuleFor(x => x.Address.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Address.Latitude must be between -90 and 90.")
            .When(x => x.Address is not null);

        RuleFor(x => x.Address.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Address.Longitude must be between -180 and 180.")
            .When(x => x.Address is not null);
    }
}
