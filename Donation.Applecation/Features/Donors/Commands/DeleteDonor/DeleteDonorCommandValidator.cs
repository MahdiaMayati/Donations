using FluentValidation;

namespace Donation.Application.Features.Donors.Commands.DeleteDonor;

public sealed class DeleteDonorCommandValidator : AbstractValidator<DeleteDonorCommand>
{
    public DeleteDonorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");
    }
}
