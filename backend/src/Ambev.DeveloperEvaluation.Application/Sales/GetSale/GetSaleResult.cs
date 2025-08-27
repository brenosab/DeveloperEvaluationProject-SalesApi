namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleResult
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
