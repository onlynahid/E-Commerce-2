using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class SettingsRepository: ISettingsRepository
    {
        private readonly AppDbContext _context;

        public SettingsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddSettingsAsync(Settings settings)
        {
            await _context.Settings.AddAsync(settings);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSettingsAsync(int id)
        {
            var settings = await _context.Settings.FindAsync(id);
            if (settings != null)
            {
                _context.Settings.Remove(settings);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Settings>> GetAllSettingsAsync()
        {
          return  await _context.Settings.ToListAsync();
        }
        public async Task<Settings> GetSettingsByIdAsync(int id)
        {
          return await _context.Settings.FindAsync(id);
        }
        public async Task UpdateSettingsAsync(Settings settings)
        {
            _context.Update(settings);
            await _context.SaveChangesAsync();
        }
    }
}
