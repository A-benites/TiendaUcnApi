/// <summary>
/// Generic class representing a paginated result set.
/// </summary>
/// <typeparam name="T">Type of items in the result set.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Current page number.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total count of items across all pages.
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// Total number of pages available based on TotalCount and PageSize.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Collection of items in the current page.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}
