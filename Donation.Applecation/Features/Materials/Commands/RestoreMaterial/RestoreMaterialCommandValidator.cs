using FluentValidation;

namespace Donation.Application.Features.Materials.Commands.RestoreMaterial;

public sealed class RestoreMaterialCommandValidator : AbstractValidator<RestoreMaterialCommand>
{
    public RestoreMaterialCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
