using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace AYYUAZ.APP.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public AdminCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromForm] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if category name is unique
            if (!await _categoryService.IsCategoryNameUniqueAsync(createCategoryDto.Name))
            {
                return BadRequest(new { message = "Category name already exists." });
            }

            var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromForm] UpdateCategoryDto updateCategoryDto)
        {
            if (id != updateCategoryDto.Id)
            {
                return BadRequest("Category ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if category name is unique (excluding current category)
            if (!await _categoryService.IsCategoryNameUniqueAsync(updateCategoryDto.Name, id))
            {
                return BadRequest(new { message = "Category name already exists." });
            }

            var category = await _categoryService.UpdateCategoryAsync(updateCategoryDto);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }
        [HttpGet("with-products")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesWithProducts()
        {
            var categories = await _categoryService.GetCategoriesWithProductsAsync();
            return Ok(categories);
        }
        [HttpGet("{id}/with-products")]
        public async Task<ActionResult<CategoryDto>> GetCategoryWithProducts(int id)
        {
            var category = await _categoryService.GetCategoryWithProductsAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> SearchCategories([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var categories = await _categoryService.SearchCategoriesByNameAsync(searchTerm);
            return Ok(categories);
        }
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetCategoriesWithPagination([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and page size must be greater than 0.");
            }

            var categories = await _categoryService.GetCategoriesWithPaginationAsync(page, pageSize);
            var totalCount = await _categoryService.GetCategoryCountAsync();

            return Ok(new
            {
                categories,
                totalCount,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> GetCategoryCount()
        {
            var count = await _categoryService.GetCategoryCountAsync();
            return Ok(count);
        }
        [HttpGet("check-name")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> CheckCategoryNameUnique([FromQuery] string name, [FromQuery] int? excludeId = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Category name cannot be empty.");
            }

            bool isUnique;
            if (excludeId.HasValue)
            {
                isUnique = await _categoryService.IsCategoryNameUniqueAsync(name, excludeId.Value);
            }
            else
            {
                isUnique = await _categoryService.IsCategoryNameUniqueAsync(name);
            }

            return Ok(new { name, isUnique });
        }
    }
}
