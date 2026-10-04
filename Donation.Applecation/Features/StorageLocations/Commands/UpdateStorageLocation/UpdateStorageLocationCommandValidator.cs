using FluentValidation;

namespace Donation.Application.Features.StorageLocations.Commands.UpdateStorageLocation;

public sealed class UpdateStorageLocationCommandValidator : AbstractValidator<UpdateStorageLocationCommand>
{
    public UpdateStorageLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");

        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .Must(code => !string.IsNullOrWhiteSpace(code)).WithMessage("Code is required.")
            .MaximumLength(100).WithMessage("Code must not exceed 100 characters.");
    }
}
