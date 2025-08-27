using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system, including items, discounts, and business rules enforcement.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer (user) who made the purchase.
    /// </summary>
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of items in the sale.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the total sale amount (sum of all items after discounts).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets whether the sale is cancelled.
    /// </summary>
    public bool Cancelled { get; set; }
}

/// <summary>
/// Represents an item in a sale, including quantity, price, discount, and business rules.
/// </summary>
public class SaleItem : BaseEntity
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public bool Cancelled { get; set; }

    /// <summary>
    /// Applies business rules for discounts and restrictions based on quantity.
    /// </summary>
    public void ApplyBusinessRules()
    {
        if (Quantity < 4)
        {
            Discount = 0;
        }
        else if (Quantity >= 4 && Quantity < 10)
        {
            Discount = 0.10m;
        }
        else if (Quantity >= 10 && Quantity <= 20)
        {
            Discount = 0.20m;
        }
        else if (Quantity > 20)
        {
            throw new InvalidOperationException("Cannot sell more than 20 identical items.");
        }
        Total = Quantity * UnitPrice * (1 - Discount);
    }
}
