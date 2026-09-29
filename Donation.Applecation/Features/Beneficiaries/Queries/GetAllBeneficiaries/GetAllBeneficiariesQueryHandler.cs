using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Beneficiaries.Queries.GetAllBeneficiaries;

public sealed class GetAllBeneficiariesQueryHandler : IRequestHandler<GetAllBeneficiariesQuery, IReadOnlyList<BeneficiaryResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllBeneficiariesQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<BeneficiaryResponse>> Handle(GetAllBeneficiariesQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var query = _context.Beneficiaries
            .AsNoTracking()
            .Where(b => !b.IsDeleted);

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(b => b.UserId == _currentUser.UserId.Value);
        }

        return await query
            .OrderByDescending(b => b.Id)
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
            .ToListAsync(cancellationToken);
    }
}
