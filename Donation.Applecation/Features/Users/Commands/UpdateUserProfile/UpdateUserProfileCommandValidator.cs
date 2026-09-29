using FluentValidation;

namespace Donation.Application.Features.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("FirstName is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("LastName is required.")
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow.Date).WithMessage("DateOfBirth must be in the past.")
            .When(x => x.DateOfBirth.HasValue);

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
