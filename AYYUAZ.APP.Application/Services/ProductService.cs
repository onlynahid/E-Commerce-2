using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Application.Exceptions.AppException;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AYYUAZ.APP.Constants;
namespace AYYUAZ.APP.Application.Service 
{
    public class ProductService : IProductService 
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileStorageService _fileStorageService;
        public ProductService(IProductRepository productRepository, IFileStorageService fileStorageService)
        {
            _productRepository = productRepository;
            _fileStorageService = fileStorageService;
        }
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            const string defaultImageUrl = "default-product.jpg";
            string imageUrl = defaultImageUrl;
            const string products = "products";

            if (createProductDto.Image?.Length > 0)
            {
                imageUrl = await _fileStorageService.UploadImageAsync(createProductDto.Image,products);
            }

            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                StockQuantity = createProductDto.Stock,
                CategoryId = createProductDto.CategoryId,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                AgeGroup = createProductDto.AgeGroups != null ? string.Join(",", createProductDto.AgeGroups) : null,
                Material = createProductDto.Materials != null ? string.Join(",", createProductDto.Materials) : null,
                Size = createProductDto.Size != null ? string.Join(",", createProductDto.Size) : null,
                Colors = createProductDto.Colors != null ? string.Join(",", createProductDto.Colors) : null,
            };

            await _productRepository.AddAsync(product);

            if (createProductDto.DiscountPercentage.HasValue && createProductDto.DiscountPercentage.Value > 0)
            {
                await _productRepository.AddDiscountToProductAsync(product.Id, createProductDto.DiscountPercentage.Value);
            }
            return await GetProductById(product.Id);
        }
        public Task<bool> AddDiscountToProductAsync(int productId, decimal discountPercentage)
        {
            return _productRepository.AddDiscountToProductAsync(productId, discountPercentage);
        }
        public Task<bool> RemoveDiscountFromProductAsync(int productId)
        {
            return _productRepository.RemoveDiscountFromProductAsync(productId);
        }
        public Task<bool> UpdateProductDiscountAsync(int productId, decimal newDiscountPercentage)
        {
            return _productRepository.UpdateProductDiscountAsync(productId, newDiscountPercentage);
        }    
        public async Task<bool> DeleteProductAsync(int productId)
        {
            const string defaultImageUrl = "default-product.jpg";
            var product = await _productRepository.GetById(productId);
            if (product == null)
                throw new NotFoundException(ErrorMessages.ProductNotFound);

            if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl !=defaultImageUrl)
            {
                await _fileStorageService.DeleteImageAsync(product.ImageUrl);
            }

            await _productRepository.DeleteAsync(productId);
            return true;
        }    
        public Task<List<Product>> FilterProductsAsync(ProductFilterDto filter)
        {
            return _productRepository.FilterProductsAsync(
                filter.AgeGroups,
                filter.Size,
                filter.Materials,
                filter.Colors,
                filter.MinPrice,
                filter.MaxPrice,
                filter.Categories
            );
        }
        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            var products = await _productRepository.GetAllWithDetails();
            return products.Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetAvailableProducts()
        {
            var products = await _productRepository.GetAll();
            return products
                .Where(p => p.StockQuantity > 0)
                .Select(p => MapToDto(p));
        }     
        public async Task<IEnumerable<ProductDto>> GetLatestProducts(int count = 10)
        {
            var products = await _productRepository.GetAll();
            return products
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .Select(p => MapToDto(p));
        }  
        public async Task<ProductDto> GetProductById(int productId)
        {
            var product = await _productRepository.GetById(productId);
            return product == null ? throw new NotFoundException() : MapToDto(product);
        }     
        public async Task<int> GetProductCount()
        {
            var products = await _productRepository.GetAll();
            return products.Count();
        }      
        public async Task<int> GetProductCountByCategory(int categoryId)
        {
            var products = await _productRepository.GetAll();
            return products.Count(p => p.CategoryId == categoryId);
        }
        public async Task<IEnumerable<ProductDto>> GetProductsByCategory(int categoryId)
        {
            var products = await _productRepository.GetAll();
            return products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsByName(string searchTerm)
        {
            var products = await _productRepository.GetAll();
            return products
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var products = await _productRepository.GetByPriceRange(minPrice, maxPrice);
            return products.Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsSortedByDate(bool ascending = false)
        {
            var products = await _productRepository.GetAll();
            return ascending
                ? products.OrderBy(p => p.CreatedAt).Select(p => MapToDto(p))
                : products.OrderByDescending(p => p.CreatedAt).Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsSortedByPrice(bool ascending = true)
        {
            var products = await _productRepository.GetAll();
            return ascending
                ? products.OrderBy(p => p.Price).Select(p => MapToDto(p))
                : products.OrderByDescending(p => p.Price).Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsWithPagination(int page, int pageSize)
        {
            var products = await _productRepository.GetPagedAsync(page, pageSize);
            return products.Select(p => MapToDto(p));
        }
        public async Task<bool> IsProductAvailableAsync(int productId)
        {
            var product = await _productRepository.GetById(productId);
            if (product == null)
                throw new NotFoundException(ErrorMessages.ProductNotFound);

            return product.StockQuantity > 0;
        }
        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            const string defaultImageUrl = "default-product.jpg";
            const string products = "products";
            var product = await _productRepository.GetById(id);
            if (product == null)
                throw new NotFoundException(ErrorMessages.ProductNotFound);

            if (updateProductDto.Image != null && updateProductDto.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl !=defaultImageUrl)
                {
                    await _fileStorageService.DeleteImageAsync(product.ImageUrl);
                }
                product.ImageUrl = await _fileStorageService.UploadImageAsync(updateProductDto.Image,products);
            }

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;
            product.CategoryId = updateProductDto.CategoryId;
            product.AgeGroup = updateProductDto.AgeGroups != null ? string.Join(",", updateProductDto.AgeGroups) : null;
            product.Material = updateProductDto.Materials != null ? string.Join(",", updateProductDto.Materials) : null;
            product.Size = updateProductDto.Size != null ? string.Join(",", updateProductDto.Size) : null;
            product.Colors = updateProductDto.Colors != null ? string.Join(",", updateProductDto.Colors) : null;

            if (updateProductDto.Stock.HasValue)
            {
                product.StockQuantity = updateProductDto.Stock.Value;
            }

            await _productRepository.UpdateAsync(product);
            return MapToDto(product);
        }
        private ProductDto MapToDto(Product product)
        {
            decimal finalPrice = product.Price;

            if (product.Discount != null && product.Discount.Percentage > 0 && product.Discount.Percentage <= 100)
            {
                var discountAmount = product.Price * product.Discount.Percentage / 100m;
                finalPrice = Math.Max(0, (decimal)(product.Price - discountAmount));
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                Size = string.IsNullOrEmpty(product.Size) ? null : product.Size.Split(',').ToList(),
                AgeGroups = string.IsNullOrEmpty(product.AgeGroup) ? null : product.AgeGroup.Split(',').ToList(),
                Materials = string.IsNullOrEmpty(product.Material) ? null : product.Material.Split(',').ToList(),
                Colors = string.IsNullOrEmpty(product.Colors) ? null : product.Colors.Split(',').ToList(),
                FinalPrice = finalPrice,
                DiscountPercantage = product.Discount?.Percentage ?? 0m,
            };
        }
    }
}
