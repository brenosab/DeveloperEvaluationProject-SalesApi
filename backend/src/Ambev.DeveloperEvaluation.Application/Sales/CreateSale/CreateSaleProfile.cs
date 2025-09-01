using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber ?? string.Empty))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName ?? string.Empty))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()); // Calculated later

        CreateMap<CreateSaleItemCommand, SaleItem>()
            .ForMember(dest => dest.Discount, opt => opt.Ignore()) // Calculated later
            .ForMember(dest => dest.Total, opt => opt.Ignore()); // Calculated later

        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, CreateSaleItemResult>();
    }
}
