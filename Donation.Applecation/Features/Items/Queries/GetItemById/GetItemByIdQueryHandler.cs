using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.Item.Response;
using Donation.Application.Features.Items.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Items.Queries.GetItemById;

public sealed class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemResponse?>
{
    private readonly IAppDbContext _context;

    public GetItemByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ItemResponse?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.Items
            .AsNoTracking()
            .Include(i => i.Photos)
            .Include(i => i.ItemColors)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        return item is null ? null : ItemMapper.Map(item);
    }
}
