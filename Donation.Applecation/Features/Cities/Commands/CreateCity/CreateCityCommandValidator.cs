using FluentValidation;

namespace Donation.Application.Features.Cities.Commands.CreateCity;

public sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .Must(code => !string.IsNullOrWhiteSpace(code)).WithMessage("Code is required.")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");
    }
}
