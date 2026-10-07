using FluentValidation;

namespace Donation.Application.Features.ItemPhotos.Commands.CreateItemPhoto;

public sealed class CreateItemPhotoCommandValidator : AbstractValidator<CreateItemPhotoCommand>
{
    public CreateItemPhotoCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId is required.");

        RuleFor(x => x.Url)
            .Cascade(CascadeMode.Stop)
            .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("Url is required.")
            .MaximumLength(2048).WithMessage("Url must not exceed 2048 characters.");
    }
}
