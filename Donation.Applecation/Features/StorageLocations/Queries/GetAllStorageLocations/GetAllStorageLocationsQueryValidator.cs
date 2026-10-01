using FluentValidation;
using Donation.Application.Common.Pagination;

namespace Donation.Application.Features.StorageLocations.Queries.GetAllStorageLocations;

public sealed class GetAllStorageLocationsQueryValidator : AbstractValidator<GetAllStorageLocationsQuery>
{
    public GetAllStorageLocationsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, PaginationRequest.MaxLimit)
            .WithMessage($"Limit must be between 1 and {PaginationRequest.MaxLimit}.");
    }
}
