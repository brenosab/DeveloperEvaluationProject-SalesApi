using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequest
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string SaleNumber { get; set; } = string.Empty;
    [Required]
    public DateTime SaleDate { get; set; }
    [Required]
    public Guid CustomerId { get; set; }
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    [Required]
    public string Branch { get; set; } = string.Empty;
    [Required]
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
    public bool Cancelled { get; set; }
}

public class UpdateSaleItemRequest
{
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    public string ProductName { get; set; } = string.Empty;
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal UnitPrice { get; set; }
    public bool Cancelled { get; set; }
}
