using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IHeroRepository : IGenericRepository<Hero>
    {
        //Task<Hero> GetHeroById(int heroId);
        //Task<List<Hero>> GetAllHeroes();
        //Task AddHeroAsync(Hero hero);
        //Task UpdateHeroAsync(Hero hero);
        //Task DeleteHeroAsync(int heroId);
        Task<IEnumerable<Hero>> SearchByTitle(string searchTerm);
        Task<Hero> GetActiveHero();
        Task<bool> ExistsByTitle(string title);
    }
}
