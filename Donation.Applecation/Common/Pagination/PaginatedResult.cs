namespace Donation.Application.Common.Pagination;

/// <summary>
/// Pagination metadata nested under <c>data.pagination</c> in list responses.
/// </summary>
public sealed class PaginationMetadata
{
    public int Page { get; init; }
    public int Limit { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }

    public static PaginationMetadata Create(int page, int limit, int totalItems)
    {
        page = page < 1 ? PaginationRequest.DefaultPage : page;
        limit = limit < 1
            ? PaginationRequest.DefaultLimit
            : Math.Min(limit, PaginationRequest.MaxLimit);

        var totalPages = limit <= 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)limit);

        return new PaginationMetadata
        {
            Page = page,
            Limit = limit,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1 && totalItems > 0
        };
    }
}

/// <summary>
/// Uniform paginated payload for list endpoints:
/// <c>{ "items": [...], "pagination": { ... } }</c>
/// </summary>
public sealed class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public PaginationMetadata Pagination { get; init; } = PaginationMetadata.Create(
        PaginationRequest.DefaultPage,
        PaginationRequest.DefaultLimit,
        0);

    public static PaginatedResult<T> Create(
        IReadOnlyList<T> items,
        int page,
        int limit,
        int totalItems)
    {
        return new PaginatedResult<T>
        {
            Items = items,
            Pagination = PaginationMetadata.Create(page, limit, totalItems)
        };
    }
}
