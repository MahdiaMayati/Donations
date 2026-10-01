using FluentValidation;

namespace Donation.Application.Features.StorageLocations.Commands.CreateStorageLocation;

public sealed class CreateStorageLocationCommandValidator : AbstractValidator<CreateStorageLocationCommand>
{
    public CreateStorageLocationCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");

        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .Must(code => !string.IsNullOrWhiteSpace(code)).WithMessage("Code is required.")
            .MaximumLength(100).WithMessage("Code must not exceed 100 characters.");
    }
}
