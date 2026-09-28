using FluentValidation;

namespace Donation.Application.Features.Volunteers.Commands.CreateVolunteer;

public sealed class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
    }
}
