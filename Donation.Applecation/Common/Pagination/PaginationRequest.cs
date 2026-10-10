using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Donation.Application.Common.Pagination;

/// <summary>
/// Shared query-string pagination parameters: ?page=1&amp;limit=10
/// </summary>
public sealed class PaginationRequest
{
    public const int DefaultPage = 1;
    public const int DefaultLimit = 10;
    public const int MaxLimit = 100;

    private int _page = DefaultPage;
    private int _limit = DefaultLimit;

    /// <summary>1-based page index.</summary>
    [DefaultValue(DefaultPage)]
    [Range(1, int.MaxValue)]
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? DefaultPage : value;
    }

    /// <summary>Page size (max 100).</summary>
    [DefaultValue(DefaultLimit)]
    [Range(1, MaxLimit)]
    public int Limit
    {
        get => _limit;
        set => _limit = value < 1
            ? DefaultLimit
            : Math.Min(value, MaxLimit);
    }

    /// <summary>Optional free-text filter when supported by the endpoint.</summary>
    public string? Search { get; set; }

    /// <summary>Optional sort field when supported by the endpoint.</summary>
    public string? SortBy { get; set; }

    /// <summary>When true, sort descending (default false = ascending).</summary>
    [DefaultValue(false)]
    public bool SortDesc { get; set; }
}
