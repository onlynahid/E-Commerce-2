using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYYUAZ.APP.Application.Dtos;

namespace AYYUAZ.APP.Application.Interfaces
{
    public interface ISettingsService 
    {
        Task<IEnumerable<SettingsDto>> GetAllSettings();
        Task<SettingsDto> GetSettingsById(int settingsId);
        Task<SettingsDto> CreateSettingsAsync(CreateSettingsDto createSettingsDto);
        Task<SettingsDto> UpdateSettingsAsync(UpdateSettingsDto updateSettingsDto);
        Task<bool> DeleteSettingsAsync(int settingsId);
        Task<bool> UpdateCurrentSettingsAsync(UpdateSettingsDto updateSettingsDto);
        Task<Dictionary<string, string>> GetSocialMediaLinks();
        Task<bool> UpdateSocialMediaLinksAsync(Dictionary<string, string> socialLinks);
        Task<bool> ValidateSettingsAsync(SettingsDto settings);
    }
}