using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Volunteer.Response;
using MediatR;

namespace Donation.Application.Features.Volunteers.Queries.GetAllVolunteers;

public sealed record GetAllVolunteersQuery(
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<VolunteerResponse>>;
