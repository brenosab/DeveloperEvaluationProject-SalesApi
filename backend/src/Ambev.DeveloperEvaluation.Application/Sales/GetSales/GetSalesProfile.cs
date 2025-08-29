using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Profile for mapping between Sale entity and GetSaleResponse
/// </summary>
public class GetSalesProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale operation
    /// </summary>
    public GetSalesProfile()
    {
        // Map PagedResult<Sale> to PagedResult<GetSaleResult>
        CreateMap(typeof(PagedResult<>), typeof(PagedResult<>))
            .ConvertUsing(typeof(PagedResultConverter<,>));
        
        CreateMap<GetSalesCommand, SaleFilter>();
    }
}
