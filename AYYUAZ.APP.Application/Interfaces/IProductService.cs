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
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(int id,UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<ProductDto>> GetProductsByNameAsync(string searchTerm);
        Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<ProductDto>> GetProductsWithPaginationAsync(int page, int pageSize);
        Task<int> GetProductCountByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetProductsSortedByPriceAsync(bool ascending = true);
        Task<IEnumerable<ProductDto>> GetProductsSortedByDateAsync(bool ascending = false);
        Task<bool> IsProductAvailableAsync(int productId);
        Task<IEnumerable<ProductDto>> GetAvailableProductsAsync();
        Task<IEnumerable<ProductDto>> GetLatestProductsAsync(int count = 10);
        Task<int> GetProductCountAsync();
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Product>> FilterProductsAsync(ProductFilterDto filter);
        Task<bool> AddDiscountToProductAsync(int productId, decimal discountPercentage);
        Task<bool> RemoveDiscountFromProductAsync(int productId);
        Task<bool> UpdateProductDiscountAsync(int productId, decimal newDiscountPercentage);
    }
}
