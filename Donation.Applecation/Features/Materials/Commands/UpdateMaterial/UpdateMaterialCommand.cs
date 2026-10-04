using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Commands.UpdateMaterial;

public sealed record UpdateMaterialCommand(Guid Id, string Name) : IRequest<MaterialResponse?>;
