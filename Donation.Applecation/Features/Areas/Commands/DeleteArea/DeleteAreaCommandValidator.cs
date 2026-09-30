using FluentValidation;

namespace Donation.Application.Features.Areas.Commands.DeleteArea;

public sealed class DeleteAreaCommandValidator : AbstractValidator<DeleteAreaCommand>
{
    public DeleteAreaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
