using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Color.Response;
using Donation.Application.Features.Colors.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Colors.Commands.UpdateColor;

public sealed class UpdateColorCommandHandler : IRequestHandler<UpdateColorCommand, ColorResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateColorCommandHandler> _logger;

    public UpdateColorCommandHandler(IAppDbContext context, ILogger<UpdateColorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ColorResponse?> Handle(UpdateColorCommand request, CancellationToken cancellationToken)
    {
        var color = await _context.Colors
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (color is null)
        {
            return null;
        }

        var name = request.Name.Trim();
        var code = request.Code.Trim();

        var nameExists = await _context.Colors
            .AnyAsync(c => c.Id != request.Id && c.Name.ToLower() == name.ToLower(), cancellationToken);
        if (nameExists)
        {
            throw new ConflictException("A color with this name already exists.");
        }

        var codeExists = await _context.Colors
            .AnyAsync(c => c.Id != request.Id && c.Code.ToLower() == code.ToLower(), cancellationToken);
        if (codeExists)
        {
            throw new ConflictException("A color with this code already exists.");
        }

        color.Name = name;
        color.Code = code;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Color updated with Id {ColorId}", color.Id);
        return ColorMapper.Map(color);
    }
}
