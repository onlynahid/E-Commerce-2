using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace AYYUAZ.APP.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<AdminProductController> _logger;
        public AdminProductController(IProductService productService, ILogger<AdminProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            _logger.LogInformation("Creating new product");     
            var product = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromForm] UpdateProductDto updateProductDto)
        {
            _logger.LogInformation("Updating product ID: {ProductId}", id);   
            var product = await _productService.UpdateProductAsync(id, updateProductDto);
            return Ok(product);
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Deleting product ID: {ProductId}", id);
            
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            _logger.LogInformation("Getting product by ID: {ProductId}", id);
            var product = await _productService.GetProductById(id);
            return Ok(product);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            _logger.LogInformation("Getting all products");   
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetProductsWithPagination([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting products with pagination - Page: {Page}, Size: {PageSize}", page, pageSize);
            
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "Page and page size must be greater than 0." });
            }

            var products = await _productService.GetProductsWithPagination(page, pageSize);
            var totalCount = await _productService.GetProductCount();

            return Ok(new
            {
                products,
                totalCount,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                hasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
                hasPreviousPage = page > 1
            });
        }
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<object>> GetProductsByCategory(int categoryId)
        {
            _logger.LogInformation("Getting products for category: {CategoryId}", categoryId);
            
            var products = await _productService.GetProductsByCategory(categoryId);
            var count = await _productService.GetProductCountByCategory(categoryId);

            return Ok(new
            {
                products,
                categoryId,
                totalCount = count
            });
        }
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchProducts([FromQuery] string searchTerm)
        {
            _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty." });
            }

            var products = await _productService.GetProductsByName(searchTerm);
            return Ok(new
            {
                products,
                searchTerm,
                count = products.Count()
            });
        }
        [HttpPost("filter")]
        public async Task<ActionResult<object>> FilterProducts([FromBody] ProductFilterDto filter)
        {
            _logger.LogInformation("Filtering products");
            
            var products = await _productService.FilterProductsAsync(filter);
           
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                Size = string.IsNullOrEmpty(p.Size) ? null : p.Size.Split(',').ToList(),
                AgeGroups = string.IsNullOrEmpty(p.AgeGroup) ? null : p.AgeGroup.Split(',').ToList(),
                Materials = string.IsNullOrEmpty(p.Material) ? null : p.Material.Split(',').ToList(),
                Colors = string.IsNullOrEmpty(p.Colors) ? null : p.Colors.Split(',').ToList(),
                FinalPrice = p.Discount != null && p.Discount.Percentage.HasValue && p.Discount.Percentage.Value > 0 
                    ? Math.Max(0, p.Price - (p.Price * p.Discount.Percentage.Value / 100m))
                    : p.Price,
                DiscountPercantage = p.Discount?.Percentage ?? 0m
            }).ToList();
            
            return Ok(new
            {
                products = productDtos,
                filter,
                count = productDtos.Count
            });
        }
        [HttpGet("sorted/price")]
        public async Task<ActionResult<object>> GetProductsSortedByPrice([FromQuery] bool ascending = true)
        {
            _logger.LogInformation("Getting products sorted by price - Ascending: {Ascending}", ascending);
            
            var products = await _productService.GetProductsSortedByPrice(ascending);
            return Ok(new
            {
                products,
                sortOrder = ascending ? "ascending" : "descending",
                sortBy = "price"
            });
        }
        [HttpGet("sorted/date")]
        public async Task<ActionResult<object>> GetProductsSortedByDate([FromQuery] bool ascending = false)
        {
            _logger.LogInformation("Getting products sorted by date - Ascending: {Ascending}", ascending);
            
            var products = await _productService.GetProductsSortedByDate(ascending);
            return Ok(new
            {
                products,
                sortOrder = ascending ? "ascending" : "descending",
                sortBy = "createdAt"
            });
        }
        [HttpGet("available")]
        public async Task<ActionResult<object>> GetAvailableProducts()
        {
            _logger.LogInformation("Getting available products");
            
            var products = await _productService.GetAvailableProducts();
            return Ok(new
            {
                products,
                count = products.Count(),
                filter = "available_only"
            });
        }
        [HttpGet("latest")]
        public async Task<ActionResult<object>> GetLatestProducts([FromQuery] int count = 10)
        {
            _logger.LogInformation("Getting latest products - Count: {Count}", count);
            
            if (count < 1 || count > 100)
            {
                return BadRequest(new { message = "Count must be between 1 and 100." });
            }

            var products = await _productService.GetLatestProducts(count);
            return Ok(new
            {
                products,
                requestedCount = count,
                actualCount = products.Count()
            });
        }
        [HttpGet("{id}/availability")]
        public async Task<ActionResult<object>> CheckProductAvailability(int id)
        {
            _logger.LogInformation("Checking availability for product: {ProductId}", id);
            
            var isAvailable = await _productService.IsProductAvailableAsync(id);
            var product = await _productService.GetProductById(id);

            return Ok(new
            {
                productId = id,
                isAvailable,
                stock = product.Stock,
                productName = product.Name
            });
        }
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetProductStatistics()
        {
            _logger.LogInformation("Getting product statistics");
            
            var totalProducts = await _productService.GetProductCount();
            var availableProducts = await _productService.GetAvailableProducts();
            var allProducts = await _productService.GetAllProducts();

            var outOfStockCount = allProducts.Count(p => p.Stock == 0);
            var lowStockCount = allProducts.Count(p => p.Stock > 0 && p.Stock <= 5);

            return Ok(new
            {
                totalProducts,
                availableProducts = availableProducts.Count(),
                outOfStockProducts = outOfStockCount,
                lowStockProducts = lowStockCount,
                averagePrice = allProducts.Any() ? allProducts.Average(p => p.Price) : 0,
                highestPrice = allProducts.Any() ? allProducts.Max(p => p.Price) : 0,
                lowestPrice = allProducts.Any() ? allProducts.Min(p => p.Price) : 0
            });
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<object>> GetProductsByPriceRange(
            [FromQuery] decimal minPrice, 
            [FromQuery] decimal maxPrice)
        {
            _logger.LogInformation("Getting products by price range - Min: {MinPrice}, Max: {MaxPrice}", minPrice, maxPrice);
            
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
            {
                return BadRequest(new { message = "Invalid price range." });
            }

            var products = await _productService.GetProductsByPriceRange(minPrice, maxPrice);
            return Ok(new
            {
                products,
                priceRange = new { minPrice, maxPrice },
                count = products.Count()
            });
        }

        [HttpPost("{productId}/discount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<object>> AddDiscountToProduct(int productId, [FromBody] AddDiscountRequest request)
        {
            _logger.LogInformation("Adding discount to product: {ProductId}", productId);
            
            if (request?.DiscountPercentage == null || request.DiscountPercentage < 0 || request.DiscountPercentage > 100)
            {
                return BadRequest(new { message = "Discount percentage must be between 0 and 100." });
            }

            await _productService.AddDiscountToProductAsync(productId, request.DiscountPercentage);
            var updatedProduct = await _productService.GetProductById(productId);

            return Ok(new
            {
                message = "Discount added successfully",
                productId,
                discountPercentage = request.DiscountPercentage,
                product = updatedProduct
            });
        }

        [HttpPut("{productId}/discount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateProductDiscount(int productId, [FromBody] UpdateDiscountRequest request)
        {
            _logger.LogInformation("Updating discount for product: {ProductId}", productId);
            
            if (request?.NewDiscountPercentage == null || request.NewDiscountPercentage < 0 || request.NewDiscountPercentage > 100)
            {
                return BadRequest(new { message = "Discount percentage must be between 0 and 100." });
            }

            await _productService.UpdateProductDiscountAsync(productId, request.NewDiscountPercentage);
            var updatedProduct = await _productService.GetProductById(productId);

            return Ok(new
            {
                message = "Discount updated successfully",
                productId,
                newDiscountPercentage = request.NewDiscountPercentage,
                product = updatedProduct
            });
        }

        [HttpDelete("{productId}/discount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<object>> RemoveDiscountFromProduct(int productId)
        {
            _logger.LogInformation("Removing discount from product: {ProductId}", productId);
            
            await _productService.RemoveDiscountFromProductAsync(productId);
            var updatedProduct = await _productService.GetProductById(productId);

            return Ok(new
            {
                message = "Discount removed successfully",
                productId,
                product = updatedProduct
            });
        }
        [HttpGet("{productId}/discount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<object>> GetProductDiscount(int productId)
        {
            _logger.LogInformation("Getting discount info for product: {ProductId}", productId);

            var product = await _productService.GetProductById(productId);

            return Ok(new
            {
                productId,
                productName = product.Name,
                originalPrice = product.Price,
                discountPercentage = product.DiscountPercantage,
                finalPrice = product.FinalPrice,
                hasDiscount = product.DiscountPercantage > 0,
                savings = product.Price - product.FinalPrice
            });
        }
    }
    public class AddDiscountRequest
    {
        public decimal DiscountPercentage { get; set; }
    }

    public class UpdateDiscountRequest
    {
        public decimal NewDiscountPercentage { get; set; }
    }

    public class BulkDiscountRequest
    {
        public List<ProductDiscountItem> ProductDiscounts { get; set; } = new();
    }

    public class ProductDiscountItem
    {
        public int ProductId { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}