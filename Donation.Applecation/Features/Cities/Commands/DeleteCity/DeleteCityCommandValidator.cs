using FluentValidation;

namespace Donation.Application.Features.Cities.Commands.DeleteCity;

public sealed class DeleteCityCommandValidator : AbstractValidator<DeleteCityCommand>
{
    public DeleteCityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
