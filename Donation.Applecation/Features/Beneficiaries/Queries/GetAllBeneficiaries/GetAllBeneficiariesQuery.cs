using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;

namespace Donation.Application.Features.Beneficiaries.Queries.GetAllBeneficiaries;

public sealed record GetAllBeneficiariesQuery(
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<BeneficiaryResponse>>;
