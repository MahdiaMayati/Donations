using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;

namespace Donation.Application.Features.Beneficiaries.Queries.GetBeneficiaryById;

public sealed record GetBeneficiaryByIdQuery(int Id) : IRequest<BeneficiaryResponse?>;
