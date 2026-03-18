using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Exceptions.AppException;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Constants;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;

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
                imageUrl = await _fileStorageService.UploadImageAsync(createHeroDto.ImageUrl, "heroes");
            }

            var hero = new Hero
            {
                Title = createHeroDto.Title,
                Description = createHeroDto.Description,
                ImageUrl = imageUrl,
                DiscountMessage = createHeroDto.DiscountMessage,
                OrderMessage = createHeroDto.OrderMessage
            };

            await _heroRepository.AddAsync(hero);

            return MapToDto(hero);
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
        public async Task<List<HeroDto>> GetAllHeroesAsync()
        {
            var heroes = await _heroRepository.GetAll();
            return heroes.Select(h => MapToDto(h)).ToList();
        }
        public async Task<HeroDto> GetHeroByIdAsync(int heroid)
        {
            var hero = await _heroRepository.GetById(heroid);
            if (hero == null)
                throw new NotFoundException(ErrorMessages.HeroNotFound);
            return MapToDto(hero);
        }
        public async Task<HeroDto> UpdateHeroAsync(int id, UpdateHeroDto updateHeroDto)
        {
            var hero = await _heroRepository.GetById(id);
            if (hero == null)
                throw new NotFoundException(ErrorMessages.HeroNotFound);

            if (updateHeroDto.ImageUrl?.Length > 0)
            {
                if (!string.IsNullOrEmpty(hero.ImageUrl) && hero.ImageUrl != "default-hero.jpg")
                {
                    await _fileStorageService.DeleteImageAsync(hero.ImageUrl);
                }

                hero.ImageUrl = await _fileStorageService.UploadImageAsync(updateHeroDto.ImageUrl, "heroes");
            }

            hero.Title = updateHeroDto.Title;
            hero.Description = updateHeroDto.Description;
            hero.DiscountMessage = updateHeroDto.DiscountMessage;
            hero.OrderMessage = updateHeroDto.OrderMessage;

            await _heroRepository.UpdateAsync(hero);

            return MapToDto(hero);
        }

        public async Task<bool> DeleteHeroAsync(int id)
        {
            var hero = await _heroRepository.GetById(id);
            if (hero == null)
                return false;
            if (!string.IsNullOrEmpty(hero.ImageUrl) && hero.ImageUrl != "default-hero.jpg")
            {
                await _fileStorageService.DeleteImageAsync(hero.ImageUrl);
            }
            await _heroRepository.DeleteAsync(id);
            return true;
        }
    }
}
