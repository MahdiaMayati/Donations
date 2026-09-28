using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Commands.UpdateDonor;

public sealed record UpdateDonorCommand(int Id) : IRequest<DonorResponse?>;
