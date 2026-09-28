using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Address.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Addresses.Commands.UpdateAddress;

public sealed class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateAddressCommandHandler> _logger;

    public UpdateAddressCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateAddressCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<AddressResponse?> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (address is null)
        {
            return null;
        }

        EnsureCanManage(address);

        var areaExists = await _context.Areas
            .AnyAsync(a => a.Id == request.AreaId, cancellationToken);

        if (!areaExists)
        {
            throw new NotFoundException("Area not found.");
        }

        address.AreaId = request.AreaId;
        address.Street = request.Street.Trim();
        address.Details = request.Details.Trim();
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Address updated with Id {AddressId}", address.Id);

        return new AddressResponse
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

    private void EnsureCanManage(Address address)
    {
        if (_currentUser.IsAdmin)
        {
            return;
        }

        if (address.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only manage your own addresses.");
        }
    }
}
