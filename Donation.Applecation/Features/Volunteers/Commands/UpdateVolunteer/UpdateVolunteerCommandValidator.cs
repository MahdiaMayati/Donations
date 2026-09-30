using FluentValidation;

namespace Donation.Application.Features.Volunteers.Commands.UpdateVolunteer;

public sealed class UpdateVolunteerCommandValidator : AbstractValidator<UpdateVolunteerCommand>
{
    public UpdateVolunteerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status is invalid.");
    }
}
