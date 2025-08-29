using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SalesProfile : Profile
{
    public SalesProfile()
    {
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemCommand>();
        CreateMap<CreateSaleResult, CreateSaleResponse>();
        CreateMap<GetSaleResult, GetSaleResponse>();
        CreateMap<GetSaleItemResult, GetSaleItemResponse>();
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();
        CreateMap<DeleteSaleRequest, DeleteSaleCommand>();
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
        CreateMap<UpdateSaleItemResult, UpdateSaleItemResponse>();
        
        CreateMap<GetSalesRequest, GetSalesCommand>()
          .ForMember(dest => dest.MinSaleDate,
                     opt => opt.MapFrom(src => src.MinSaleDate.HasValue
                         ? src.MinSaleDate.Value.ToUniversalTime()
                         : (DateTime?)null))
          .ForMember(dest => dest.MaxSaleDate,
                     opt => opt.MapFrom(src => src.MaxSaleDate.HasValue
                         ? src.MaxSaleDate.Value.ToUniversalTime()
                         : (DateTime?)null));
        CreateMap<GetSaleResult, GetSaleResponse>();
        CreateMap<PagedResult<GetSaleResult>, GetSalesResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Data));
    }
}
