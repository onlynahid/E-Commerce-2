using AutoMapper;
using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Domain.Entities;

namespace AYYUAZ.APP.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
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
        }

        private static List<string> MapStringToList(string? input)
        {
            return !string.IsNullOrEmpty(input) ? input.Split(',').ToList() : new List<string>();
        }

        private static decimal CalculateFinalPrice(decimal price, Discount? discount)
        {
            if (discount != null && discount.Percentage.HasValue)
            {
                var discountAmount = (price * discount.Percentage.Value) / 100;
                return price - discountAmount;
            }
            return price;
        }
    }
}