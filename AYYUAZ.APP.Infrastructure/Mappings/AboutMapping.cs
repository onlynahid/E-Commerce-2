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
    public class AboutMapping:Profile
    {
        public AboutMapping()
        {
            CreateMap<About, AboutDto>();
            CreateMap<CreateAboutDto, About>();
            CreateMap<UpdateAboutDto, About>();
        }
    }
}
