using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemColor.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemColors.Queries.GetAllItemColors;

public sealed class GetAllItemColorsQueryHandler
    : IRequestHandler<GetAllItemColorsQuery, PaginatedResult<ItemColorResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllItemColorsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ItemColorResponse>> Handle(
        GetAllItemColorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ItemColors.AsNoTracking();

        if (request.ItemId is Guid itemId && itemId != Guid.Empty)
        {
            query = query.Where(ic => ic.ItemId == itemId);
        }

        return await query
            .OrderBy(ic => ic.ItemId)
            .ThenBy(ic => ic.ColorId)
            .Select(ic => new ItemColorResponse
            {
                ItemId = ic.ItemId,
                ColorId = ic.ColorId,
                ColorName = ic.Color.Name,
                ColorCode = ic.Color.Code
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
