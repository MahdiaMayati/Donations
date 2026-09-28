using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Area.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Areas.Commands.CreateArea;

public sealed class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, AreaResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateAreaCommandHandler> _logger;

    public CreateAreaCommandHandler(IAppDbContext context, ILogger<CreateAreaCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AreaResponse> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
    {
        var cityExists = await _context.Cities
            .AnyAsync(c => c.Id == request.CityId, cancellationToken);

        if (!cityExists)
        {
            throw new NotFoundException("City not found.");
        }

        var name = request.Name.Trim();

        var duplicate = await _context.Areas.AnyAsync(
            a => a.CityId == request.CityId && a.Name.ToLower() == name.ToLower(),
            cancellationToken);

        if (duplicate)
        {
            throw new ConflictException("An area with this name already exists in the selected city.");
        }

        var area = new Area
        {
            CityId = request.CityId,
            Name = name
        };

        _context.Areas.Add(area);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Area created with Id {AreaId}", area.Id);

        return new AreaResponse
        {
            Id = area.Id,
            CityId = area.CityId,
            Name = area.Name
        };
    }
}
