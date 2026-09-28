using FluentValidation;

namespace Donation.Application.Features.Donors.Commands.UpdateDonor;

public sealed class UpdateDonorCommandValidator : AbstractValidator<UpdateDonorCommand>
{
    public UpdateDonorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");
    }
}
