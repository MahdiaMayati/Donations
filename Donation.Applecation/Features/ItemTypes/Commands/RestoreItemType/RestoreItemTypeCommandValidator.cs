using FluentValidation;

namespace Donation.Application.Features.ItemTypes.Commands.RestoreItemType;

public sealed class RestoreItemTypeCommandValidator : AbstractValidator<RestoreItemTypeCommand>
{
    public RestoreItemTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
