using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Pagination;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesCommand : PagedQuery, IRequest<PagedResult<GetSaleResult>>
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime? MinSaleDate { get; set; }
    public DateTime? MaxSaleDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public string ItemCategory {  get; set; } = string.Empty;
}
