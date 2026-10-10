using Donation.Application.Common.Pagination;
using FluentValidation;

namespace Donation.Application.Features.ItemPhotos.Queries.GetAllItemPhotos;

public sealed class GetAllItemPhotosQueryValidator : AbstractValidator<GetAllItemPhotosQuery>
{
    public GetAllItemPhotosQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, PaginationRequest.MaxLimit)
            .WithMessage($"Limit must be between 1 and {PaginationRequest.MaxLimit}.");
    }
}
