using FluentValidation;

namespace Donation.Application.Features.ItemTypes.Commands.DeleteItemType;

public sealed class DeleteItemTypeCommandValidator : AbstractValidator<DeleteItemTypeCommand>
{
    public DeleteItemTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
