using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Filters;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

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
        CreateMap<GetSalesCommand, SaleFilter>();
        CreateMap<Sale, GetSaleResult>();
        CreateMap<PagedResult<Sale>, PagedResult<GetSaleResult>>();
    }
}
