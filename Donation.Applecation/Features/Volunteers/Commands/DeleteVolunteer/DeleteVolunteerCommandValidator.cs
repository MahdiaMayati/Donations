using FluentValidation;

namespace Donation.Application.Features.Volunteers.Commands.DeleteVolunteer;

public sealed class DeleteVolunteerCommandValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");
    }
}
