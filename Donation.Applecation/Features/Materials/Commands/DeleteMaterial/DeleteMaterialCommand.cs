using MediatR;

namespace Donation.Application.Features.Materials.Commands.DeleteMaterial;

public sealed record DeleteMaterialCommand(Guid Id) : IRequest<bool>;
