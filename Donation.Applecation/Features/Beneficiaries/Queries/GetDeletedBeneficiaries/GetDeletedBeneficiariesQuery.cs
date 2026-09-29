using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;

namespace Donation.Application.Features.Beneficiaries.Queries.GetDeletedBeneficiaries;

public sealed record GetDeletedBeneficiariesQuery() : IRequest<IReadOnlyList<BeneficiaryResponse>>;
