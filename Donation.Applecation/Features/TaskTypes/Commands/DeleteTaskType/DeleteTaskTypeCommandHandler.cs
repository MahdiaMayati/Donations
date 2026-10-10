using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.TaskTypes.Commands.DeleteTaskType;

public sealed class DeleteTaskTypeCommandHandler : IRequestHandler<DeleteTaskTypeCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteTaskTypeCommandHandler> _logger;

    public DeleteTaskTypeCommandHandler(
        IAppDbContext context,
        ILogger<DeleteTaskTypeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteTaskTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TaskTypes
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        _context.TaskTypes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("TaskType deleted with Id {TaskTypeId}", request.Id);
        return true;
    }
}
