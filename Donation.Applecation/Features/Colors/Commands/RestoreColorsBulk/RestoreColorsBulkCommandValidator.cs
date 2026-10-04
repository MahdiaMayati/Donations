using FluentValidation;

namespace Donation.Application.Features.Colors.Commands.RestoreColorsBulk;

public sealed class RestoreColorsBulkCommandValidator : AbstractValidator<RestoreColorsBulkCommand>
{
    public RestoreColorsBulkCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("At least one id is required.");
    }
}
