namespace Donation.Application.DTOs.Common;

/// <summary>
/// Backward-compatible alias. Prefer <see cref="Donation.Application.Common.Pagination.PaginatedResult{T}"/>.
/// </summary>
[Obsolete("Use Donation.Application.Common.Pagination.PaginatedResult<T> instead.")]
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
