using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.TaskType.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.TaskTypes.Queries.GetAllTaskTypes;

public sealed class GetAllTaskTypesQueryHandler
    : IRequestHandler<GetAllTaskTypesQuery, PaginatedResult<TaskTypeResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllTaskTypesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<TaskTypeResponse>> Handle(
        GetAllTaskTypesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.TaskTypes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(t =>
                t.Code.ToLower().Contains(term) ||
                t.Name.ToLower().Contains(term));
        }

        return await query
            .OrderBy(t => t.Name)
            .Select(t => new TaskTypeResponse
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
