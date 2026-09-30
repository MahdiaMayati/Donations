using FluentValidation;

namespace Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;

public sealed class CreateBeneficiaryCommandValidator : AbstractValidator<CreateBeneficiaryCommand>
{
    public CreateBeneficiaryCommandValidator()
    {
        RuleFor(x => x.User).NotNull().WithMessage("User details are required.");
        RuleFor(x => x.City).NotNull().WithMessage("City details are required.");
        RuleFor(x => x.Area).NotNull().WithMessage("Area details are required.");
        RuleFor(x => x.Address).NotNull().WithMessage("Address details are required.");

        When(x => x.User is not null, () =>
        {
            RuleFor(x => x.User.FirstName)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("FirstName is required.")
                .MaximumLength(100);

            RuleFor(x => x.User.LastName)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("LastName is required.")
                .MaximumLength(100);

            RuleFor(x => x.User.PhoneNumber)
                .MaximumLength(30)
                .When(x => !string.IsNullOrWhiteSpace(x.User.PhoneNumber));

            RuleFor(x => x.User.DateOfBirth)
                .LessThan(DateTime.UtcNow.Date).WithMessage("DateOfBirth must be in the past.")
                .When(x => x.User.DateOfBirth.HasValue);

            RuleFor(x => x.User.PreferredContactMethod)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("PreferredContactMethod is required.")
                .MaximumLength(50);

            RuleFor(x => x.User.MaritalStatus)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("MaritalStatus is required.")
                .MaximumLength(100);

            RuleFor(x => x.User.EducationalStatus)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("EducationalStatus is required.")
                .MaximumLength(100);

            RuleFor(x => x.User.Job)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Job is required.")
                .MaximumLength(200);

            RuleFor(x => x.User.HealthStatus)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("HealthStatus is required.")
                .MaximumLength(200);
        });

        When(x => x.City is not null, () =>
        {
            RuleFor(x => x.City.Id)
                .NotEmpty().WithMessage("City.Id is required.");
        });

        When(x => x.Area is not null, () =>
        {
            RuleFor(x => x.Area.Name)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Area.Name is required.")
                .MaximumLength(200);
        });

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address.Street)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Address.Street is required.")
                .MaximumLength(300);

            RuleFor(x => x.Address.Details)
                .Cascade(CascadeMode.Stop)
                .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Address.Details is required.")
                .MaximumLength(500);

            RuleFor(x => x.Address.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Address.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        });

        RuleFor(x => x.IdPhotoUrl)
            .Cascade(CascadeMode.Stop)
            .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("IdPhotoUrl is required.")
            .MaximumLength(1000).WithMessage("IdPhotoUrl must not exceed 1000 characters.");
    }
}
