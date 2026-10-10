using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.TaskType.Response;
using Donation.Application.Features.TaskTypes.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.TaskTypes.Commands.CreateTaskType;

public sealed class CreateTaskTypeCommandHandler : IRequestHandler<CreateTaskTypeCommand, TaskTypeResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateTaskTypeCommandHandler> _logger;

    public CreateTaskTypeCommandHandler(
        IAppDbContext context,
        ILogger<CreateTaskTypeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaskTypeResponse> Handle(
        CreateTaskTypeCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();
        var name = request.Name.Trim();

        var codeExists = await _context.TaskTypes
            .AnyAsync(t => t.Code.ToLower() == code.ToLower(), cancellationToken);
        if (codeExists)
        {
            throw new ConflictException("A task type with this code already exists.");
        }

        var nameExists = await _context.TaskTypes
            .AnyAsync(t => t.Name.ToLower() == name.ToLower(), cancellationToken);
        if (nameExists)
        {
            throw new ConflictException("A task type with this name already exists.");
        }

        var entity = new TaskType
        {
            Code = code,
            Name = name
        };

        _context.TaskTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("TaskType created with Id {TaskTypeId}", entity.Id);
        return TaskTypeMapper.Map(entity);
    }
}
