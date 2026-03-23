using AutoMapper;
using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Infrastructure.Mappings
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.AgeGroups, opt => opt.MapFrom(src => MapStringToList(src.AgeGroup)))
                .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => MapStringToList(src.Material)))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => MapStringToList(src.Size)))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => MapStringToList(src.Colors)))
                .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => CalculateFinalPrice(src.Price, src.Discount)))
                .ForMember(dest => dest.DiscountPercantage, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.Percentage ?? 0 : 0));

            CreateMap<Product, ProductGetDto>()
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.AgeGroups, opt => opt.MapFrom(src => MapStringToList(src.AgeGroup)))
                .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => MapStringToList(src.Material)))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => MapStringToList(src.Size)))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => MapStringToList(src.Colors)));

            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.AgeGroup, opt => opt.MapFrom(src => src.AgeGroups != null ? string.Join(",", src.AgeGroups) : null))
                .ForMember(dest => dest.Material, opt => opt.MapFrom(src => src.Materials != null ? string.Join(",", src.Materials) : null))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size != null ? string.Join(",", src.Size) : null))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors != null ? string.Join(",", src.Colors) : null))
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock.HasValue ? src.Stock.Value : 0))
                .ForMember(dest => dest.AgeGroup, opt => opt.MapFrom(src => src.AgeGroups != null ? string.Join(",", src.AgeGroups) : null))
                .ForMember(dest => dest.Material, opt => opt.MapFrom(src => src.Materials != null ? string.Join(",", src.Materials) : null))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size != null ? string.Join(",", src.Size) : null))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors != null ? string.Join(",", src.Colors) : null))
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<ProductGetDto, Product>();
            CreateMap<ProductFilterDto, Product>();
            CreateMap<Product, ProductFilterDto>()
                .ForMember(dest => dest.AgeGroups, opt => opt.MapFrom(src => MapStringToList(src.AgeGroup)))
                .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => MapStringToList(src.Material)))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => MapStringToList(src.Size)))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => MapStringToList(src.Colors)));
        }

        private List<string> MapStringToList(string? input)
        {
            return string.IsNullOrEmpty(input) ? new List<string>() : new List<string>(input.Split(','));
        }

        private decimal CalculateFinalPrice(decimal price, Discount? discount)
        {
            if (discount == null || discount.Percentage == null)
                return price;

            return price - (price * (discount.Percentage.Value / 100));
        }
    }
}
