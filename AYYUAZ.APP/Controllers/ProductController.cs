using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AYYUAZ.APP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);
            return Ok(product);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategory(categoryId);
            return Ok(products);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ModelState.AddModelError("searchTerm", "Search term cannot be empty.");
                return BadRequest(ModelState);
            }

            var products = await _productService.GetProductsByName(searchTerm);
            return Ok(products);
        }
        [HttpGet("price-range")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
            {
                ModelState.AddModelError("priceRange", "Invalid price range.");
                return BadRequest(ModelState);
            }

            var products = await _productService.GetProductsByPriceRange(minPrice, maxPrice);
            return Ok(products);
        }
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetProductsWithPagination([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                ModelState.AddModelError("pagination", "Page and page size must be greater than 0.");
                return BadRequest(ModelState);
            }

            var products = await _productService.GetProductsWithPagination(page, pageSize);
            var totalcount = await _productService.GetProductCount();

            return Ok(new
            {
                products,
                totalcount,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalcount / pageSize)
            });
        }
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAvailableProducts()
        {
            var products = await _productService.GetAvailableProducts();
            return Ok(products);
        }
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLatestProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetLatestProducts(count);
            return Ok(products);
        }
        [HttpGet("sorted-by-price")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsSortedByPrice([FromQuery] bool ascending = true)
        {
            var products = await _productService.GetProductsSortedByPrice(ascending);
            return Ok(products);
        }
        [HttpGet("{id}/availability")]
        public async Task<ActionResult<bool>> CheckProductAvailability(int id)
        {
            var isAvailable = await _productService.IsProductAvailableAsync(id);
            return Ok(new { productId = id, isAvailable });
        }
        [HttpPost("filter")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> FilterProducts([FromBody] ProductFilterDto filter)
        {
            if (filter == null)
            {
                ModelState.AddModelError("filter", "Filter criteria cannot be null.");
                return BadRequest(ModelState);
            }

            if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice > filter.MaxPrice)
            {
                ModelState.AddModelError("price", "Minimum price cannot be greater than maximum price.");
                return BadRequest(ModelState);
            }

            if ((filter.MinPrice ?? 0) < 0 || (filter.MaxPrice ?? 0) < 0)
            {
                ModelState.AddModelError("price", "Price values cannot be negative.");
                return BadRequest(ModelState);
            }

            var filteredProducts = await _productService.FilterProductsAsync(filter);
            var productDtos = filteredProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "",
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                AgeGroups = string.IsNullOrEmpty(p.AgeGroup) ? null : p.AgeGroup.Split(',').ToList(),
                Materials = string.IsNullOrEmpty(p.Material) ? null : p.Material.Split(',').ToList(),
                Size = string.IsNullOrEmpty(p.Size) ? null : p.Size.Split(',').ToList(),
                Colors = string.IsNullOrEmpty(p.Colors) ? null : p.Colors.Split(',').ToList()
            }).ToList();

            return Ok(new
            {
                products = productDtos,
                totalCount = productDtos.Count,
                appliedFilters = new
                {
                    ageGroups = filter.AgeGroups,
                    materials = filter.Materials,
                    sizes = filter.Size,
                    colors = filter.Colors,
                    minPrice = filter.MinPrice,
                    maxPrice = filter.MaxPrice
                }
            });
        }
        [HttpPost("test-filter")]
        public async Task<ActionResult> TestFilterLogic([FromBody] ProductFilterDto filter)
        {
            var filteredProducts = await _productService.FilterProductsAsync(filter);

            return Ok(new
            {
                message = "Test filter results",
                filterCriteria = new
                {
                    sizes = filter.Size,
                    ageGroups = filter.AgeGroups,
                    materials = filter.Materials,
                    colors = filter.Colors,
                    minPrice = filter.MinPrice,
                    maxPrice = filter.MaxPrice
                },
                resultsCount = filteredProducts.Count,
                firstFewResults = filteredProducts.Take(3).Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    rawSize = p.Size,
                    rawAgeGroup = p.AgeGroup,
                    rawMaterial = p.Material,
                    rawColors = p.Colors,
                    price = p.Price
                }).ToList()
            });
        }
        [HttpGet("debug/all-sizes")]
        public async Task<ActionResult> GetAllSizesForDebug()
        {
            var products = await _productService.GetAllProducts();
            var sizes = products
                .Where(p => !string.IsNullOrEmpty(p.Size?.FirstOrDefault()))
                .SelectMany(p => p.Size ?? new List<string>())
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            return Ok(new
            {
                message = "All unique sizes in database",
                sizes = sizes,
                totalProducts = products.Count(),
                productsWithSizes = products.Count(p => p.Size != null && p.Size.Any())
            });
        }
        [HttpGet("debug/all-data")]
        public async Task<ActionResult> GetAllDataForDebug()
        {
            var products = await _productService.GetAllProducts();
            var productDetails = products.Take(5).Select(p => new
            {
                id = p.Id,
                name = p.Name,
                sizeRaw = p.Size?.FirstOrDefault(),
                sizeList = p.Size,
                sizesCount = p.Size?.Count ?? 0,
                ageGroupRaw = p.AgeGroups?.FirstOrDefault(),
                ageGroupList = p.AgeGroups,
                materialsRaw = p.Materials?.FirstOrDefault(),
                materialsList = p.Materials,
                colorsRaw = p.Colors?.FirstOrDefault(),
                colorsList = p.Colors
            }).ToList();

            return Ok(new
            {
                message = "Raw data from first 5 products",
                totalProducts = products.Count(),
                sampleProducts = productDetails,
                allUniqueSizes = products
                    .Where(p => p.Size != null && p.Size.Any())
                    .SelectMany(p => p.Size)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList()
            });
        }
       [HttpGet("debug/raw-database")]
        public async Task<ActionResult> GetRawDatabaseData()
        {
            var filteredProducts = await _productService.FilterProductsAsync(new ProductFilterDto());

            var rawData = filteredProducts.Take(5).Select(p => new
            {
                id = p.Id,
                name = p.Name,
                sizeRaw = p.Size,
                ageGroupRaw = p.AgeGroup,
                materialRaw = p.Material,
                colorsRaw = p.Colors
            }).ToList();

            return Ok(new
            {
                message = "Raw database fields from first 5 products",
                totalProducts = filteredProducts.Count,
                sampleRawProducts = rawData
            });
        }
        [HttpPost("debug-simple-filter")]
        public async Task<ActionResult> DebugSimpleFilter()
        {
            var simpleFilter = new ProductFilterDto
            {
                Size = new List<string> { "L" }
            };

            var filteredProducts = await _productService.FilterProductsAsync(simpleFilter);

            return Ok(new
            {
                message = "Simple L size filter test",
                totalProducts = filteredProducts.Count,
                productsWithL = filteredProducts.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    rawSize = p.Size,
                    hasL = p.Size?.Contains("L") ?? false,
                    exactMatch = p.Size?.Split(',')?.Contains("L") ?? false
                }).ToList()
            });
        }
        [HttpGet("{id}/discount-info")]
        public async Task<ActionResult<object>> GetProductDiscountInfo(int id)
        {
            var product = await _productService.GetProductById(id);

            return Ok(new
            {
                productId = id,
                productName = product.Name,
                originalPrice = product.Price,
                discountPercentage = product.DiscountPercantage,
                finalPrice = product.FinalPrice,
                hasDiscount = product.DiscountPercantage > 0,
                savings = product.Price - product.FinalPrice,
                savingsPercentage = product.Price > 0
                    ? Math.Round((product.Price - product.FinalPrice) / product.Price * 100, 2)
                    : 0
            });
        }
        [HttpGet("with-discounts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsWithDiscounts()
        {
            var products = await _productService.GetAllProducts();

            var productsWithDiscounts = products
                .Where(p => p.DiscountPercantage > 0)
                .OrderByDescending(p => p.DiscountPercantage)
                .ToList();

            return Ok(new
            {
                products = productsWithDiscounts,
                count = productsWithDiscounts.Count,
                message = "Products with active discounts"
            });
        }
        [HttpGet("discounts-above")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsWithDiscountsAbove([FromQuery] decimal minDiscountPercentage = 10)
        {
            if (minDiscountPercentage < 0 || minDiscountPercentage > 100)
            {
                return BadRequest(new { message = "Discount percentage must be between 0 and 100." });
            }

            var products = await _productService.GetAllProducts();

            var filteredProducts = products
                .Where(p => p.DiscountPercantage >= minDiscountPercentage)
                .OrderByDescending(p => p.DiscountPercantage)
                .ToList();

            return Ok(new
            {
                products = filteredProducts,
                count = filteredProducts.Count,
                minDiscountPercentage,
                message = $"Products with discounts of {minDiscountPercentage}% or more"
            });
        }
        [HttpGet("discount-statistics")]
        public async Task<ActionResult<object>> GetDiscountStatistics()
        {
            var products = await _productService.GetAllProducts();
            var productsList = products.ToList();

            var productsWithDiscounts = productsList
                .Where(p => p.DiscountPercantage > 0)
                .ToList();

            var totalSavings = productsWithDiscounts.Sum(p => p.Price - p.FinalPrice);

            return Ok(new
            {
                totalProducts = productsList.Count,
                productsWithDiscounts = productsWithDiscounts.Count,
                productsWithoutDiscounts = productsList.Count - productsWithDiscounts.Count,
                discountCoverage = productsList.Count > 0
                    ? Math.Round((double)productsWithDiscounts.Count / productsList.Count * 100, 2)
                    : 0,
                averageDiscountPercentage = productsWithDiscounts.Any()
                    ? Math.Round(productsWithDiscounts.Average(p => p.DiscountPercantage), 2)
                    : 0,
                highestDiscount = productsWithDiscounts.Any()
                    ? productsWithDiscounts.Max(p => p.DiscountPercantage)
                    : 0,
                lowestDiscount = productsWithDiscounts.Any()
                    ? productsWithDiscounts.Min(p => p.DiscountPercantage)
                    : 0,
                totalPotentialSavings = Math.Round(totalSavings, 2),
                discountRanges = new
                {
                    under10Percent = productsWithDiscounts.Count(p => p.DiscountPercantage < 10),
                    between10And25Percent = productsWithDiscounts.Count(p => p.DiscountPercantage >= 10 && p.DiscountPercantage < 25),
                    between25And50Percent = productsWithDiscounts.Count(p => p.DiscountPercantage >= 25 && p.DiscountPercantage < 50),
                    above50Percent = productsWithDiscounts.Count(p => p.DiscountPercantage >= 50)
                }
            });
        }
    }
}