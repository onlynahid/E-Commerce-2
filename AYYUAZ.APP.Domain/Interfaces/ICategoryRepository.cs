using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllWithProducts();
        Task<Category> GetByIdWithProducts(int categoryId);
        Task<IEnumerable<Category>> SearchByName(string searchTerm);
        Task<IEnumerable<Category>> GetWithPagination(int page, int pageSize);
        Task<int> GetCount();
        Task<bool> ExistsByName(string categoryName);
        Task<bool> ExistsByNameExcludingId(string categoryName, int excludeId);
        Task<IEnumerable<Category>> GetPopular(int count);
    }
}
