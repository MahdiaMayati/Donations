using MediatR;

namespace Donation.Application.Features.TaskTypes.Commands.DeleteTaskType;

public sealed record DeleteTaskTypeCommand(Guid Id) : IRequest<bool>;
