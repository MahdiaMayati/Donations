using FluentValidation;

namespace Donation.Application.Features.Volunteers.Commands.UpdateVolunteer;

public sealed class UpdateVolunteerCommandValidator : AbstractValidator<UpdateVolunteerCommand>
{
    public UpdateVolunteerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status is invalid.");
    }
}
