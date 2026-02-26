using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Application.Exceptions.AppException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Application.Services
{
    public class HeroService : IHeroService
    {
        private readonly IHeroRepository _heroRepository;
        private readonly IFileStorageService _fileStorageService;

        public HeroService(IHeroRepository heroRepository, IFileStorageService fileStorageService)
        {
            _heroRepository = heroRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<HeroDto> AddHeroAsync(CreateHeroDto createHeroDto)
        {
            string imageUrl = "default-hero.jpg";

            if (createHeroDto.ImageUrl != null && createHeroDto.ImageUrl.Length > 0)
            {
                try
                {
                    imageUrl = await _fileStorageService.UploadImageAsync(createHeroDto.ImageUrl, "heroes");
                }
                catch (ArgumentException)
                {
                    throw new BadRequestException();
                }
            }

            var hero = new Hero
            {
                Title = createHeroDto.Title,
                Description = createHeroDto.Description,
                ImageUrl = imageUrl,
                DiscountMessage = createHeroDto.DiscountMessage,
                OrderMessage = createHeroDto.OrderMessage
            };

            await _heroRepository.AddHeroAsync(hero);
            return MapToDto(hero);
        }

        public async Task<List<HeroDto>> GetAllHeroesAsync()
        {
            var heroes = await _heroRepository.GetAllHeroesAsync();
            return heroes.Select(h => MapToDto(h)).ToList();
        }

        public async Task<HeroDto> GetHeroByIdAsync(int heroid)
        {
            var hero = await _heroRepository.GetHeroByIdAsync(heroid);
            if (hero == null)
                throw new NotFoundException();
            return MapToDto(hero);
        }

        public async Task<HeroDto> UpdateHeroAsync(int id, UpdateHeroDto updateHeroDto)
        {
            var hero = await _heroRepository.GetHeroByIdAsync(id);
            if (hero == null)
                throw new NotFoundException();

            if (updateHeroDto.ImageUrl != null && updateHeroDto.ImageUrl.Length > 0)
            {
                try
                {
                    if (!string.IsNullOrEmpty(hero.ImageUrl) && hero.ImageUrl != "default-hero.jpg")
                    {
                        await _fileStorageService.DeleteImageAsync(hero.ImageUrl);
                    }

                    hero.ImageUrl = await _fileStorageService.UploadImageAsync(updateHeroDto.ImageUrl, "heroes");
                }
                catch (ArgumentException)
                {
                    throw new BadRequestException();
                }
            }

            hero.Title = updateHeroDto.Title;
            hero.Description = updateHeroDto.Description;
            hero.DiscountMessage = updateHeroDto.DiscountMessage;
            hero.OrderMessage = updateHeroDto.OrderMessage;

            await _heroRepository.UpdateHeroAsync(hero);
            return MapToDto(hero);
        }

        public async Task<bool> DeleteHeroAsync(int id)
        {
            var hero = await _heroRepository.GetHeroByIdAsync(id);
            if (hero == null)
                throw new NotFoundException();

            if (!string.IsNullOrEmpty(hero.ImageUrl) && hero.ImageUrl != "default-hero.jpg")
            {
                await _fileStorageService.DeleteImageAsync(hero.ImageUrl);
            }

            await _heroRepository.DeleteHeroAsync(id);
            return true;
        }

        private HeroDto MapToDto(Hero hero)
        {
            return new HeroDto
            {
                Id = hero.Id,
                Title = hero.Title,
                Description = hero.Description,
                ImageUrl = hero.ImageUrl,
                DiscountMessage = hero.DiscountMessage,
                OrderMessage = hero.OrderMessage
            };
        }
    }
}
