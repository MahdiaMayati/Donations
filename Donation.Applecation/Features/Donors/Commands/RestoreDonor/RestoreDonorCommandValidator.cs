using FluentValidation;

namespace Donation.Application.Features.Donors.Commands.RestoreDonor;

public sealed class RestoreDonorCommandValidator : AbstractValidator<RestoreDonorCommand>
{
    public RestoreDonorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
