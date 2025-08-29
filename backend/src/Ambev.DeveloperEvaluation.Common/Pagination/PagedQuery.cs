namespace Ambev.DeveloperEvaluation.Common.Pagination;

public class PagedQuery : IPagingFilter, IOrderFilter
{
    private int _page = 1;
    private int _pageSize = 10;

    public bool Paginated { get; set; } = true;

    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int Page
    {
        get => _page;
        set => _page = value > 0 ? value : 1;
    }

    /// <summary>
    /// Page size (default: 10)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 0 ? value : 10;
    }

    /// <summary>
    /// Order by expression (e.g., "saleDate desc, totalAmount asc")
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Optional search/filter string (can be used for free text or advanced filtering)
    /// </summary>
    public string? Filter { get; set; }
}
