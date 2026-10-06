using FluentValidation;

namespace Donation.Application.Features.ItemPhotos.Commands.UpdateItemPhoto;

public sealed class UpdateItemPhotoCommandValidator : AbstractValidator<UpdateItemPhotoCommand>
{
    public UpdateItemPhotoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Url)
            .Cascade(CascadeMode.Stop)
            .Must(url => !string.IsNullOrWhiteSpace(url)).WithMessage("Url is required.")
            .MaximumLength(2048).WithMessage("Url must not exceed 2048 characters.");
    }
}
