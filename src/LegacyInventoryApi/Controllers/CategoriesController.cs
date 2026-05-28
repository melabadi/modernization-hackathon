using LegacyInventoryApi.Models;
using LegacyInventoryApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LegacyInventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Category>>>> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(ApiResponse<List<Category>>.Ok(categories));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Category>>> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<Category>.Fail($"Category with ID {id} not found."));

            return Ok(ApiResponse<Category>.Ok(category));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Category>>> Create([FromBody] CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(ApiResponse<Category>.Fail("Category name is required."));

            var category = new Category
            {
                Name = request.Name,
                Description = request.Description
            };

            var created = await _categoryRepository.CreateAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<Category>.Ok(created, "Category created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Category>>> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            var updated = await _categoryRepository.UpdateAsync(id, request);
            if (updated == null)
                return NotFound(ApiResponse<Category>.Fail($"Category with ID {id} not found."));

            return Ok(ApiResponse<Category>.Ok(updated, "Category updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var deleted = await _categoryRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<bool>.Fail($"Category with ID {id} not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Category deleted successfully."));
        }
    }
}
