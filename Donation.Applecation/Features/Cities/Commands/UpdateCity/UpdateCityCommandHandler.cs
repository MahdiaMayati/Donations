using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.City.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Cities.Commands.UpdateCity;

public sealed class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, CityResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateCityCommandHandler> _logger;

    public UpdateCityCommandHandler(IAppDbContext context, ILogger<UpdateCityCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CityResponse?> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (city is null)
        {
            return null;
        }

        var normalizedCode = request.Code.Trim();
        var name = request.Name.Trim();

        var nameExists = await _context.Cities
            .AnyAsync(
                c => c.Id != request.Id && c.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (nameExists)
        {
            throw new ConflictException("A city with this name already exists.");
        }

        var codeExists = await _context.Cities
            .AnyAsync(
                c => c.Id != request.Id && c.Code.ToLower() == normalizedCode.ToLower(),
                cancellationToken);

        if (codeExists)
        {
            throw new ConflictException("A city with this code already exists.");
        }

        city.Name = name;
        city.Code = normalizedCode;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("City updated with Id {CityId}", city.Id);

        return new CityResponse
        {
            Id = city.Id,
            Name = city.Name,
            Code = city.Code
        };
    }
}
