using FluentValidation;

namespace Donation.Application.Features.Donors.Commands.UpdateDonor;

public sealed class UpdateDonorCommandValidator : AbstractValidator<UpdateDonorCommand>
{
    private static readonly string[] AllowedContactMethods = ["WhatsApp", "Call", "SMS"];

    public UpdateDonorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");

        RuleFor(x => x.FullName)
            .MaximumLength(200)
            .When(x => x.FullName is not null);

        RuleFor(x => x.FullName)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("FullName cannot be empty.")
            .When(x => x.FullName is not null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is invalid.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .When(x => x.PhoneNumber is not null);

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));

        RuleFor(x => x.PreferredContactMethod)
            .Must(v => AllowedContactMethods.Contains(v!.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("PreferredContactMethod must be WhatsApp, Call, or SMS.")
            .When(x => !string.IsNullOrWhiteSpace(x.PreferredContactMethod));

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.AreaId)
                .GreaterThan(0).WithMessage("Address.AreaId must be greater than zero.");

            RuleFor(x => x.Address!.Street)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Address.Street is required.")
                .MaximumLength(300);

            RuleFor(x => x.Address!.Details)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Address.Details is required.")
                .MaximumLength(500);

            RuleFor(x => x.Address!.Latitude)
                .InclusiveBetween(-90, 90);

            RuleFor(x => x.Address!.Longitude)
                .InclusiveBetween(-180, 180);
        });
    }
}
