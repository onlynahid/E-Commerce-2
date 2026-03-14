using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Attributes;
using AYYUAZ.APP.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AYYUAZ.APP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound(string.Format(ErrorMessages.CategoryNotFound,id));
            }
            return Ok(category);
        }
        [HttpGet("with-products")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesWithProducts()
        {
            var categories = await _categoryService.GetCategoriesWithProducts();
            return Ok(categories);
        }
        [HttpGet("{id}/with-products")]
        public async Task<ActionResult<CategoryDto>> GetCategoryWithProducts(int id)
        {
            var category = await _categoryService.GetCategoryWithProducts(id);
            if (category == null)
            {
                return NotFound(string.Format(ErrorMessages.CategoryNotFound, id));
            }
            return Ok(category);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> SearchCategories([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest(ErrorMessages.CategoryNotFound);
            }
            var categories = await _categoryService.SearchCategoriesByName(searchTerm);
            return Ok(categories);
        }
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetCategoriesWithPagination([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(ErrorMessages.CategoryNotFound);
            }
            var categories = await _categoryService.GetCategoriesWithPagination(page, pageSize);
            var totalCount = await _categoryService.GetCategoryCount();
            return Ok(new
            {
                categories,
                totalCount, 
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
    }
}