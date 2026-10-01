using FluentValidation;

namespace Donation.Application.Features.Warehouses.Queries.GetWarehouseById;

public sealed class GetWarehouseByIdQueryValidator : AbstractValidator<GetWarehouseByIdQuery>
{
    public GetWarehouseByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
