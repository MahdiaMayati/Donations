using Donation.Application.DTOs.TaskType.Response;
using MediatR;

namespace Donation.Application.Features.TaskTypes.Queries.GetTaskTypeById;

public sealed record GetTaskTypeByIdQuery(Guid Id) : IRequest<TaskTypeResponse?>;
