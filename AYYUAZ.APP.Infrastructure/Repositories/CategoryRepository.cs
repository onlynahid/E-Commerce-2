using AYYUAZ.APP.Application.Exceptions.AppException;
using AYYUAZ.APP.Constants;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
        #region Category-Specific Methods
        //public override async Task<Category> AddAsync(Category category)
        //{
        //    category.CreatedAt = DateTime.UtcNow;
        //    return await base.AddAsync(category);
        //}

        //public override async Task DeleteAsync(int categoryId)
        //{
        //    var category = await GetByIdWithProducts(categoryId);
        //    if (category != null)
        //    {
        //        if (category.Products.Any())
        //        {
        //            throw new ConflictException(ErrorMessages.ConflictException);
        //        }
                
        //        _dbSet.Remove(category);
        //        await SaveChangesAsync();
        //    }
        //}
        public async Task<IEnumerable<Category>> GetAllWithProducts()
        {
            return await _dbSet
                .Include(c => c.Products)
                .ToListAsync();
        }
        public Task<Category> GetByIdWithProducts(int categoryId)
        {
            return _dbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
        public async Task<IEnumerable<Category>> GetWithPagination(int page, int pageSize)
        {
            return await _dbSet
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public Task<int> GetCount()
        {
            return _dbSet.CountAsync();
        }
        public Task<bool> ExistsByName(string categoryName)
        {
            return _dbSet.AnyAsync(c => c.Name == categoryName);
        }
        public Task<bool> ExistsByNameExcludingId(string categoryName, int excludeId)
        {
            return _dbSet.AnyAsync(c => c.Name == categoryName && c.Id != excludeId);
        }
        public async Task<IEnumerable<Category>> GetPopular(int count)
        {
            return await _dbSet
                .Include(c => c.Products)
                .OrderByDescending(c => c.Products.Count)
                .Take(count)
                .ToListAsync();
        }
        public async Task<IEnumerable<Category>> SearchByName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAll();

            return await _dbSet
                .Where(c => c.Name.Contains(searchTerm))
                .ToListAsync();
        }
        #endregion
    }
}
