using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Commands.RestoreDonor;

public sealed record RestoreDonorCommand(Guid Id) : IRequest<DonorResponse?>;
