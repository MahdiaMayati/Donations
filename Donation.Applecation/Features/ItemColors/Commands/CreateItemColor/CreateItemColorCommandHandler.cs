using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemColor.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemColors.Commands.CreateItemColor;

public sealed class CreateItemColorCommandHandler
    : IRequestHandler<CreateItemColorCommand, ItemColorResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateItemColorCommandHandler> _logger;

    public CreateItemColorCommandHandler(
        IAppDbContext context,
        ILogger<CreateItemColorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemColorResponse> Handle(
        CreateItemColorCommand request,
        CancellationToken cancellationToken)
    {
        var itemExists = await _context.Items
            .AsNoTracking()
            .AnyAsync(i => i.Id == request.ItemId, cancellationToken);
        if (!itemExists)
        {
            throw new NotFoundException("Item not found.");
        }

        var color = await _context.Colors
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.ColorId, cancellationToken);
        if (color is null)
        {
            throw new NotFoundException("Color not found.");
        }

        var alreadyLinked = await _context.ItemColors
            .AsNoTracking()
            .AnyAsync(
                ic => ic.ItemId == request.ItemId && ic.ColorId == request.ColorId,
                cancellationToken);
        if (alreadyLinked)
        {
            throw new ConflictException("This color is already linked to the item.");
        }

        var link = new ItemColor
        {
            ItemId = request.ItemId,
            ColorId = request.ColorId
        };

        _context.ItemColors.Add(link);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "ItemColor linked ItemId {ItemId} ColorId {ColorId}",
            request.ItemId,
            request.ColorId);

        return new ItemColorResponse
        {
            ItemId = link.ItemId,
            ColorId = link.ColorId,
            ColorName = color.Name,
            ColorCode = color.Code
        };
    }
}
