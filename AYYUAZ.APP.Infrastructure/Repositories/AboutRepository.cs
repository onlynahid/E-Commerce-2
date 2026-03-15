using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class AboutRepository : GenericRepository<About>, IAboutRepository
    {
        public AboutRepository(AppDbContext context) : base(context)
        {
        }

        #region About-Specific Methods

        public async Task AddAboutAsync(About about)
        {
            await AddAsync(about);
        }

        public async Task DeleteAboutAsync(int aboutId)
        {
            await DeleteAsync(aboutId);
        }

        public async Task UpdateAboutAsync(About about)
        {
            await UpdateAsync(about);
        }

        public Task<bool> ExistsByTitle(string title)
        {
            return _dbSet.AnyAsync(a => a.Title == title);
        }

        public Task<bool> ExistsByTitleExcludingId(string title, int excludeId)
        {
            return _dbSet.AnyAsync(a => a.Title == title && a.Id != excludeId);
        }

        public async Task<IEnumerable<About>> SearchByTitle(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAbout();

            return await _dbSet
                .Where(a => a.Title.Contains(searchTerm))
                .ToListAsync();
        }

        public Task<About> GetAboutById(int aboutId)
        {
            return _dbSet.FindAsync(aboutId).AsTask();
        }

        public async  Task<IEnumerable<About>> GetAllAbout()
        {
            return await _dbSet.ToListAsync();
        }

        #endregion
    }
}
