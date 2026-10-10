using FluentValidation;

namespace Donation.Application.Features.Colors.Commands.DeleteColor;

public sealed class DeleteColorCommandValidator : AbstractValidator<DeleteColorCommand>
{
    public DeleteColorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
