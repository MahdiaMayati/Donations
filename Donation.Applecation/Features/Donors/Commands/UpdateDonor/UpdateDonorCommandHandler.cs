using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Donors.Commands.UpdateDonor;

public sealed class UpdateDonorCommandHandler : IRequestHandler<UpdateDonorCommand, DonorResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateDonorCommandHandler> _logger;

    public UpdateDonorCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateDonorCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<DonorResponse?> Handle(UpdateDonorCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var donor = await _context.Donors
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (donor is null)
        {
            return null;
        }

        EnsureCanManage(donor);

        // Donor has no mutable fields beyond UserId; return current state (idempotent).
        _logger.LogInformation("Donor update (no-op) for Id {DonorId}", donor.Id);

        return new DonorResponse
        {
            Id = donor.Id,
            UserId = donor.UserId
        };
    }

    private void EnsureCanManage(Donor donor)
    {
        if (_currentUser.IsAdmin)
        {
            return;
        }

        if (donor.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only manage your own donor profile.");
        }
    }
}
