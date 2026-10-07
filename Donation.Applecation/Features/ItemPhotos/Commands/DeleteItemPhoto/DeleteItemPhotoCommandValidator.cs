using FluentValidation;

namespace Donation.Application.Features.ItemPhotos.Commands.DeleteItemPhoto;

public sealed class DeleteItemPhotoCommandValidator : AbstractValidator<DeleteItemPhotoCommand>
{
    public DeleteItemPhotoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
