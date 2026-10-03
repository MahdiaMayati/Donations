using FluentValidation;

namespace Donation.Application.Features.Colors.Commands.RestoreColor;

public sealed class RestoreColorCommandValidator : AbstractValidator<RestoreColorCommand>
{
    public RestoreColorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
