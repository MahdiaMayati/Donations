using FluentValidation;

namespace Donation.Application.Features.StorageLocations.Queries.GetStorageLocationById;

public sealed class GetStorageLocationByIdQueryValidator : AbstractValidator<GetStorageLocationByIdQuery>
{
    public GetStorageLocationByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
