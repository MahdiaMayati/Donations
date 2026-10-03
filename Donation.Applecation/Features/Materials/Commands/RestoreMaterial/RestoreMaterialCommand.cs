using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Commands.RestoreMaterial;

public sealed record RestoreMaterialCommand(Guid Id) : IRequest<MaterialResponse?>;
