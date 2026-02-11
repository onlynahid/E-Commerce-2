using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYYUAZ.APP.Application.Dtos;

namespace AYYUAZ.APP.Application.Interfaces
{
    public interface ICategoryService 
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<IEnumerable<CategoryDto>> GetCategoriesWithProductsAsync();
        Task<CategoryDto> GetCategoryWithProductsAsync(int categoryId);
        Task<bool> IsCategoryNameUniqueAsync(string categoryName);
        Task<bool> IsCategoryNameUniqueAsync(string categoryName, int excludeId);
        Task<IEnumerable<CategoryDto>> SearchCategoriesByNameAsync(string searchTerm);
        Task<IEnumerable<CategoryDto>> GetCategoriesWithPaginationAsync(int page, int pageSize);
        Task<int> GetCategoryCountAsync();
    }
}