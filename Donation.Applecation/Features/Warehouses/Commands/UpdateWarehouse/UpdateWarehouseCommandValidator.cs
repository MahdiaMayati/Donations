using FluentValidation;

namespace Donation.Application.Features.Warehouses.Commands.UpdateWarehouse;

public sealed class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("OrganizationId is required.");

        RuleFor(x => x.AddressId)
            .NotEmpty().WithMessage("AddressId is required.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}
