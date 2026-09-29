using MediatR;

namespace Donation.Application.Features.Beneficiaries.Commands.DeleteBeneficiary;

public sealed record DeleteBeneficiaryCommand(int Id) : IRequest<bool>;
