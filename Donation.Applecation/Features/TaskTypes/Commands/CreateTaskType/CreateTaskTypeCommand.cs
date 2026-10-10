using Donation.Application.DTOs.TaskType.Response;
using MediatR;

namespace Donation.Application.Features.TaskTypes.Commands.CreateTaskType;

public sealed record CreateTaskTypeCommand(string Code, string Name) : IRequest<TaskTypeResponse>;
