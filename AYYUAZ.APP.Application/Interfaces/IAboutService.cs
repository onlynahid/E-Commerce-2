using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Application.Interfaces
{
    public interface IAboutService 
    {
        Task<IEnumerable<AboutDto>> GetAllAboutAsync();
        Task<AboutDto> GetAboutByIdAsync(int aboutId);
        Task<AboutDto> CreateAboutAsync(CreateAboutDto createAboutDto);
        Task<AboutDto> UpdateAboutAsync(UpdateAboutDto updateAboutDto,int id);
        Task<bool> DeleteAboutAsync(int aboutId);
       
    }
}