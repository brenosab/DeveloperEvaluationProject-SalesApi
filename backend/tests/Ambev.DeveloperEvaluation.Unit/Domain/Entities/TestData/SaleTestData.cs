using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for Sale and SaleItem using the Bogus library.
/// Centralizes all test data generation for Sale-related tests.
/// </summary>
public static class SaleTestData
{
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.Id, f => Guid.NewGuid())
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Title, f => f.Commerce.ProductName())
        .RuleFor(i => i.Price, f => f.Random.Decimal(1, 1000))
        .RuleFor(i => i.Description, f => f.Commerce.ProductDescription())
        .RuleFor(i => i.Category, f => f.Commerce.Categories(1)[0])
        .RuleFor(i => i.Image, f => f.Image.PicsumUrl())
        .RuleFor(i => i.RatingRate, f => f.Random.Decimal(0, 5))
        .RuleFor(i => i.RatingCount, f => f.Random.Int(0, 1000))
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, (f, i) => i.Price)
        .RuleFor(i => i.Discount, 0)
        .RuleFor(i => i.Total, 0)
        .RuleFor(i => i.Cancelled, f => false);

    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => Guid.NewGuid())
        .RuleFor(s => s.SaleNumber, f => f.Random.AlphaNumeric(10))
        .RuleFor(s => s.SaleDate, f => f.Date.Past())
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.Branch, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => SaleItemFaker.Generate(f.Random.Int(1, 5)))
        .RuleFor(s => s.TotalAmount, 0)
        .RuleFor(s => s.Cancelled, f => false);

    /// <summary>
    /// Generates a valid Sale entity with random valid items.
    /// </summary>
    public static Sale GenerateValidSale(int? itemCount = null)
    {
        var sale = SaleFaker.Generate();
        if (itemCount.HasValue)
            sale.Items = SaleItemFaker.Generate(itemCount.Value);
        return sale;
    }

    /// <summary>
    /// Generates a Sale entity with an item that exceeds the allowed quantity (invalid scenario).
    /// </summary>
    public static Sale GenerateSaleWithInvalidItem()
    {
        var sale = GenerateValidSale();
        sale.Items[0].Quantity = 21;
        return sale;
    }

    /// <summary>
    /// Generates a valid SaleItem with a specific quantity.
    /// </summary>
    public static SaleItem GenerateSaleItem(int quantity)
    {
        var item = SaleItemFaker.Generate();
        item.Quantity = quantity;
        return item;
    }
}
