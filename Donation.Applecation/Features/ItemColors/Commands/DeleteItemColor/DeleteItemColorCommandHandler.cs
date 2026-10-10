using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemColors.Commands.DeleteItemColor;

public sealed class DeleteItemColorCommandHandler : IRequestHandler<DeleteItemColorCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteItemColorCommandHandler> _logger;

    public DeleteItemColorCommandHandler(
        IAppDbContext context,
        ILogger<DeleteItemColorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteItemColorCommand request, CancellationToken cancellationToken)
    {
        var link = await _context.ItemColors
            .FirstOrDefaultAsync(
                ic => ic.ItemId == request.ItemId && ic.ColorId == request.ColorId,
                cancellationToken);

        if (link is null)
        {
            return false;
        }

        _context.ItemColors.Remove(link);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "ItemColor removed ItemId {ItemId} ColorId {ColorId}",
            request.ItemId,
            request.ColorId);
        return true;
    }
}
