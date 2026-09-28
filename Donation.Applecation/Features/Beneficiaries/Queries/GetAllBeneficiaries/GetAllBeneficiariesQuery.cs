using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;

namespace Donation.Application.Features.Beneficiaries.Queries.GetAllBeneficiaries;

public sealed record GetAllBeneficiariesQuery() : IRequest<IReadOnlyList<BeneficiaryResponse>>;
