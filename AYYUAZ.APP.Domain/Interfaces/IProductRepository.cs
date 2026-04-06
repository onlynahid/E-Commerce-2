using AYYUAZ.APP.Domain.Entities;
using System.Linq.Expressions;

namespace AYYUAZ.APP.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> FilterProductsAsync(List<string>? ageGroups, List<string>? sizes, List<string>? materials, List<string>? colors, decimal? minPrice, 
            decimal? maxPrice, List<string>? categories);
        Task<IEnumerable<Product>> GetByPriceRange(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> SearchByName(string searchTerm);
        Task<IEnumerable<Product>> GetByCategoryId(int categoryId);
        Task<Product?> GetByIdWithDetails(int productId);
        Task<IEnumerable<Product>> GetAllWithDetails();
        Task<IEnumerable<Product>> GetAvailableProducts();
        Task<IEnumerable<Product>> GetOutOfStockProducts();
        Task<bool> IsProductAvailableAsync(int productId);
        Task<IEnumerable<Product>> GetSortedByPrice(bool ascending = true);
        Task<IEnumerable<Product>> GetSortedByDate(bool ascending = false);
        Task<IEnumerable<Product>> GetLatestProducts(int count = 10);
        Task<bool> AddDiscountToProductAsync(int productId, decimal discountPercentage);
        Task<bool> RemoveDiscountFromProductAsync(int productId);
        Task<bool> UpdateProductDiscountAsync(int productId, decimal newDiscountPercentage);
        Task<IEnumerable<Product>> GetProductsWithDiscountsAsync();
        Task<int> GetProductCountByCategory(int categoryId);
        Task<decimal> GetAveragePrice();
        Task<decimal> GetAveragePriceByCategory(int categoryId);
    }
}
