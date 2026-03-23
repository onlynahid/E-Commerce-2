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
    public class HeroMapping: Profile
    {
        public HeroMapping()
        {
            CreateMap<Hero, HeroDto>();
            CreateMap<CreateHeroDto, Hero>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<UpdateHeroDto, Hero>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }
    }
}
