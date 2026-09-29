using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Area.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Areas.Commands.UpdateArea;

public sealed class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, AreaResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateAreaCommandHandler> _logger;

    public UpdateAreaCommandHandler(IAppDbContext context, ILogger<UpdateAreaCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AreaResponse?> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.Areas
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (area is null)
        {
            return null;
        }

        var cityExists = await _context.Cities
            .AnyAsync(c => c.Id == request.CityId, cancellationToken);

        if (!cityExists)
        {
            throw new NotFoundException("City not found.");
        }

        var name = request.Name.Trim();

        var duplicate = await _context.Areas.AnyAsync(
            a => a.Id != request.Id
                 && a.CityId == request.CityId
                 && a.Name.ToLower() == name.ToLower(),
            cancellationToken);

        if (duplicate)
        {
            throw new ConflictException("An area with this name already exists in the selected city.");
        }

        area.CityId = request.CityId;
        area.Name = name;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Area updated with Id {AreaId}", area.Id);

        return new AreaResponse
        {
            Id = area.Id,
            CityId = area.CityId,
            Name = area.Name
        };
    }
}
