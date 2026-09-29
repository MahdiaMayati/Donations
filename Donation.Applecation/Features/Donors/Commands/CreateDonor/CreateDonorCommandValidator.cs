using FluentValidation;

namespace Donation.Application.Features.Donors.Commands.CreateDonor;

public sealed class CreateDonorCommandValidator : AbstractValidator<CreateDonorCommand>
{
    public CreateDonorCommandValidator()
    {
    }
}
