using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.City.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Cities.Commands.CreateCity;

public sealed class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, CityResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateCityCommandHandler> _logger;

    public CreateCityCommandHandler(IAppDbContext context, ILogger<CreateCityCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CityResponse> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim();
        var name = request.Name.Trim();

        var nameExists = await _context.Cities
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);

        if (nameExists)
        {
            throw new ConflictException("A city with this name already exists.");
        }

        var codeExists = await _context.Cities
            .AnyAsync(c => c.Code.ToLower() == normalizedCode.ToLower(), cancellationToken);

        if (codeExists)
        {
            throw new ConflictException("A city with this code already exists.");
        }

        var city = new City
        {
            Name = name,
            Code = normalizedCode
        };

        _context.Cities.Add(city);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("City created with Id {CityId}", city.Id);

        return new CityResponse
        {
            Id = city.Id,
            Name = city.Name,
            Code = city.Code
        };
    }
}
