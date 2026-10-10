using Donation.Application.DTOs.TaskType.Response;
using MediatR;

namespace Donation.Application.Features.TaskTypes.Commands.UpdateTaskType;

public sealed record UpdateTaskTypeCommand(Guid Id, string Code, string Name)
    : IRequest<TaskTypeResponse?>;
