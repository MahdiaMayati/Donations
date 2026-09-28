using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Beneficiaries.Commands.UpdateBeneficiary;

public sealed class UpdateBeneficiaryCommandHandler : IRequestHandler<UpdateBeneficiaryCommand, BeneficiaryResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateBeneficiaryCommandHandler> _logger;

    public UpdateBeneficiaryCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateBeneficiaryCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<BeneficiaryResponse?> Handle(UpdateBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var beneficiary = await _context.Beneficiaries
            .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

        if (beneficiary is null)
        {
            return null;
        }

        EnsureCanManage(beneficiary);

        var addressExists = await _context.Addresses
            .AnyAsync(a => a.Id == request.AddressId, cancellationToken);

        if (!addressExists)
        {
            throw new NotFoundException("Address not found.");
        }

        beneficiary.AddressId = request.AddressId;
        beneficiary.IdPhotoUrl = request.IdPhotoUrl.Trim();
        beneficiary.IsHeadOfHousehold = request.IsHeadOfHousehold;

        if (_currentUser.IsAdmin)
        {
            if (request.VerificationStatus.HasValue)
            {
                beneficiary.VerificationStatus = request.VerificationStatus.Value;
            }

            if (request.VerifiedUntil.HasValue || request.VerificationStatus.HasValue)
            {
                beneficiary.VerifiedUntil = request.VerifiedUntil;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Beneficiary updated with Id {BeneficiaryId}", beneficiary.Id);

        return new BeneficiaryResponse
        {
            Id = beneficiary.Id,
            UserId = beneficiary.UserId,
            AddressId = beneficiary.AddressId,
            IdPhotoUrl = beneficiary.IdPhotoUrl,
            IsHeadOfHousehold = beneficiary.IsHeadOfHousehold,
            VerificationStatus = beneficiary.VerificationStatus,
            VerifiedUntil = beneficiary.VerifiedUntil,
            CreatedAt = beneficiary.CreatedAt,
            IsDeleted = beneficiary.IsDeleted
        };
    }

    private void EnsureCanManage(Beneficiary beneficiary)
    {
        if (_currentUser.IsAdmin)
        {
            return;
        }

        if (beneficiary.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only manage your own beneficiary profile.");
        }
    }
}
