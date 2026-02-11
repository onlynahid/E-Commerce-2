using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Application.Services
{
    public class AboutService : IAboutService
    {
        private readonly IAboutRepository _aboutRepository;
        public AboutService(IAboutRepository aboutRepository)
        {
            _aboutRepository = aboutRepository;
        }
        public async Task<AboutDto> CreateAboutAsync(CreateAboutDto createAboutDto)
        {
            var about = new About
            {
                Title = createAboutDto.Title,
                Description = createAboutDto.Description
            };

            await _aboutRepository.AddAboutAsync(about);
            return MapToDto(about);
        }
        public async Task<bool> DeleteAboutAsync(int aboutId)
        {
            var about = await _aboutRepository.GetAboutByIdAsync(aboutId);
            if (about == null)
                return false;

            await _aboutRepository.DeleteAboutAsync(aboutId);
            return true;
        }
        public async Task<AboutDto> GetAboutByIdAsync(int aboutId)
        {
            var about = await _aboutRepository.GetAboutByIdAsync(aboutId);
            return about == null ? throw new KeyNotFoundException("Not Found AboutId") : MapToDto(about);
        }
        public async Task<IEnumerable<AboutDto>> GetAllAboutAsync()
        {
            var abouts = await _aboutRepository.GetAllAboutAsync();
            return abouts.Select(MapToDto);
        }        
        private static AboutDto MapToDto(About about)
        {
            return new AboutDto
            {
                Id = about.Id,
                Title = about.Title,
                Description = about.Description
            };
        }
        public async Task<AboutDto> UpdateAboutAsync(UpdateAboutDto updateAboutDto, int id)
        {
            var about = await _aboutRepository.GetAboutByIdAsync(id);
            if (about == null)
            {
                throw new KeyNotFoundException("About not found");
            }
            about.Title = updateAboutDto.Title;
            about.Description = updateAboutDto.Description;

            await _aboutRepository.UpdateAboutAsync(about);
            return MapToDto(about);
        }
    }
}
