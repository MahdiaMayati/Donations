using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Queries.GetMaterialById;

public sealed record GetMaterialByIdQuery(Guid Id) : IRequest<MaterialResponse?>;
