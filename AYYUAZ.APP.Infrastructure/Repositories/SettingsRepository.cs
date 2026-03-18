using AYYUAZ.APP.Application.Exceptions.AppException;
using AYYUAZ.APP.Constants;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class SettingsRepository : GenericRepository<Settings>, ISettingsRepository
    {
        public SettingsRepository(AppDbContext context) : base(context)
        {
        }

        #region Settings-Specific Methods

        //public async Task AddSettingsAsync(Settings settings)
        //{
        //    await AddAsync(settings);
        //}

        //public async Task DeleteSettingsAsync(int id)
        //{
        //    await DeleteAsync(id);
        //}

        //public Task<List<Settings>> GetAllSettings()
        //{
        //    return _dbSet.ToListAsync();
        //}

        //public Task<Settings> GetSettingsById(int id)
        //{
        //    return _dbSet.FindAsync(id).AsTask();
        //}

        //public async Task UpdateSettingsAsync(Settings settings)
        //{
        //    await UpdateAsync(settings);
        //}

        public Task<Settings> GetCurrentSettings()
        {
            return _dbSet.FirstOrDefaultAsync();
        }

        public async Task<Dictionary<string, string>> GetSocialMediaLinksAsync()
        {
            var currentSettings = await GetCurrentSettings();

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

        public async Task UpdateSocialMediaLinksAsync(Dictionary<string, string> socialLinks)
        {
            var currentSettings = await GetCurrentSettings();

            if (currentSettings == null)
                throw new NotFoundException(ErrorMessages.SettingsNotFound);

            if (socialLinks.ContainsKey("facebook"))
                currentSettings.FacebookUrl = socialLinks["facebook"];
            if (socialLinks.ContainsKey("instagram"))
                currentSettings.InstagramUrl = socialLinks["instagram"];
            if (socialLinks.ContainsKey("twitter"))
                currentSettings.TwitterUrl = socialLinks["twitter"];

            await UpdateAsync(currentSettings);
        }

        #endregion
    }
}
