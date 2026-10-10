using FluentValidation;

namespace Donation.Application.Features.Warehouses.Commands.DeleteWarehouse;

public sealed class DeleteWarehouseCommandValidator : AbstractValidator<DeleteWarehouseCommand>
{
    public DeleteWarehouseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
