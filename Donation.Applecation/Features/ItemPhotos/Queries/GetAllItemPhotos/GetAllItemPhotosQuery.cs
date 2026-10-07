using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemPhoto.Response;
using MediatR;

namespace Donation.Application.Features.ItemPhotos.Queries.GetAllItemPhotos;

public sealed record GetAllItemPhotosQuery(
    Guid? ItemId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit) : IRequest<PaginatedResult<ItemPhotoResponse>>;
