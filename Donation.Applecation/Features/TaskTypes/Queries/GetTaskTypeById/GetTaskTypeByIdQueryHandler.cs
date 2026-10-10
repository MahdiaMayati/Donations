using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.TaskType.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.TaskTypes.Queries.GetTaskTypeById;

public sealed class GetTaskTypeByIdQueryHandler
    : IRequestHandler<GetTaskTypeByIdQuery, TaskTypeResponse?>
{
    private readonly IAppDbContext _context;

    public GetTaskTypeByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskTypeResponse?> Handle(
        GetTaskTypeByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.TaskTypes
            .AsNoTracking()
            .Where(t => t.Id == request.Id)
            .Select(t => new TaskTypeResponse
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
