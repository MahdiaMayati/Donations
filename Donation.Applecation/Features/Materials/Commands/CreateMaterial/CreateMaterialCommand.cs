using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Commands.CreateMaterial;

public sealed record CreateMaterialCommand(string Name) : IRequest<MaterialResponse>;
