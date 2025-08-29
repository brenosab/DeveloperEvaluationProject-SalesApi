namespace Ambev.DeveloperEvaluation.Domain.Filters;

public class SaleFilter
{
    public DateTime? MinSaleDate { get; init; }
    public DateTime? MaxSaleDate { get; init; }
    public string? CustomerName { get; init; }
    public string? SaleNumber { get; init; }
    public string? Branch { get; init; }
    public string? ItemDescription { get; init; }
    public string? ItemCategory { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public string? OrderBy { get; init; }
}
