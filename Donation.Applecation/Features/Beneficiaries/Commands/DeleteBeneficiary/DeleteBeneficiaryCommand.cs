using MediatR;

namespace Donation.Application.Features.Beneficiaries.Commands.DeleteBeneficiary;

public sealed record DeleteBeneficiaryCommand(Guid Id) : IRequest<bool>;
