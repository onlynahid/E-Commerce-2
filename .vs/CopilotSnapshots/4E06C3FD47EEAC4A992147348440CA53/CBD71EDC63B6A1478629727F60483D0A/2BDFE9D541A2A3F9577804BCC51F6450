using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Application.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;
        private Settings? _settingsBackup;

        public SettingsService(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        // Basic CRUD operations
        public async Task<IEnumerable<SettingsDto>> GetAllSettingsAsync()
        {
            var settings = await _settingsRepository.GetAllSettingsAsync();
            return settings.Select(MapToDto);
        }

        public async Task<SettingsDto> GetSettingsByIdAsync(int settingsId)
        {
            var settings = await _settingsRepository.GetSettingsByIdAsync(settingsId);
            return settings == null ? null : MapToDto(settings);
        }

        public async Task<SettingsDto> CreateSettingsAsync(CreateSettingsDto createSettingsDto)
        {
            var settings = new Settings
            {
                Name = createSettingsDto.Name,
                PhoneNumber = createSettingsDto.PhoneNumber,
                Address = createSettingsDto.Address,
                Email = createSettingsDto.Email,
                FacebookUrl = createSettingsDto.FacebookUrl,
                InstagramUrl = createSettingsDto.InstagramUrl,
                TwitterUrl = createSettingsDto.TwitterUrl
            };

            await _settingsRepository.AddSettingsAsync(settings);
            return MapToDto(settings);
        }

        public async Task<SettingsDto> UpdateSettingsAsync(UpdateSettingsDto updateSettingsDto)
        {
            var settings = await _settingsRepository.GetSettingsByIdAsync(updateSettingsDto.Id);
            if (settings == null)
                return null;

            settings.Name = updateSettingsDto.Name;
            settings.PhoneNumber = updateSettingsDto.PhoneNumber;
            settings.Address = updateSettingsDto.Address;
            settings.Email = updateSettingsDto.Email;
            settings.FacebookUrl = updateSettingsDto.FacebookUrl;
            settings.InstagramUrl = updateSettingsDto.InstagramUrl;
            settings.TwitterUrl = updateSettingsDto.TwitterUrl;

            await _settingsRepository.UpdateSettingsAsync(settings);
            return MapToDto(settings);
        }

        public async Task<bool> DeleteSettingsAsync(int settingsId)
        {
            var settings = await _settingsRepository.GetSettingsByIdAsync(settingsId);
            if (settings == null)
                return false;

            await _settingsRepository.DeleteSettingsAsync(settingsId);
            return true;
        }

        // Settings specific operations
        public async Task<SettingsDto> GetCurrentSettingsAsync()
        {
            var allSettings = await _settingsRepository.GetAllSettingsAsync();
            var currentSettings = allSettings.FirstOrDefault();
            return currentSettings == null ? null : MapToDto(currentSettings);
        }

        public async Task<SettingsDto> GetDefaultSettingsAsync()
        {
            return new SettingsDto
            {
                Id = 0,
                Name = "Default Company",
                PhoneNumber = "+1-000-000-0000",
                Address = "Default Address",
                Email = "info@company.com",
                FacebookUrl = "",
                InstagramUrl = "",
                TwitterUrl = ""
            };
        }

        public async Task<bool> UpdateCurrentSettingsAsync(UpdateSettingsDto updateSettingsDto)
        {
            var allSettings = await _settingsRepository.GetAllSettingsAsync();
            var currentSettings = allSettings.FirstOrDefault();
            
            if (currentSettings == null)
            {
                // Create new settings if none exist
                var newSettings = new Settings
                {
                    Name = updateSettingsDto.Name,
                    PhoneNumber = updateSettingsDto.PhoneNumber,
                    Address = updateSettingsDto.Address,
                    Email = updateSettingsDto.Email,
                    FacebookUrl = updateSettingsDto.FacebookUrl,
                    InstagramUrl = updateSettingsDto.InstagramUrl,
                    TwitterUrl = updateSettingsDto.TwitterUrl
                };
                await _settingsRepository.AddSettingsAsync(newSettings);
                return true;
            }

            currentSettings.Name = updateSettingsDto.Name;
            currentSettings.PhoneNumber = updateSettingsDto.PhoneNumber;
            currentSettings.Address = updateSettingsDto.Address;
            currentSettings.Email = updateSettingsDto.Email;
            currentSettings.FacebookUrl = updateSettingsDto.FacebookUrl;
            currentSettings.InstagramUrl = updateSettingsDto.InstagramUrl;
            currentSettings.TwitterUrl = updateSettingsDto.TwitterUrl;

            await _settingsRepository.UpdateSettingsAsync(currentSettings);
            return true;
        }

        // Contact information
        public async Task<string> GetContactPhoneAsync()
        {
            var currentSettings = await GetCurrentSettingsAsync();
            return currentSettings?.PhoneNumber ?? "";
        }

        public async Task<string> GetContactEmailAsync()
        {
            var currentSettings = await GetCurrentSettingsAsync();
            return currentSettings?.Email ?? "";
        }

        public async Task<string> GetContactAddressAsync()
        {
            var currentSettings = await GetCurrentSettingsAsync();
            return currentSettings?.Address ?? "";
        }

        // Social media
        public async Task<Dictionary<string, string>> GetSocialMediaLinksAsync()
        {
            var currentSettings = await GetCurrentSettingsAsync();
            if (currentSettings == null)
            {
                return new Dictionary<string, string>
                {
                    { "facebook", "" },
                    { "instagram", "" },
                    { "twitter", "" }
                };
            }

            return new Dictionary<string, string>
            {
                { "facebook", currentSettings.FacebookUrl ?? "" },
                { "instagram", currentSettings.InstagramUrl ?? "" },
                { "twitter", currentSettings.TwitterUrl ?? "" }
            };
        }

        public async Task<bool> UpdateSocialMediaLinksAsync(Dictionary<string, string> socialLinks)
        {
            var allSettings = await _settingsRepository.GetAllSettingsAsync();
            var currentSettings = allSettings.FirstOrDefault();
            
            if (currentSettings == null)
                return false;

            if (socialLinks.ContainsKey("facebook"))
                currentSettings.FacebookUrl = socialLinks["facebook"];
            if (socialLinks.ContainsKey("instagram"))
                currentSettings.InstagramUrl = socialLinks["instagram"];
            if (socialLinks.ContainsKey("twitter"))
                currentSettings.TwitterUrl = socialLinks["twitter"];

            await _settingsRepository.UpdateSettingsAsync(currentSettings);
            return true;
        }

        // Validation
        public async Task<bool> ValidateSettingsAsync(SettingsDto settings)
        {
            if (settings == null)
                return false;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(settings.Name))
                return false;

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(settings.Email))
            {
                var emailAttribute = new EmailAddressAttribute();
                if (!emailAttribute.IsValid(settings.Email))
                    return false;
            }

            // Validate URLs if provided
            if (!string.IsNullOrWhiteSpace(settings.FacebookUrl) && !IsValidUrl(settings.FacebookUrl))
                return false;
            if (!string.IsNullOrWhiteSpace(settings.InstagramUrl) && !IsValidUrl(settings.InstagramUrl))
                return false;
            if (!string.IsNullOrWhiteSpace(settings.TwitterUrl) && !IsValidUrl(settings.TwitterUrl))
                return false;

            return true;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true;

            var allSettings = await _settingsRepository.GetAllSettingsAsync();
            return !allSettings.Any(s => s.Email != null && s.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int excludeId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true;

            var allSettings = await _settingsRepository.GetAllSettingsAsync();
            return !allSettings.Any(s => s.Id != excludeId && s.Email != null && s.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        // System settings
        public async Task<bool> ResetToDefaultSettingsAsync()
        {
            try
            {
                var allSettings = await _settingsRepository.GetAllSettingsAsync();
                var currentSettings = allSettings.FirstOrDefault();
                
                if (currentSettings == null)
                {
                    // Create default settings
                    var defaultSettings = new Settings
                    {
                        Name = "Default Company",
                        PhoneNumber = "+1-000-000-0000",
                        Address = "Default Address",
                        Email = "info@company.com",
                        FacebookUrl = "",
                        InstagramUrl = "",
                        TwitterUrl = ""
                    };
                    await _settingsRepository.AddSettingsAsync(defaultSettings);
                    return true;
                }

                // Update existing settings to default values
                currentSettings.Name = "Default Company";
                currentSettings.PhoneNumber = "+1-000-000-0000";
                currentSettings.Address = "Default Address";
                currentSettings.Email = "info@company.com";
                currentSettings.FacebookUrl = "";
                currentSettings.InstagramUrl = "";
                currentSettings.TwitterUrl = "";

                await _settingsRepository.UpdateSettingsAsync(currentSettings);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BackupCurrentSettingsAsync()
        {
            try
            {
                var allSettings = await _settingsRepository.GetAllSettingsAsync();
                var currentSettings = allSettings.FirstOrDefault();
                
                if (currentSettings == null)
                    return false;

                // Create a deep copy for backup
                _settingsBackup = new Settings
                {
                    Id = currentSettings.Id,
                    Name = currentSettings.Name,
                    PhoneNumber = currentSettings.PhoneNumber,
                    Address = currentSettings.Address,
                    Email = currentSettings.Email,
                    FacebookUrl = currentSettings.FacebookUrl,
                    InstagramUrl = currentSettings.InstagramUrl,
                    TwitterUrl = currentSettings.TwitterUrl
                };

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RestoreSettingsFromBackupAsync()
        {
            try
            {
                if (_settingsBackup == null)
                    return false;

                var allSettings = await _settingsRepository.GetAllSettingsAsync();
                var currentSettings = allSettings.FirstOrDefault();
                
                if (currentSettings == null)
                {
                    // Create new settings from backup
                    var restoredSettings = new Settings
                    {
                        Name = _settingsBackup.Name,
                        PhoneNumber = _settingsBackup.PhoneNumber,
                        Address = _settingsBackup.Address,
                        Email = _settingsBackup.Email,
                        FacebookUrl = _settingsBackup.FacebookUrl,
                        InstagramUrl = _settingsBackup.InstagramUrl,
                        TwitterUrl = _settingsBackup.TwitterUrl
                    };
                    await _settingsRepository.AddSettingsAsync(restoredSettings);
                    return true;
                }

                // Update existing settings from backup
                currentSettings.Name = _settingsBackup.Name;
                currentSettings.PhoneNumber = _settingsBackup.PhoneNumber;
                currentSettings.Address = _settingsBackup.Address;
                currentSettings.Email = _settingsBackup.Email;
                currentSettings.FacebookUrl = _settingsBackup.FacebookUrl;
                currentSettings.InstagramUrl = _settingsBackup.InstagramUrl;
                currentSettings.TwitterUrl = _settingsBackup.TwitterUrl;

                await _settingsRepository.UpdateSettingsAsync(currentSettings);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Helper methods
        private SettingsDto MapToDto(Settings settings)
        {
            return new SettingsDto
            {
                Id = settings.Id,
                Name = settings.Name,
                PhoneNumber = settings.PhoneNumber,
                Address = settings.Address,
                Email = settings.Email,
                FacebookUrl = settings.FacebookUrl,
                InstagramUrl = settings.InstagramUrl,
                TwitterUrl = settings.TwitterUrl
            };
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
