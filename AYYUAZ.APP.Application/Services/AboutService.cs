using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Exceptions.AppException;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AYYUAZ.APP.Constants;

namespace AYYUAZ.APP.Application.Services
{
    public class AboutService : IAboutService
    {
        private readonly IAboutRepository _aboutRepository;
        private readonly IMapper _mapper;
        public AboutService(IAboutRepository aboutRepository, IMapper mapper)
        {
            _aboutRepository = aboutRepository;
            _mapper = mapper;
        }
        public async Task<AboutDto> CreateAboutAsync(CreateAboutDto createAboutDto)
        {
            var about = _mapper.Map<About>(createAboutDto);
            await _aboutRepository.AddAboutAsync(about);
            return _mapper.Map<AboutDto>(about);
        }
        public async Task<bool> DeleteAboutAsync(int aboutId)
        {
            var about = await _aboutRepository.GetAboutById(aboutId);
            if (about == null)
                return false;

            await _aboutRepository.DeleteAboutAsync(aboutId);
            return true;
        }
        public  Task<AboutDto> GetAboutById(int aboutId)
        {
            var about =  _aboutRepository.GetAboutById(aboutId);
            if (about == null)
            {
                throw new NotFoundException(ErrorMessages.AboutNotFound);
            }
            return Task.FromResult(_mapper.Map<AboutDto>(about));
        }
        public  Task<IEnumerable<AboutDto>> GetAllAbout()
        {
            var abouts =  _aboutRepository.GetAllAbout(); 
            return Task.FromResult(_mapper.Map<IEnumerable<AboutDto>>(abouts));
        }
        public async Task<AboutDto> UpdateAboutAsync(UpdateAboutDto updateAboutDto, int id)
        {
            var about = await _aboutRepository.GetAboutById(id);
            _mapper.Map(updateAboutDto, about);
            await _aboutRepository.UpdateAboutAsync(about);
            return _mapper.Map<AboutDto>(about);
        }
    }
}
