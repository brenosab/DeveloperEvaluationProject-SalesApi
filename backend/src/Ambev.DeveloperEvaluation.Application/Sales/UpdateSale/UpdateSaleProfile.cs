using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleCommand, Sale>()
            .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber ?? string.Empty))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName ?? string.Empty))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());

        CreateMap<UpdateSaleItemCommand, SaleItem>()
            .ForMember(dest => dest.Discount, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore());

        CreateMap<Sale, UpdateSaleResult>();
        CreateMap<SaleItem, UpdateSaleItemResult>();
    }
}
