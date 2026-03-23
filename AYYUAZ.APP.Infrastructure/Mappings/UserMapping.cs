using AutoMapper;
using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Infrastructure.ApplicationUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Infrastructure.Mappings
{
    public class UserMapping: Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
              .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
              .ForMember(dest => dest.FullName, opt => opt.Ignore())
              .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<TokenDto, TokenDto>();

            CreateMap<AuthResponseDto, AuthResponseDto>();
        }
    }
}
