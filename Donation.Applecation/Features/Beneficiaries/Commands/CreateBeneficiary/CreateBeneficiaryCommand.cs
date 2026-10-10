using Donation.Application.DTOs.Beneficiary.Request;
using Donation.Application.DTOs.Beneficiary.Response;
using MediatR;

namespace Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;

public sealed record CreateBeneficiaryCommand(
    CreateBeneficiaryUserRequest User,
    CreateBeneficiaryCityRequest City,
    CreateBeneficiaryAreaRequest Area,
    CreateBeneficiaryAddressRequest Address,
    string IdPhotoUrl,
    bool IsHeadOfHousehold) : IRequest<BeneficiaryResponse>;
