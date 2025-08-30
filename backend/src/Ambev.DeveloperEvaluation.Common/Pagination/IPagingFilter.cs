namespace Ambev.DeveloperEvaluation.Common.Pagination;


public interface IPagingFilter
{
    int Page { get; set; }
    int PageSize { get; set; }
}

public static class PagingFilterExtensions
{
    public static int GetSkipSize(this IPagingFilter pagingFilter)
    {
        return (pagingFilter.Page - 1) * pagingFilter.PageSize;
    }
}

public interface IOrderFilter
{
    string? OrderBy { get; set; }
}
