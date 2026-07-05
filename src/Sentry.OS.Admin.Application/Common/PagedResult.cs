namespace Sentry.OS.Admin.Application.Common;

/// <summary>Generic paged collection wrapper used as the <c>data</c> payload of list endpoints.</summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize) =>
        new() { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize };
}

/// <summary>Query parameters shared by every collection endpoint.</summary>
public class PagingRequest
{
    private const int MaxPageSize = 200;
    private int _pageSize = 50;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > MaxPageSize ? 50 : value;
    }
}
