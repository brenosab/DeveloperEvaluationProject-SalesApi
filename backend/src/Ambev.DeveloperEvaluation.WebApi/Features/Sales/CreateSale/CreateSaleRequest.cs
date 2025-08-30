using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequest
{
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
    public List<CreateSaleItemRequest> Items { get; set; } = [];
}

public class CreateSaleItemRequest
{
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string Category { get; set; } = string.Empty;
    [Required]
    public string Image { get; set; } = string.Empty;
    [Required]
    public decimal RatingRate { get; set; }
    [Required]
    public int RatingCount { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal UnitPrice { get; set; }
}
