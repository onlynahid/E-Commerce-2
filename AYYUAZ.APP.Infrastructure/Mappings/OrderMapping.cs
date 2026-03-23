using AutoMapper;
using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Infrastructure.Mappings
{
    public class OrderMapping : Profile
    {
        public OrderMapping() 
        {
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : string.Empty));
            CreateMap<CreateOrderItemDto, OrderItem>();

            CreateMap<Order, OrderDto>();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>();
            CreateMap<CheckOutDto, Order>();
        }
    }
}
