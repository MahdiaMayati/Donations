using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Colors.Commands.DeleteColor;

public sealed class DeleteColorCommandHandler : IRequestHandler<DeleteColorCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteColorCommandHandler> _logger;

    public DeleteColorCommandHandler(IAppDbContext context, ILogger<DeleteColorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteColorCommand request, CancellationToken cancellationToken)
    {
        var color = await _context.Colors
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (color is null)
        {
            return false;
        }

        color.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Color soft-deleted with Id {ColorId}", request.Id);
        return true;
    }
}
