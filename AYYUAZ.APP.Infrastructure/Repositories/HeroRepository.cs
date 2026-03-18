using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class HeroRepository : GenericRepository<Hero>, IHeroRepository
    {
        public HeroRepository(AppDbContext context) : base(context)
        {
        }
        #region Hero-Specific Methods
        //public async Task AddHeroAsync(Hero hero)
        //{
        //    await AddAsync(hero);
        //}
        //public async Task DeleteHeroAsync(int heroId)
        //{
        //    await DeleteAsync(heroId);
        //}
        //public Task<List<Hero>> GetAllHeroes()
        //{
        //    return _dbSet.ToListAsync();
        //}
        //public Task<Hero> GetHeroById(int heroId)
        //{
        //    return _dbSet.FindAsync(heroId).AsTask();
        //}

        //public async Task UpdateHeroAsync(Hero hero)
        //{
        //    await UpdateAsync(hero);
        //}
        public async Task<IEnumerable<Hero>> SearchByTitle(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAll();

            return await _dbSet
                .Where(h => h.Title.Contains(searchTerm))
                .ToListAsync();
        }
        public Task<Hero> GetActiveHero()
        {
            return _dbSet.FirstOrDefaultAsync();
        }

        public Task<bool> ExistsByTitle(string title)
        {
            return _dbSet.AnyAsync(h => h.Title == title);
        }

        #endregion
    }
}
