using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;

namespace Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;

public sealed record CreateBeneficiaryCommand(
    int AddressId,
    string IdPhotoUrl,
    bool IsHeadOfHousehold) : IRequest<BeneficiaryResponse>;
