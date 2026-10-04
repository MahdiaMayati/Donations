using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Color.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Colors.Queries.GetAllColors;

public sealed class GetAllColorsQueryHandler
    : IRequestHandler<GetAllColorsQuery, PaginatedResult<ColorResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllColorsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ColorResponse>> Handle(
        GetAllColorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Colors.AsNoTracking();

        if (request.Ids is { Count: > 0 })
        {
            var ids = request.Ids.Where(id => id != Guid.Empty).Distinct().ToHashSet();
            query = query.Where(c => ids.Contains(c.Id));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(term) ||
                c.Code.ToLower().Contains(term));
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new ColorResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                IsDeleted = c.IsDeleted
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
