using Ambev.DeveloperEvaluation.Common.Pagination;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesCommand : PagedQuery, IRequest<PagedResult<GetSaleResult>>
{
    // Filtros adicionais podem ser adicionados aqui
}
