using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileStorageService _fileStorageService;
        public CategoryService(
            ICategoryRepository categoryRepository, 
            IFileStorageService fileStorageService)
        {
            _categoryRepository = categoryRepository;
            _fileStorageService = fileStorageService;
        }
        public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            return category != null ? MapToDto(category) : throw new KeyNotFoundException("Category Not Found");
        }
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
           
            var isUnique = await IsCategoryNameUniqueAsync(createCategoryDto.Name);
            if (!isUnique)
            {
                throw new ArgumentException("Category name already exists.");
            }

            string imageUrl = "default-category.jpg";
            
            if (createCategoryDto.Image != null && createCategoryDto.Image.Length > 0)
            {
                try
                {
                    imageUrl = await _fileStorageService.UploadImageAsync(createCategoryDto.Image, "categories");
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Image upload failed: {ex.Message}");
                }
            }

            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            return MapToDto(category);
        }
        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(updateCategoryDto.Id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            var isUnique = await IsCategoryNameUniqueAsync(updateCategoryDto.Name, updateCategoryDto.Id);
            if (!isUnique)
            {
                throw new ArgumentException("Category name already exists.");
            }
           
            if (updateCategoryDto.Image != null && updateCategoryDto.Image.Length > 0)
            {
                try
                {
                   
                    if (!string.IsNullOrEmpty(category.ImageUrl) && category.ImageUrl != "default-category.jpg")
                    {
                        await _fileStorageService.DeleteImageAsync(category.ImageUrl);
                    }
                   
                    category.ImageUrl = await _fileStorageService.UploadImageAsync(updateCategoryDto.Image, "categories");
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Image upload failed: {ex.Message}");
                }
            }
            else if (!string.IsNullOrEmpty(updateCategoryDto.ImageUrl))
            {
                category.ImageUrl = updateCategoryDto.ImageUrl;
            }

           
            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;

            await _categoryRepository.UpdateAsync(category);
            return MapToDto(category);
        }
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdWithProductsAsync(categoryId);

            if (category == null)
                return false;
            if (category.Products.Any())
            {
                throw new InvalidOperationException("Cannot delete category that has products. Please move or delete products first.");
            }

            // Delete image file if it exists and is not the default image
            if (!string.IsNullOrEmpty(category.ImageUrl) && category.ImageUrl != "default-category.jpg")
            {
                await _fileStorageService.DeleteImageAsync(category.ImageUrl);
            }

            await _categoryRepository.DeleteAsync(categoryId);
            return true;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToDto);
        }
        public async Task<IEnumerable<CategoryDto>> GetCategoriesWithProductsAsync()
        {
            var categories = await _categoryRepository.GetAllWithProductsAsync();
            return categories.Select(MapToDto);
        }
        public async Task<CategoryDto> GetCategoryWithProductsAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdWithProductsAsync(categoryId);
            return category != null ? MapToDto(category) : throw new KeyNotFoundException("Category Not Found");
        }
        public async Task<bool> IsCategoryNameUniqueAsync(string categoryName)
        {
            var exists = await _categoryRepository.ExistsByNameAsync(categoryName);
            return !exists;
        }
        public async Task<bool> IsCategoryNameUniqueAsync(string categoryName, int excludeId)
        {
            var exists = await _categoryRepository.ExistsByNameExcludingIdAsync(categoryName, excludeId);
            return !exists;
        }
        public async Task<IEnumerable<CategoryDto>> SearchCategoriesByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllCategoriesAsync();
            }

            var categories = await _categoryRepository.SearchByNameAsync(searchTerm);
            return categories.Select(MapToDto);
        }
        public async Task<IEnumerable<CategoryDto>> GetCategoriesWithPaginationAsync(int page, int pageSize)
        {
            var categories = await _categoryRepository.GetWithPaginationAsync(page, pageSize);
            return categories.Select(MapToDto);
        }
        public async Task<int> GetCategoryCountAsync()
        {
            return await _categoryRepository.GetCountAsync();
        }
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                ProductCount = category.Products?.Count ?? 0
            };
        }
    }
}
