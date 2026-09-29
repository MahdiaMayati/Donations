using FluentValidation;

namespace Donation.Application.DTOs.Auth.Request;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("FirstName is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("LastName is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("OrganizationId is required.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow.Date).WithMessage("DateOfBirth must be in the past.")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Gender)
            .NotNull().WithMessage("Gender is required (true = Male, false = Female).");

        RuleFor(x => x.PreferredContactMethod)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("PreferredContactMethod is required.")
            .MaximumLength(50); // Suggested values: WhatsApp, Call, SMS

        RuleFor(x => x.MaritalStatus)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("MaritalStatus is required.")
            .MaximumLength(100);

        RuleFor(x => x.EducationalStatus)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("EducationalStatus is required.")
            .MaximumLength(100);

        RuleFor(x => x.Job)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Job is required.")
            .MaximumLength(200);

        RuleFor(x => x.HealthStatus)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("HealthStatus is required.")
            .MaximumLength(200);
    }
}
