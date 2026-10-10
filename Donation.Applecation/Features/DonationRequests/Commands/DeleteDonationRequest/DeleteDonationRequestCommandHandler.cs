using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.DonationRequests.Commands.DeleteDonationRequest;

public sealed class DeleteDonationRequestCommandHandler
    : IRequestHandler<DeleteDonationRequestCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteDonationRequestCommandHandler> _logger;

    public DeleteDonationRequestCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<DeleteDonationRequestCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteDonationRequestCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var entity = await _context.DonationRequests
            .Include(d => d.Donor)
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        if (!_currentUser.IsAdmin && entity.Donor.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only cancel your own donation requests.");
        }

        if (entity.Status != DonationRequestStatus.Submitted)
        {
            throw new BusinessRuleException(
                "Donation requests can only be cancelled while status is Submitted.");
        }

        var now = DateTime.UtcNow;
        entity.IsDeleted = true;
        entity.DeletedAt = now;
        entity.UpdatedAt = now;

        foreach (var item in entity.Items.Where(i => !i.IsDeleted))
        {
            item.IsDeleted = true;
            item.DeletedAt = now;
            item.UpdatedAt = now;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("DonationRequest {DonationRequestId} soft-deleted", entity.Id);
        return true;
    }
}
