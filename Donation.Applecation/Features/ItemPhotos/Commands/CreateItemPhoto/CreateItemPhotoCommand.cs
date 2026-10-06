using Donation.Application.DTOs.ItemPhoto.Response;
using MediatR;

namespace Donation.Application.Features.ItemPhotos.Commands.CreateItemPhoto;

public sealed record CreateItemPhotoCommand(Guid ItemId, string Url) : IRequest<ItemPhotoResponse>;
