using FluentValidation;

namespace Donation.Application.Features.StorageLocations.Commands.DeleteStorageLocation;

public sealed class DeleteStorageLocationCommandValidator : AbstractValidator<DeleteStorageLocationCommand>
{
    public DeleteStorageLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
