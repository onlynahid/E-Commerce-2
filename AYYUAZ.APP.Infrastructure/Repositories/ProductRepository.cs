using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        #region Product-Specific Methods

        public Task<List<Product>> FilterProductsAsync(List<string>? ageGroups, List<string>? sizes, List<string>? materials, List<string>? colors, decimal? minPrice, 
            decimal? maxPrice, List<string>? categories)
        {
            var query = _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .AsQueryable();

            if (ageGroups != null && ageGroups.Any() && !ageGroups.Contains("string"))
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.AgeGroup) &&
                    ageGroups.Any(ag => ("," + p.AgeGroup + ",").ToLower().Contains("," + ag.ToLower() + ",")));
            }

            if (sizes != null && sizes.Any() && !sizes.Contains("string"))
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.Size) &&
                    sizes.Any(s => ("," + p.Size + ",").ToLower().Contains("," + s.ToLower() + ",")));
            }

            if (materials != null && materials.Any() && !materials.Contains("string"))
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.Material) &&
                    materials.Any(m => ("," + p.Material + ",").ToLower().Contains("," + m.ToLower() + ",")));
            }

            if (colors != null && colors.Any() && !colors.Contains("string"))
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.Colors) &&
                    colors.Any(c => ("," + p.Colors + ",").ToLower().Contains("," + c.ToLower() + ",")));
            }
            if (categories != null && categories.Any() && !categories.Contains("string"))
            {
                query = query.Where(p => categories.Any(c => p.Category.Name.ToLower().Contains(c.ToLower())));
            }


            if (minPrice.HasValue && minPrice.Value > 0)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            return query.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchByName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllWithDetails();

            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => p.Name.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryId(int categoryId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public Task<Product?> GetByIdWithDetails(int productId)
        {
            return _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<IEnumerable<Product>> GetAllWithDetails()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAvailableProducts()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => p.StockQuantity > 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetOutOfStockProducts()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => p.StockQuantity == 0)
                .ToListAsync();
        }

        public async Task<bool> IsProductAvailableAsync(int productId)
        {
            var product = await GetById(productId);
            return product?.StockQuantity > 0;
        }

        public async Task<IEnumerable<Product>> GetSortedByPrice(bool ascending = true)
        {
            var query = _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount);

            return await (ascending
                ? query.OrderBy(p => p.Price).ToListAsync()
                : query.OrderByDescending(p => p.Price).ToListAsync());
        }

        public async Task<IEnumerable<Product>> GetSortedByDate(bool ascending = false)
        {
            var query = _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount);

            return await (ascending
                ? query.OrderBy(p => p.CreatedAt).ToListAsync()
                : query.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        public async Task<IEnumerable<Product>> GetLatestProducts(int count = 10)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> AddDiscountToProductAsync(int productId, decimal discountPercentage)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
                return false;

            var product = await GetByIdWithDetails(productId);
            if (product == null)
                return false;

            var discount = new Discount
            {
                Percentage = discountPercentage
            };

            await _context.Discounts.AddAsync(discount);
            await _context.SaveChangesAsync();

            product.DiscountId = discount.Id;
            await UpdateAsync(product);

            return true;
        }
        public async Task<bool> RemoveDiscountFromProductAsync(int productId)
        {
            var product = await GetByIdWithDetails(productId);
            if (product == null)
                return false;

            if (product.DiscountId.HasValue)
            {
                var discount = await _context.Discounts.FindAsync(product.DiscountId.Value);
                if (discount != null)
                {
                    _context.Discounts.Remove(discount);
                }

                product.DiscountId = null;
                await UpdateAsync(product);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> UpdateProductDiscountAsync(int productId, decimal newDiscountPercentage)
        {
            if (newDiscountPercentage < 0 || newDiscountPercentage > 100)
                return false;

            var product = await GetByIdWithDetails(productId);
            if (product == null)
                return false;

            if (product.Discount != null)
            {
                product.Discount.Percentage = newDiscountPercentage;
                _context.Discounts.Update(product.Discount);
                await _context.SaveChangesAsync();
            }
            else
            {
                await AddDiscountToProductAsync(productId, newDiscountPercentage);
            }

            return true;
        }

        public async Task<IEnumerable<Product>> GetProductsWithDiscountsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => p.Discount != null && p.Discount.Percentage > 0)
                .ToListAsync();
        }

        public Task<int> GetProductCountByCategory(int categoryId)
        {
            return _dbSet.CountAsync(p => p.CategoryId == categoryId);
        }

        public Task<decimal> GetAveragePrice()
        {
            return _dbSet.AverageAsync(p => p.Price);
        }

        public Task<decimal> GetAveragePriceByCategory(int categoryId)
        {
            return _dbSet.Where(p => p.CategoryId == categoryId).AverageAsync(p => p.Price);
        }

        #endregion
    }
}
