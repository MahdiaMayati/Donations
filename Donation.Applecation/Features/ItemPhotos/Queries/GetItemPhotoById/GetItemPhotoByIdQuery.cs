using Donation.Application.DTOs.ItemPhoto.Response;
using MediatR;

namespace Donation.Application.Features.ItemPhotos.Queries.GetItemPhotoById;

public sealed record GetItemPhotoByIdQuery(Guid Id) : IRequest<ItemPhotoResponse?>;
