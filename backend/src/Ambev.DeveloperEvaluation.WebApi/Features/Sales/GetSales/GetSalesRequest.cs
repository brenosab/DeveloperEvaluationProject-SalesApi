using Ambev.DeveloperEvaluation.Common.Pagination;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

public class GetSalesRequest : PagedQuery
{
    public string? SaleNumber { get; set; }
    public DateTime? MinSaleDate { get; set; }
    public DateTime? MaxSaleDate { get; set; }
    public string? CustomerName { get; set; }
    public string? Branch { get; set; }
    public string? ItemDescription { get; set; }
    public string? ItemCategory { get; set; }
}