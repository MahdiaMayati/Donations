using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.TaskType.Response;
using Donation.Application.Features.TaskTypes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.TaskTypes.Commands.UpdateTaskType;

public sealed class UpdateTaskTypeCommandHandler
    : IRequestHandler<UpdateTaskTypeCommand, TaskTypeResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateTaskTypeCommandHandler> _logger;

    public UpdateTaskTypeCommandHandler(
        IAppDbContext context,
        ILogger<UpdateTaskTypeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaskTypeResponse?> Handle(
        UpdateTaskTypeCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.TaskTypes
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var code = request.Code.Trim();
        var name = request.Name.Trim();

        var codeExists = await _context.TaskTypes
            .AnyAsync(t => t.Id != request.Id && t.Code.ToLower() == code.ToLower(), cancellationToken);
        if (codeExists)
        {
            throw new ConflictException("A task type with this code already exists.");
        }

        var nameExists = await _context.TaskTypes
            .AnyAsync(t => t.Id != request.Id && t.Name.ToLower() == name.ToLower(), cancellationToken);
        if (nameExists)
        {
            throw new ConflictException("A task type with this name already exists.");
        }

        entity.Code = code;
        entity.Name = name;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("TaskType updated with Id {TaskTypeId}", entity.Id);
        return TaskTypeMapper.Map(entity);
    }
}
