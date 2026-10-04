using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Color.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Colors.Queries.GetDeletedColors;

public sealed class GetDeletedColorsQueryHandler
    : IRequestHandler<GetDeletedColorsQuery, PaginatedResult<ColorResponse>>
{
    private readonly IAppDbContext _context;

    public GetDeletedColorsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ColorResponse>> Handle(
        GetDeletedColorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Colors
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(c => c.IsDeleted);

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
