using Donation.Application.DTOs.Donor.Request;
using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Commands.UpdateDonor;

public sealed record UpdateDonorCommand(
    Guid Id,
    string? FullName,
    string? Email,
    string? PhoneNumber,
    string? Password,
    string? PreferredContactMethod,
    DonorAddressRequest? Address) : IRequest<DonorResponse?>;
