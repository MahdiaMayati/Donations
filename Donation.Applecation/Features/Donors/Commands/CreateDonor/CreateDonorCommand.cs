using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Commands.CreateDonor;

public sealed record CreateDonorCommand() : IRequest<DonorResponse>;
