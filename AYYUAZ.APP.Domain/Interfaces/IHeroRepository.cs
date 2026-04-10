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
        Task<IEnumerable<Hero>> SearchByTitle(string searchTerm);
        Task<Hero> GetActiveHero();
        Task<bool> ExistsByTitle(string title);
    }
}
