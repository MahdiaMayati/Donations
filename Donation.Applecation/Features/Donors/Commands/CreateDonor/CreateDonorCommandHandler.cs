using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Donors.Commands.CreateDonor;

public sealed class CreateDonorCommandHandler : IRequestHandler<CreateDonorCommand, DonorResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateDonorCommandHandler> _logger;

    public CreateDonorCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateDonorCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<DonorResponse> Handle(CreateDonorCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var userId = _currentUser.UserId.Value;

        var exists = await _context.Donors
            .AnyAsync(d => d.UserId == userId, cancellationToken);

        if (exists)
        {
            throw new ConflictException("A donor profile already exists for this user.");
        }

        var donor = new Donor { UserId = userId };
        _context.Donors.Add(donor);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Donor created with Id {DonorId}", donor.Id);

        return new DonorResponse
        {
            Id = donor.Id,
            UserId = donor.UserId
        };
    }
}
