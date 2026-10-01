using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Queries.GetDeletedDonors;

public sealed record GetDeletedDonorsQuery(
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<DonorResponse>>;
