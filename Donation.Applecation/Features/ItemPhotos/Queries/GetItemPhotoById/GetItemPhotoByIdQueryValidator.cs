using FluentValidation;

namespace Donation.Application.Features.ItemPhotos.Queries.GetItemPhotoById;

public sealed class GetItemPhotoByIdQueryValidator : AbstractValidator<GetItemPhotoByIdQuery>
{
    public GetItemPhotoByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
