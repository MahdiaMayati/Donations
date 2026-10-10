using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Common.Pagination;

public static class QueryablePaginationExtensions
{
    /// <summary>
    /// Counts matching rows, then applies Skip/Take for the requested page.
    /// Caller should apply filtering and ordering before invoking this method.
    /// </summary>
    public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int limit,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? PaginationRequest.DefaultPage : page;
        limit = limit < 1
            ? PaginationRequest.DefaultLimit
            : Math.Min(limit, PaginationRequest.MaxLimit);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return PaginatedResult<T>.Create(items, page, limit, totalItems);
    }

    public static Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
        => query.ToPaginatedListAsync(pagination.Page, pagination.Limit, cancellationToken);
}
