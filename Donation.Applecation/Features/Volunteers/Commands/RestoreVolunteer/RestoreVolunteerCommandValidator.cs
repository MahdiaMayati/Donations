using FluentValidation;

namespace Donation.Application.Features.Volunteers.Commands.RestoreVolunteer;

public sealed class RestoreVolunteerCommandValidator : AbstractValidator<RestoreVolunteerCommand>
{
    public RestoreVolunteerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
