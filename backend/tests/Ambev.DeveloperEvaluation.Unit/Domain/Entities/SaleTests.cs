using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact]
    public void ApplyBusinessRules_ShouldApplyDiscountsAndTotalCorrectly()
    {
        // Arrange
        var sale = TestData.SaleTestData.GenerateValidSale();
        foreach (var item in sale.Items)
        {
            item.Quantity = 5;
            item.UnitPrice = 100;
        }

        // Act
        sale.ApplyBusinessRules();

        // Assert
        foreach (var item in sale.Items)
        {
            Assert.Equal(0.10m, item.Discount);
            Assert.Equal(item.Quantity * item.UnitPrice * (1 - item.Discount), item.Total);
        }
        Assert.Equal(sale.Items.Sum(i => i.Total), sale.TotalAmount);
    }

    [Fact]
    public void ApplyBusinessRules_ShouldThrow_WhenQuantityExceeds20()
    {
        // Arrange

        var sale = TestData.SaleTestData.GenerateSaleWithInvalidItem();
        sale.Items[0].UnitPrice = 100;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.ApplyBusinessRules());
    }

    [Theory]
    [InlineData(3, 100, 0)]
    [InlineData(5, 100, 0.10)]
    [InlineData(15, 100, 0.20)]
    public void ApplyBusinessRules_ShouldApplyCorrectDiscountByQuantity(int quantity, decimal unitPrice, decimal expectedDiscount)
    {
        // Arrange

        var item = TestData.SaleTestData.GenerateSaleItem(quantity);
        item.UnitPrice = unitPrice;
        var sale = TestData.SaleTestData.GenerateValidSale(1);
        sale.Items = new List<SaleItem> { item };

        // Act
        sale.ApplyBusinessRules();

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
    }
}
