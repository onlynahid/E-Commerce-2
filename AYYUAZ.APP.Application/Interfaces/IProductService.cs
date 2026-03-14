using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Application.Interfaces
{
    public interface IProductService 
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
        Task<ProductDto> GetProductById(int productId);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(int id,UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<ProductDto>> GetProductsByName(string searchTerm);
        Task<IEnumerable<ProductDto>> GetProductsByPriceRange(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<ProductDto>> GetProductsWithPagination(int page, int pageSize);
        Task<int> GetProductCountByCategory(int categoryId);
        Task<IEnumerable<ProductDto>> GetProductsSortedByPrice(bool ascending = true);
        Task<IEnumerable<ProductDto>> GetProductsSortedByDate(bool ascending = false);
        Task<bool> IsProductAvailableAsync(int productId);
        Task<IEnumerable<ProductDto>> GetAvailableProducts();
        Task<IEnumerable<ProductDto>> GetLatestProducts(int count = 10);
        Task<int> GetProductCount();
        Task<IEnumerable<ProductDto>> GetProductsByCategory(int categoryId);
        Task<List<Product>> FilterProductsAsync(ProductFilterDto filter);
        Task<bool> AddDiscountToProductAsync(int productId, decimal discountPercentage);
        Task<bool> RemoveDiscountFromProductAsync(int productId);
        Task<bool> UpdateProductDiscountAsync(int productId, decimal newDiscountPercentage);
    }
}
