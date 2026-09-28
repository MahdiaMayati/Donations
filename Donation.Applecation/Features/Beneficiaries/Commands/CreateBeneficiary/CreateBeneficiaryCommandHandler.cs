using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Response;
using Donation.Domain.Entities;
using Donation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;

public sealed class CreateBeneficiaryCommandHandler : IRequestHandler<CreateBeneficiaryCommand, BeneficiaryResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateBeneficiaryCommandHandler> _logger;

    public CreateBeneficiaryCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateBeneficiaryCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<BeneficiaryResponse> Handle(CreateBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var userId = _currentUser.UserId.Value;

        var exists = await _context.Beneficiaries
            .AnyAsync(b => b.UserId == userId && !b.IsDeleted, cancellationToken);

        if (exists)
        {
            throw new ConflictException("A beneficiary profile already exists for this user.");
        }

        var addressExists = await _context.Addresses
            .AnyAsync(a => a.Id == request.AddressId, cancellationToken);

        if (!addressExists)
        {
            throw new NotFoundException("Address not found.");
        }

        var beneficiary = new Beneficiary
        {
            UserId = userId,
            AddressId = request.AddressId,
            IdPhotoUrl = request.IdPhotoUrl.Trim(),
            IsHeadOfHousehold = request.IsHeadOfHousehold,
            VerificationStatus = VerificationStatus.Pending,
            VerifiedUntil = null,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Beneficiaries.Add(beneficiary);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Beneficiary created with Id {BeneficiaryId}", beneficiary.Id);

        return Map(beneficiary);
    }

    private static BeneficiaryResponse Map(Beneficiary b) => new()
    {
        Id = b.Id,
        UserId = b.UserId,
        AddressId = b.AddressId,
        IdPhotoUrl = b.IdPhotoUrl,
        IsHeadOfHousehold = b.IsHeadOfHousehold,
        VerificationStatus = b.VerificationStatus,
        VerifiedUntil = b.VerifiedUntil,
        CreatedAt = b.CreatedAt,
        IsDeleted = b.IsDeleted
    };
}
