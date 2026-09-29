using Donation.Application.DTOs.Beneficiary.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.Beneficiaries.Commands.UpdateBeneficiary;

public sealed record UpdateBeneficiaryCommand(
    int Id,
    int AddressId,
    string IdPhotoUrl,
    bool IsHeadOfHousehold,
    VerificationStatus? VerificationStatus,
    DateTime? VerifiedUntil) : IRequest<BeneficiaryResponse?>;
