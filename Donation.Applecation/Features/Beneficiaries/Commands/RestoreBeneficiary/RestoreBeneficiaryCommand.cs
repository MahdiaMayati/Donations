using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;

namespace Donation.Application.Features.Beneficiaries.Commands.RestoreBeneficiary;

public sealed record RestoreBeneficiaryCommand(Guid Id) : IRequest<BeneficiaryResponse?>;
