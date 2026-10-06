using Donation.Application.DTOs.ItemPhoto.Response;
using MediatR;

namespace Donation.Application.Features.ItemPhotos.Commands.UpdateItemPhoto;

public sealed record UpdateItemPhotoCommand(Guid Id, string Url) : IRequest<ItemPhotoResponse?>;
