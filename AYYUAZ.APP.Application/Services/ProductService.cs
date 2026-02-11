using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Application.Service 
{
    public class ProductService  : IProductService 
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileStorageService  _fileStorageService ;
        public ProductService (IProductRepository productRepository, IFileStorageService  fileStorageService)
        {
            _productRepository = productRepository;
            _fileStorageService  = fileStorageService ;
        }
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            string imageUrl = "default-product.jpg"; 
            
            if (createProductDto.Image != null && createProductDto.Image.Length > 0)
            {
                try
                {
                    imageUrl = await _fileStorageService .UploadImageAsync(createProductDto.Image, "products");
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Image upload failed: {ex.Message}");
                }
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

            await _productRepository.AddProductAsync(product);

            // Create discount if provided
            if (createProductDto.DiscountPercentage.HasValue && createProductDto.DiscountPercentage.Value > 0)
            {
                await _productRepository.AddDiscountToProductAsync(product.Id, createProductDto.DiscountPercentage.Value);
            }

            return await GetProductByIdAsync(product.Id);
        }
        public async Task<bool> AddDiscountToProductAsync(int productId, decimal discountPercentage)
        {
            return await _productRepository.AddDiscountToProductAsync(productId, discountPercentage);
        }
        public async Task<bool> RemoveDiscountFromProductAsync(int productId)
        {
            return await _productRepository.RemoveDiscountFromProductAsync(productId);
        }
        public async Task<bool> UpdateProductDiscountAsync(int productId, decimal newDiscountPercentage)
        {
            return await _productRepository.UpdateProductDiscountAsync(productId, newDiscountPercentage);
        }    
        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return false;

            if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl != "default-product.jpg")
            {
                await _fileStorageService .DeleteImageAsync(product.ImageUrl);
            }

            await _productRepository.DeleteProductAsync(productId);
            return true;
        }    
        public async Task<List<Product>> FilterProductsAsync(ProductFilterDto filter)
        {
            return await _productRepository.FilterProductsAsync
                (
                filter.AgeGroups,
                filter.Size,
                filter.Materials,
                filter.Colors,
                filter.MinPrice,
                filter.MaxPrice
             );
        }  
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Select(p => MapToDto(p));
        }     
        public async Task<IEnumerable<ProductDto>> GetAvailableProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products
                .Where(p => p.StockQuantity > 0)
                .Select(p => MapToDto(p));
        }     
        public async Task<IEnumerable<ProductDto>> GetLatestProductsAsync(int count = 10)
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .Select(p => MapToDto(p));
        }  
        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            return product == null ? throw new KeyNotFoundException("Product Not Found") : MapToDto(product);
        }     
        public  async Task<int> GetProductCountAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Count();
        }      
        public async Task<int> GetProductCountByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Count(p => p.CategoryId == categoryId);
        }
        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsByNameAsync(string searchTerm)
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var products = await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice);
            return products.Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsSortedByDateAsync(bool ascending = false)
        {
            var products = await _productRepository.GetAllProductsAsync();
            return ascending
                ? products.OrderBy(p => p.CreatedAt).Select(p => MapToDto(p))
                : products.OrderByDescending(p => p.CreatedAt).Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsSortedByPriceAsync(bool ascending = true)
        {
            var products = await _productRepository.GetAllProductsAsync();
            return ascending
                ? products.OrderBy(p => p.Price).Select(p => MapToDto(p))
                : products.OrderByDescending(p => p.Price).Select(p => MapToDto(p));
        }
        public async Task<IEnumerable<ProductDto>> GetProductsWithPaginationAsync(int page, int pageSize)
        {
            var products = await _productRepository.GetPagedAsync(page, pageSize);
            return products.Select(p => MapToDto(p));
        }
        public async Task<bool> IsProductAvailableAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return false;

            return product.StockQuantity > 0;
        }
        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (updateProductDto.Image != null && updateProductDto.Image.Length > 0)
            {
                try
                {
                    // Delete old image if it's not the default
                    if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl != "default-product.jpg")
                    {
                        await _fileStorageService.DeleteImageAsync(product.ImageUrl);
                    }


                    product.ImageUrl = await _fileStorageService.UploadImageAsync(updateProductDto.Image, "products");
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Image upload failed: {ex.Message}");
                }
            }

            // Update other properties
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

            await _productRepository.UpdateProductAsync(product);
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
