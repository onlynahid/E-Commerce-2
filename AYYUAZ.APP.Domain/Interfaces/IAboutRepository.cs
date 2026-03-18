using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IAboutRepository : IGenericRepository<About>
    {
        //Task<About> GetAboutById(int aboutId);
        //Task<IEnumerable<About>> GetAllAbout();
        //Task AddAboutAsync(About about);
        //Task UpdateAboutAsync(About about);
        //Task DeleteAboutAsync(int aboutId);
        Task<bool> ExistsByTitle(string title);
        Task<bool> ExistsByTitleExcludingId(string title, int excludeId);
        Task<IEnumerable<About>> SearchByTitle(string searchTerm);
    }
}
