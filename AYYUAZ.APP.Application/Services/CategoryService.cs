using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AYYUAZ.APP.Application.Exceptions.AppException;
using AYYUAZ.APP.Constants;
using Microsoft.AspNetCore.Http;
using KeyNotFoundException = AYYUAZ.APP.Application.Exceptions.AppException.KeyNotFoundException;
namespace AYYUAZ.APP.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        public CategoryService(
            ICategoryRepository categoryRepository,
            IFileStorageService fileStorageService,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }
        public Task<CategoryDto> GetCategoryById(int categoryId)
        {
            var category = _categoryRepository.GetById(categoryId);
            return Task.FromResult(category != null ? _mapper.Map<CategoryDto>(category) : throw new KeyNotFoundException(ErrorMessages.CategoryNotFound));
        }
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            const string defaultImageUrl = "default-category.jpg";
            const string categories="categories";
            var isUnique = await IsCategoryNameUniqueAsync(createCategoryDto.Name);
            if (!isUnique)
            {
                throw new NotFoundException(ErrorMessages.CategoryAlreadyExists);
            }
            string imageUrl = defaultImageUrl;

            if (createCategoryDto.Image != null && createCategoryDto.Image.Length > 0)
            {
                imageUrl = await _fileStorageService.UploadImageAsync(createCategoryDto.Image,categories);
            }

            var category = _mapper.Map<Category>(createCategoryDto);
            category.ImageUrl = imageUrl;
            category.CreatedAt = DateTime.UtcNow;

            await _categoryRepository.AddAsync(category);

            return _mapper.Map<CategoryDto>(category);
        }
        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto, int categoryId)
        {
            const string defaultImageUrl = "default-category.jpg";
            const string categories = "categories";
            var category = await _categoryRepository.GetById(categoryId);
            if (category == null)
                throw new NotFoundException(ErrorMessages.CategoryNotFound);

            var isUnique = await IsCategoryNameUniqueAsync(updateCategoryDto.Name, categoryId);
            if (!isUnique)
                throw new NotFoundException(ErrorMessages.CategoryAlreadyExists);

            if (updateCategoryDto.Image != null && updateCategoryDto.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(category.ImageUrl) && category.ImageUrl != defaultImageUrl)
                {
                    await _fileStorageService.DeleteImageAsync(category.ImageUrl);
                }

                category.ImageUrl = await _fileStorageService.UploadImageAsync(updateCategoryDto.Image,categories);
            }

            _mapper.Map(updateCategoryDto, category);

            await _categoryRepository.UpdateAsync(category);

            return _mapper.Map<CategoryDto>(category);
        }
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            const string defaultImageUrl = "default-category.jpg";
            var category = await _categoryRepository.GetByIdWithProducts(categoryId);

            if (category == null)
                return false;

            if (category.Products.Any())
            {
                throw new ConflictException(ErrorMessages.ConflictException);
            }
            if (!string.IsNullOrEmpty(category.ImageUrl) && category.ImageUrl != defaultImageUrl)
            {
                await _fileStorageService.DeleteImageAsync(category.ImageUrl);
            }

            await _categoryRepository.DeleteAsync(categoryId);
            return true;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAll();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
        public async Task<IEnumerable<CategoryDto>> GetCategoriesWithProducts()
        {
            var categories = await _categoryRepository.GetAllWithProducts();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
        public async Task<CategoryDto> GetCategoryWithProducts(int categoryId)
        {
            var category = await _categoryRepository.GetByIdWithProducts(categoryId);
            return category != null ? _mapper.Map<CategoryDto>(category) : throw new NotFoundException(ErrorMessages.CategoryNotFound);
        }
        public async Task<bool> IsCategoryNameUniqueAsync(string categoryName)
        {
            var exists = await _categoryRepository.ExistsByName(categoryName);
            return !exists;
        }
        public async Task<bool> IsCategoryNameUniqueAsync(string categoryName, int excludeId)
        {
            var exists = await _categoryRepository.ExistsByNameExcludingId(categoryName, excludeId);
            return !exists;
        }
        public async Task<IEnumerable<CategoryDto>> SearchCategoriesByName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllCategories();
            }
            var categories = await _categoryRepository.SearchByName(searchTerm);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
        public async Task<IEnumerable<CategoryDto>> GetCategoriesWithPagination(int page, int pageSize)
        {
            var categories = await _categoryRepository.GetWithPagination(page, pageSize);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
        public Task<int> GetCategoryCount()
        {
            return _categoryRepository.GetCount();
        }
    }
}
