using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Beneficiaries.Queries.GetBeneficiaryById;

public sealed class GetBeneficiaryByIdQueryHandler : IRequestHandler<GetBeneficiaryByIdQuery, BeneficiaryResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetBeneficiaryByIdQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BeneficiaryResponse?> Handle(GetBeneficiaryByIdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var beneficiary = await _context.Beneficiaries
            .AsNoTracking()
            .Where(b => b.Id == request.Id && !b.IsDeleted)
            .Select(b => new BeneficiaryResponse
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
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (beneficiary is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin && beneficiary.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only view your own beneficiary profile.");
        }

        return beneficiary;
    }
}
