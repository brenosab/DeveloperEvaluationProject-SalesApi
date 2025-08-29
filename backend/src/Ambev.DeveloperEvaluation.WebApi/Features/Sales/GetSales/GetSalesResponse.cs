namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using System.Collections.Generic;

public class GetSalesResponse
{
    public List<GetSaleResponse> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
