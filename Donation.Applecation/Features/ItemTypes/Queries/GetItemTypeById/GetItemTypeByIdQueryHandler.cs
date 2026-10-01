using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.ItemType.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemTypes.Queries.GetItemTypeById;

public sealed class GetItemTypeByIdQueryHandler
    : IRequestHandler<GetItemTypeByIdQuery, ItemTypeResponse?>
{
    private readonly IAppDbContext _context;

    public GetItemTypeByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ItemTypeResponse?> Handle(
        GetItemTypeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.ItemTypes
            .AsNoTracking()
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        return entity is null ? null : ItemTypeMappings.ToResponse(entity);
    }
}
