using AutoMapper;
using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Infrastructure.ApplicationUser;

namespace AYYUAZ.APP.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product
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

            // ProductGetDto, ProductFilterDto iki terefli map
            CreateMap<ProductGetDto, Product>();
            CreateMap<ProductFilterDto, Product>();
            CreateMap<Product, ProductFilterDto>()
                .ForMember(dest => dest.AgeGroups, opt => opt.MapFrom(src => MapStringToList(src.AgeGroup)))
                .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => MapStringToList(src.Material)))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => MapStringToList(src.Size)))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => MapStringToList(src.Colors)));

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
                .ForMember(dest => dest.FullName, opt => opt.Ignore()) // No FullName property in User
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); // No CreatedAt property in User

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // About
            CreateMap<About, AboutDto>();
            CreateMap<CreateAboutDto, About>();
            CreateMap<UpdateAboutDto, About>();

            // Settings
            CreateMap<Settings, SettingsDto>();
            CreateMap<CreateSettingsDto, Settings>();
            CreateMap<UpdateSettingsDto, Settings>();

            // Category
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            // OrderItem
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : string.Empty));
            CreateMap<CreateOrderItemDto, OrderItem>();

            // Order
            CreateMap<Order, OrderDto>();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>();

            // Hero
            CreateMap<Hero, HeroDto>();
            CreateMap<CreateHeroDto, Hero>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<UpdateHeroDto, Hero>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            // CheckOut
            CreateMap<CheckOutDto, Order>();

            // Token
            CreateMap<TokenDto, TokenDto>();

            // AuthResponse
            CreateMap<AuthResponseDto, AuthResponseDto>();
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