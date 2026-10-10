using FluentValidation;

namespace Donation.Application.Features.Materials.Commands.DeleteMaterial;

public sealed class DeleteMaterialCommandValidator : AbstractValidator<DeleteMaterialCommand>
{
    public DeleteMaterialCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
