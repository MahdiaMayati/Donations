using FluentValidation;

namespace Donation.Application.Features.Materials.Commands.RestoreMaterialsBulk;

public sealed class RestoreMaterialsBulkCommandValidator : AbstractValidator<RestoreMaterialsBulkCommand>
{
    public RestoreMaterialsBulkCommandValidator()
    {
        RuleFor(x => x.Ids).NotEmpty().WithMessage("At least one id is required.");
    }
}
