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
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }
    }
}
