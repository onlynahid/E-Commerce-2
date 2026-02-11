using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProductAsync(int productId)
        {
          var product= await _context.Products.FindAsync(productId);
            if(product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public Task<List<Product>> FilterProductsAsync(List<string>? ageGroups, List<string>? sizes, List<string>? materials, List<string>? colors, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(a=>a.Discount)
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
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
         return await _context.Products
                .Include(c=>c.Category)
                .Include(c=>c.Discount)
                .ToListAsync();  
        }
        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Include(c => c.Category)
                .Include(c => c.Discount)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetPagedAsync(int page, int pageSize)
        {
              return await _context.Products
               .Include(c => c.Category)
                .Include(c => c.Discount)
             .OrderBy(p => p.Id)       
             .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync();
        }
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(a=>a.Discount)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);
            
        }
        public async Task UpdateProductAsync(Product product)
        {
             _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> AddDiscountToProductAsync(int productId, decimal discountPercentage)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
                return false;

            var product = await GetProductByIdAsync(productId);
            if (product == null)
                return false;

            var discount = new Discount
            {
                Percentage = discountPercentage
            };

            await _context.Discounts.AddAsync(discount);
            await _context.SaveChangesAsync();


            product.DiscountId = discount.Id;
            await UpdateProductAsync(product);

            return true;
        }
        public async Task<bool> RemoveDiscountFromProductAsync(int productId)
        {
            var product = await GetProductByIdAsync(productId);
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
                await UpdateProductAsync(product);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        public async Task<bool> UpdateProductDiscountAsync(int productId, decimal newDiscountPercentage)
        {
            if (newDiscountPercentage < 0 || newDiscountPercentage > 100)
                return false;

            var product = await GetProductByIdAsync(productId);
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
    }
}
