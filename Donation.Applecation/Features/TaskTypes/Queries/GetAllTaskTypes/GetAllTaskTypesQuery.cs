using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.TaskType.Response;
using MediatR;

namespace Donation.Application.Features.TaskTypes.Queries.GetAllTaskTypes;

public sealed record GetAllTaskTypesQuery(
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<TaskTypeResponse>>;
