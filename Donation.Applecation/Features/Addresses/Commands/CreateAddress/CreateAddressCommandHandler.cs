using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Address.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Addresses.Commands.CreateAddress;

public sealed class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateAddressCommandHandler> _logger;

    public CreateAddressCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateAddressCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<AddressResponse> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var areaExists = await _context.Areas
            .AnyAsync(a => a.Id == request.AreaId, cancellationToken);

        if (!areaExists)
        {
            throw new NotFoundException("Area not found.");
        }

        var address = new Address
        {
            AreaId = request.AreaId,
            UserId = _currentUser.UserId.Value,
            Street = request.Street.Trim(),
            Details = request.Details.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = DateTime.UtcNow
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Address created with Id {AddressId}", address.Id);

        return Map(address);
    }

    private static AddressResponse Map(Address address) => new()
    {
        Id = address.Id,
        AreaId = address.AreaId,
        UserId = address.UserId,
        Street = address.Street,
        Details = address.Details,
        Latitude = address.Latitude,
        Longitude = address.Longitude,
        CreatedAt = address.CreatedAt
    };
}
