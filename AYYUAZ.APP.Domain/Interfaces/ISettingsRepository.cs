using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface ISettingsRepository : IGenericRepository<Settings>
    {
        //Task<List<Settings>> GetAllSettings();
        //Task<Settings> GetSettingsById(int id);
        //Task AddSettingsAsync(Settings settings);
        //Task UpdateSettingsAsync(Settings settings);
        //Task DeleteSettingsAsync(int id);
        Task<Settings> GetCurrentSettings();
        Task<Dictionary<string, string>> GetSocialMediaLinksAsync();
        Task UpdateSocialMediaLinksAsync(Dictionary<string, string> socialLinks);
    }
}
