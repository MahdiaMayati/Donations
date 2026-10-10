using FluentValidation;

namespace Donation.Application.Features.Volunteers.Commands.CreateVolunteer;

public sealed class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("OrganizationId is required.");

        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrWhiteSpace(s) || VolunteerMapping.AllowedStatuses.Contains(s.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Status must be one of: {string.Join(", ", VolunteerMapping.AllowedStatuses)}.");

        RuleFor(x => x.Days)
            .NotEmpty().WithMessage("Days is required.")
            .MaximumLength(500).WithMessage("Days must not exceed 500 characters.");

        RuleFor(x => x.HoursCount)
            .GreaterThanOrEqualTo(0).WithMessage("HoursCount must be zero or greater.");

        RuleFor(x => x.Hobbies)
            .MaximumLength(500).WithMessage("Hobbies must not exceed 500 characters.")
            .When(x => x.Hobbies is not null);

        RuleFor(x => x.Skills)
            .MaximumLength(500).WithMessage("Skills must not exceed 500 characters.")
            .When(x => x.Skills is not null);

        RuleFor(x => x.Experiences)
            .MaximumLength(1000).WithMessage("Experiences must not exceed 1000 characters.")
            .When(x => x.Experiences is not null);

        RuleFor(x => x.NeglectedTasksCount)
            .GreaterThanOrEqualTo(0).WithMessage("NeglectedTasksCount must be zero or greater.");

        RuleFor(x => x.Address).NotNull().WithMessage("Address is required.");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address.AreaId)
                .NotEmpty().WithMessage("Address.AreaId is required.");

            RuleFor(x => x.Address.Street)
                .NotEmpty().WithMessage("Address.Street is required.")
                .MaximumLength(300);

            RuleFor(x => x.Address.Details)
                .NotEmpty().WithMessage("Address.Details is required.")
                .MaximumLength(500);

            RuleFor(x => x.Address.Latitude)
                .InclusiveBetween(-90, 90);

            RuleFor(x => x.Address.Longitude)
                .InclusiveBetween(-180, 180);
        });
    }
}
