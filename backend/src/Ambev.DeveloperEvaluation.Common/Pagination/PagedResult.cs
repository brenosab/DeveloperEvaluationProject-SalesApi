namespace Ambev.DeveloperEvaluation.Common.Pagination;

public record PagedResult<T>(List<T> Data,
    int TotalItems,
    int CurrentPage,
    int TotalPages
);

