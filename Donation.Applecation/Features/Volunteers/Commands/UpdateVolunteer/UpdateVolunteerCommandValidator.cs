using FluentValidation;

namespace Donation.Application.Features.Volunteers.Commands.UpdateVolunteer;

public sealed class UpdateVolunteerCommandValidator : AbstractValidator<UpdateVolunteerCommand>
{
    public UpdateVolunteerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("OrganizationId is required.")
            .When(x => x.OrganizationId.HasValue);

        RuleFor(x => x.Status)
            .Must(s => VolunteerMapping.AllowedStatuses.Contains(s!.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Status must be one of: {string.Join(", ", VolunteerMapping.AllowedStatuses)}.")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));

        RuleFor(x => x.Days)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Days cannot be empty.")
            .MaximumLength(500)
            .When(x => x.Days is not null);

        RuleFor(x => x.HoursCount)
            .GreaterThanOrEqualTo(0).WithMessage("HoursCount must be zero or greater.")
            .When(x => x.HoursCount.HasValue);

        RuleFor(x => x.Hobbies)
            .MaximumLength(500)
            .When(x => x.Hobbies is not null);

        RuleFor(x => x.Skills)
            .MaximumLength(500)
            .When(x => x.Skills is not null);

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.AreaId)
                .NotEmpty().WithMessage("Address.AreaId is required.");

            RuleFor(x => x.Address!.Street)
                .NotEmpty().WithMessage("Address.Street is required.")
                .MaximumLength(300);

            RuleFor(x => x.Address!.Details)
                .NotEmpty().WithMessage("Address.Details is required.")
                .MaximumLength(500);

            RuleFor(x => x.Address!.Latitude)
                .InclusiveBetween(-90, 90);

            RuleFor(x => x.Address!.Longitude)
                .InclusiveBetween(-180, 180);
        });
    }
}
