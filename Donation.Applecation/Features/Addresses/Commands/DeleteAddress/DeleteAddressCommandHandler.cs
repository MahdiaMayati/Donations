using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Addresses.Commands.DeleteAddress;

public sealed class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAddressDependencyChecker _dependencyChecker;
    private readonly ILogger<DeleteAddressCommandHandler> _logger;

    public DeleteAddressCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        IAddressDependencyChecker dependencyChecker,
        ILogger<DeleteAddressCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _dependencyChecker = dependencyChecker;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (address is null)
        {
            return false;
        }

        EnsureCanManage(address);

        var (hasDependencies, message) = await _dependencyChecker.CheckAsync(request.Id, cancellationToken);
        if (hasDependencies)
        {
            throw new BusinessRuleException(
                string.IsNullOrWhiteSpace(message)
                    ? "Cannot delete this address because it is linked to active operations."
                    : message);
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Address deleted with Id {AddressId}", request.Id);

        return true;
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
