namespace Ambev.DeveloperEvaluation.Domain.Filters;

public record SaleFilter(
    DateTime? StartDate,
    DateTime? EndDate,
    string? CustomerName,
    int Page,
    int PageSize
);