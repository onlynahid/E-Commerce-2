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
        Task<IEnumerable<CategoryDto>> GetAllCategories();
        Task<CategoryDto> GetCategoryById(int categoryId);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto,int categoryid);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<IEnumerable<CategoryDto>> GetCategoriesWithProducts();
        Task<CategoryDto> GetCategoryWithProducts(int categoryId);
        Task<bool> IsCategoryNameUniqueAsync(string categoryName);
        Task<bool> IsCategoryNameUniqueAsync(string categoryName, int excludeId);
        Task<IEnumerable<CategoryDto>> SearchCategoriesByName(string searchTerm);
        Task<IEnumerable<CategoryDto>> GetCategoriesWithPagination(int page, int pageSize);
        Task<int> GetCategoryCount();
    }
}