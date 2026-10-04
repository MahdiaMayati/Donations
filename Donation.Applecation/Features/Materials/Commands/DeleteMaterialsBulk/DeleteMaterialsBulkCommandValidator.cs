using FluentValidation;

namespace Donation.Application.Features.Materials.Commands.DeleteMaterialsBulk;

public sealed class DeleteMaterialsBulkCommandValidator : AbstractValidator<DeleteMaterialsBulkCommand>
{
    public DeleteMaterialsBulkCommandValidator()
    {
        RuleFor(x => x.Ids).NotEmpty().WithMessage("At least one id is required.");
    }
}
