using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.DonationRequest.Response;
using Donation.Application.Features.DonationRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.DonationRequests.Commands.UpdateDonationRequestStatus;

public sealed class UpdateDonationRequestStatusCommandHandler
    : IRequestHandler<UpdateDonationRequestStatusCommand, DonationRequestResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateDonationRequestStatusCommandHandler> _logger;

    public UpdateDonationRequestStatusCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateDonationRequestStatusCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<DonationRequestResponse?> Handle(
        UpdateDonationRequestStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var entity = await _context.DonationRequests
            .Include(d => d.Photos)
            .Include(d => d.Items)
                .ThenInclude(i => i.Photos)
            .Include(d => d.Items)
                .ThenInclude(i => i.ItemColors)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin)
        {
            var userOrgId = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == _currentUser.UserId.Value)
                .Select(u => u.OrganizationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!userOrgId.HasValue || userOrgId.Value != entity.OrganizationId)
            {
                throw new ForbiddenException(
                    "Only Admin or organization staff can change donation request status.");
            }
        }

        if (!DonationRequestStatusTransitions.CanTransition(
                entity.Status, request.Status, entity.DeliveryMethod))
        {
            var allowed = DonationRequestStatusTransitions.GetAllowedNext(
                entity.Status, entity.DeliveryMethod);
            throw new BusinessRuleException(
                $"Invalid status transition from '{entity.Status}' to '{request.Status}'. " +
                $"Allowed: {string.Join(", ", allowed)}.");
        }

        entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "DonationRequest {DonationRequestId} status changed to {Status}",
            entity.Id,
            entity.Status);

        return DonationRequestMapper.Map(entity);
    }
}
