using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
        Task<Product> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> GetPagedAsync(int page, int pageSize);
        Task<List<Product>> FilterProductsAsync(
        List<string>? ageGroups,
        List<string>? sizes,
        List<string>? materials,
        List<string>? colors,
        decimal? minPrice,
        decimal? maxPrice
         );

        // Discount management methods
        Task<bool> AddDiscountToProductAsync(int productId, decimal discountPercentage);
        Task<bool> RemoveDiscountFromProductAsync(int productId);
        Task<bool> UpdateProductDiscountAsync(int productId, decimal newDiscountPercentage);
    }
}
