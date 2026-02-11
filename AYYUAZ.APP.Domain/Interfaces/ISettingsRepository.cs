using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface ISettingsRepository
    {
        Task<List<Settings>> GetAllSettingsAsync();
        Task<Settings> GetSettingsByIdAsync(int id);
        Task AddSettingsAsync(Settings settings);
        Task UpdateSettingsAsync(Settings settings);
        Task DeleteSettingsAsync(int id);                        
    }
}
