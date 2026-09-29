using Donation.Application.DTOs.Donor.Request;
using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Commands.CreateDonor;

public sealed record CreateDonorCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    string PreferredContactMethod,
    DonorAddressRequest Address) : IRequest<DonorResponse>;
