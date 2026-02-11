using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromForm] UpdateProductDto updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID {ProductId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetProductsWithPagination([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                {
                    return BadRequest(new { message = "Page and page size must be greater than 0." });
                }

                var products = await _productService.GetProductsWithPaginationAsync(page, pageSize);
                var totalCount = await _productService.GetProductCountAsync();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated products");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("category/{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                var count = await _productService.GetProductCountByCategoryAsync(categoryId);

                return Ok(new
                {
                    products,
                    categoryId,
                    totalCount = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for category {CategoryId}", categoryId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term cannot be empty." });
                }

                var products = await _productService.GetProductsByNameAsync(searchTerm);
                return Ok(new
                {
                    products,
                    searchTerm,
                    count = products.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with term {SearchTerm}", searchTerm);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpPost("filter")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> FilterProducts([FromBody] ProductFilterDto filter)
        {
            try
            {
                var products = await _productService.FilterProductsAsync(filter);
                return Ok(new
                {
                    products = products.Select(p => new ProductDto
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
                    }),
                    filter,
                    count = products.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering products");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("sorted/price")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsSortedByPrice([FromQuery] bool ascending = true)
        {
            try
            {
                var products = await _productService.GetProductsSortedByPriceAsync(ascending);
                return Ok(new
                {
                    products,
                    sortOrder = ascending ? "ascending" : "descending",
                    sortBy = "price"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products sorted by price");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("sorted/date")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsSortedByDate([FromQuery] bool ascending = false)
        {
            try
            {
                var products = await _productService.GetProductsSortedByDateAsync(ascending);
                return Ok(new
                {
                    products,
                    sortOrder = ascending ? "ascending" : "descending",
                    sortBy = "createdAt"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products sorted by date");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("available")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAvailableProducts()
        {
            try
            {
                var products = await _productService.GetAvailableProductsAsync();
                return Ok(new
                {
                    products,
                    count = products.Count(),
                    filter = "available_only"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available products");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("latest")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLatestProducts([FromQuery] int count = 10)
        {
            try
            {
                if (count < 1 || count > 100)
                {
                    return BadRequest(new { message = "Count must be between 1 and 100." });
                }

                var products = await _productService.GetLatestProductsAsync(count);
                return Ok(new
                {
                    products,
                    requestedCount = count,
                    actualCount = products.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest products");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }       
        [HttpGet("{id}/availability")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CheckProductAvailability(int id)
        {
            try
            {
                var isAvailable = await _productService.IsProductAvailableAsync(id);
                var product = await _productService.GetProductByIdAsync(id);

                return Ok(new
                {
                    productId = id,
                    isAvailable,
                    stock = product.Stock,
                    productName = product.Name
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability for product {ProductId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }    
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetProductStatistics()
        {
            try
            {
                var totalProducts = await _productService.GetProductCountAsync();
                var availableProducts = await _productService.GetAvailableProductsAsync();
                var allProducts = await _productService.GetAllProductsAsync();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product statistics");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("price-range")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByPriceRange(
            [FromQuery] decimal minPrice, 
            [FromQuery] decimal maxPrice)
        {
            try
            {
                if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                {
                    return BadRequest(new { message = "Invalid price range." });
                }

                var products = await _productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
                return Ok(new
                {
                    products,
                    priceRange = new { minPrice, maxPrice },
                    count = products.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by price range");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpPost("{productId}/discount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> AddDiscountToProduct(int productId, [FromBody] AddDiscountRequest request)
        {
            try
            {
                if (request?.DiscountPercentage == null || request.DiscountPercentage < 0 || request.DiscountPercentage > 100)
                {
                    return BadRequest(new { message = "Discount percentage must be between 0 and 100." });
                }

                var result = await _productService.AddDiscountToProductAsync(productId, request.DiscountPercentage);
                if (!result)
                {
                    return NotFound(new { message = $"Product with ID {productId} not found or discount could not be added." });
                }

                // Get updated product to return current state
                var updatedProduct = await _productService.GetProductByIdAsync(productId);

                return Ok(new
                {
                    message = "Discount added successfully",
                    productId,
                    discountPercentage = request.DiscountPercentage,
                    product = updatedProduct
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {productId} not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding discount to product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpPut("{productId}/discount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateProductDiscount(int productId, [FromBody] UpdateDiscountRequest request)
        {
            try
            {
                if (request?.NewDiscountPercentage == null || request.NewDiscountPercentage < 0 || request.NewDiscountPercentage > 100)
                {
                    return BadRequest(new { message = "Discount percentage must be between 0 and 100." });
                }

                var result = await _productService.UpdateProductDiscountAsync(productId, request.NewDiscountPercentage);
                if (!result)
                {
                    return NotFound(new { message = $"Product with ID {productId} not found or discount could not be updated." });
                }

                // Get updated product to return current state
                var updatedProduct = await _productService.GetProductByIdAsync(productId);

                return Ok(new
                {
                    message = "Discount updated successfully",
                    productId,
                    newDiscountPercentage = request.NewDiscountPercentage,
                    product = updatedProduct
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {productId} not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating discount for product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpDelete("{productId}/discount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> RemoveDiscountFromProduct(int productId)
        {
            try
            {
                var result = await _productService.RemoveDiscountFromProductAsync(productId);
                if (!result)
                {
                    return NotFound(new { message = $"Product with ID {productId} not found or has no discount to remove." });
                }

                // Get updated product to return current state
                var updatedProduct = await _productService.GetProductByIdAsync(productId);

                return Ok(new
                {
                    message = "Discount removed successfully",
                    productId,
                    product = updatedProduct
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {productId} not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing discount from product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("{productId}/discount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetProductDiscount(int productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                
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
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {productId} not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting discount info for product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpPost("bulk-discount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> BulkManageDiscounts([FromBody] BulkDiscountRequest request)
        {
            try
            {
                if (request?.ProductDiscounts == null || !request.ProductDiscounts.Any())
                {
                    return BadRequest(new { message = "Product discount data is required." });
                }

                var results = new List<object>();
                var successCount = 0;
                var errorCount = 0;

                foreach (var item in request.ProductDiscounts)
                {
                    try
                    {
                        bool result;
                        if (item.DiscountPercentage == 0)
                        {
                            result = await _productService.RemoveDiscountFromProductAsync(item.ProductId);
                        }
                        else
                        {
                            result = await _productService.UpdateProductDiscountAsync(item.ProductId, item.DiscountPercentage);
                        }

                        if (result)
                        {
                            successCount++;
                            results.Add(new
                            {
                                productId = item.ProductId,
                                discountPercentage = item.DiscountPercentage,
                                status = "success"
                            });
                        }
                        else
                        {
                            errorCount++;
                            results.Add(new
                            {
                                productId = item.ProductId,
                                discountPercentage = item.DiscountPercentage,
                                status = "failed",
                                error = "Product not found or operation failed"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        results.Add(new
                        {
                            productId = item.ProductId,
                            discountPercentage = item.DiscountPercentage,
                            status = "error",
                            error = ex.Message
                        });
                    }
                }

                return Ok(new
                {
                    message = "Bulk discount operation completed",
                    totalProcessed = request.ProductDiscounts.Count(),
                    successCount,
                    errorCount,
                    results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk discount operation");
                return StatusCode(500, new { message = "Internal server error" });
            }
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