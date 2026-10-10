using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Color.Response;
using Donation.Application.Features.Colors.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Colors.Commands.CreateColor;

public sealed class CreateColorCommandHandler : IRequestHandler<CreateColorCommand, ColorResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateColorCommandHandler> _logger;

    public CreateColorCommandHandler(IAppDbContext context, ILogger<CreateColorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ColorResponse> Handle(CreateColorCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        var code = request.Code.Trim();

        var nameExists = await _context.Colors
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
        if (nameExists)
        {
            throw new ConflictException("A color with this name already exists.");
        }

        var codeExists = await _context.Colors
            .AnyAsync(c => c.Code.ToLower() == code.ToLower(), cancellationToken);
        if (codeExists)
        {
            throw new ConflictException("A color with this code already exists.");
        }

        var color = new Color
        {
            Name = name,
            Code = code,
            IsDeleted = false
        };

        _context.Colors.Add(color);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Color created with Id {ColorId}", color.Id);
        return ColorMapper.Map(color);
    }
}
