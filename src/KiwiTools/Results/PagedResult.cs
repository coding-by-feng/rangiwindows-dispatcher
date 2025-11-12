namespace KiwiTools.Results;

/// <summary>
/// Represents a paged response in a format similar to PageResult in the Java stack.
/// </summary>
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int PageNumber, int PageSize, long TotalCount)
{
    public long TotalPages => PageSize == 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasNextPage => PageNumber < TotalPages;

    public bool HasPreviousPage => PageNumber > 1;

    public static PagedResult<T> Empty(int pageNumber, int pageSize) =>
        new(Array.Empty<T>(), pageNumber, pageSize, 0);
}
