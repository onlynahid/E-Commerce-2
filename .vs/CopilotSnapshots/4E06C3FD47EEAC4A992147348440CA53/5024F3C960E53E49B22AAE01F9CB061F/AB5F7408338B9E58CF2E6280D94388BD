using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        // Basic CRUD operations
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int categoryId);
        Task<Category> GetByIdAsync(int categoryId);
        Task<IEnumerable<Category>> GetAllAsync();
        
        // Extended operations with navigation properties
        Task<IEnumerable<Category>> GetAllWithProductsAsync();
        Task<Category> GetByIdWithProductsAsync(int categoryId);
        
        // Search and filtering
        Task<IEnumerable<Category>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Category>> GetWithPaginationAsync(int page, int pageSize);
        Task<int> GetCountAsync();
        
        // Validation
        Task<bool> ExistsByNameAsync(string categoryName);
        Task<bool> ExistsByNameExcludingIdAsync(string categoryName, int excludeId);
        
        // Popular categories (ordered by product count)
        Task<IEnumerable<Category>> GetPopularAsync(int count);
    }
}
