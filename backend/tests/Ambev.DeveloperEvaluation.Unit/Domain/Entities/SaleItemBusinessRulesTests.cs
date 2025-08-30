using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemBusinessRulesTests
{
    [Theory]
    [InlineData(1, 10.0, 0, 10.0)]
    [InlineData(3, 5.0, 0, 15.0)]
    [InlineData(4, 10.0, 0.10, 36.0)]
    [InlineData(9, 10.0, 0.10, 81.0)]
    [InlineData(10, 10.0, 0.20, 80.0)]
    [InlineData(20, 5.0, 0.20, 80.0)]
    public void ApplyBusinessRules_ShouldApplyCorrectDiscount(int quantity, decimal unitPrice, decimal expectedDiscount, decimal expectedTotal)
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            Title = "Test Product",
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        // Act
        item.ApplyBusinessRules();

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal(expectedTotal, item.Total);
    }

    [Fact]
    public void ApplyBusinessRules_ShouldThrow_WhenQuantityGreaterThan20()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            Title = "Test Product",
            Quantity = 21,
            UnitPrice = 10.0m
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => item.ApplyBusinessRules());
    }
}
