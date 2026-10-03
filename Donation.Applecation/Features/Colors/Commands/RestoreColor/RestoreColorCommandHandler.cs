using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Color.Response;
using Donation.Application.Features.Colors.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Colors.Commands.RestoreColor;

public sealed class RestoreColorCommandHandler : IRequestHandler<RestoreColorCommand, ColorResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RestoreColorCommandHandler> _logger;

    public RestoreColorCommandHandler(IAppDbContext context, ILogger<RestoreColorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ColorResponse?> Handle(RestoreColorCommand request, CancellationToken cancellationToken)
    {
        var color = await _context.Colors
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsDeleted, cancellationToken);

        if (color is null)
        {
            return null;
        }

        var nameConflict = await _context.Colors
            .AnyAsync(c => c.Name.ToLower() == color.Name.ToLower(), cancellationToken);
        if (nameConflict)
        {
            throw new ConflictException("Cannot restore: an active color with the same name already exists.");
        }

        var codeConflict = await _context.Colors
            .AnyAsync(c => c.Code.ToLower() == color.Code.ToLower(), cancellationToken);
        if (codeConflict)
        {
            throw new ConflictException("Cannot restore: an active color with the same code already exists.");
        }

        color.IsDeleted = false;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Color restored with Id {ColorId}", request.Id);
        return ColorMapper.Map(color);
    }
}
