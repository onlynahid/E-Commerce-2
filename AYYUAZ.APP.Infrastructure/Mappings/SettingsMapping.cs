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
    public class SettingsMapping: Profile
    {
        public SettingsMapping()
        {
            CreateMap<Settings, SettingsDto>();
            CreateMap<CreateSettingsDto, Settings>();
            CreateMap<UpdateSettingsDto, Settings>();

        }
    }
}
