using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.DonationRequest.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.DonationRequests.Queries.GetAllDonationRequests;

public sealed record GetAllDonationRequestsQuery(
    Guid OrganizationId,
    Guid? DonorId,
    DonationRequestStatus? Status,
    DeliveryMethod? DeliveryMethod,
    int Page,
    int Limit,
    string? Search) : IRequest<PaginatedResult<DonationRequestResponse>>;
