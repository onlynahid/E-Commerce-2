using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IHeroRepository
    {
        Task AddHeroAsync(Hero hero);
        Task<List<Hero>> GetAllHeroesAsync();
        Task<Hero> GetHeroByIdAsync(int id);
        Task UpdateHeroAsync(Hero hero);
        Task DeleteHeroAsync(int id);

    }
}
