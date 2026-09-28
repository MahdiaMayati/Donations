using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Donors.Commands.DeleteDonor;

public sealed class DeleteDonorCommandHandler : IRequestHandler<DeleteDonorCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteDonorCommandHandler> _logger;

    public DeleteDonorCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<DeleteDonorCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteDonorCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var donor = await _context.Donors
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (donor is null)
        {
            return false;
        }

        EnsureCanManage(donor);

        _context.Donors.Remove(donor);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Donor deleted with Id {DonorId}", request.Id);

        return true;
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
