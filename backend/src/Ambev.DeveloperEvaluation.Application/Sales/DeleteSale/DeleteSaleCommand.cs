using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
