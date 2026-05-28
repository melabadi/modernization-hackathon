using LegacyInventoryApi.Models;
using LegacyInventoryApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LegacyInventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<Product>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null,
            [FromQuery] bool? activeOnly = null)
        {
            var products = await _productRepository.GetAllAsync();

            if (activeOnly.HasValue && activeOnly.Value)
                products = products.Where(p => p.IsActive).ToList();

            if (!string.IsNullOrEmpty(category))
            {
                var categories = await _categoryRepository.GetAllAsync();
                var cat = categories.FirstOrDefault(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
                if (cat != null)
                    products = products.Where(p => p.CategoryId == cat.Id).ToList();
            }

            var totalCount = products.Count;
            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<Product>
            {
                Items = pagedProducts,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(ApiResponse<PagedResult<Product>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<Product>.Fail($"Product with ID {id} not found."));

            return Ok(ApiResponse<Product>.Ok(product));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<Product>>>> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(ApiResponse<List<Product>>.Fail("Search query cannot be empty."));

            var results = await _productRepository.SearchAsync(q);
            return Ok(ApiResponse<List<Product>>.Ok(results));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Product>>> Create([FromBody] CreateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(ApiResponse<Product>.Fail("Product name is required."));

            if (request.Price <= 0)
                return BadRequest(ApiResponse<Product>.Fail("Price must be greater than zero."));

            if (string.IsNullOrWhiteSpace(request.Sku))
                return BadRequest(ApiResponse<Product>.Fail("SKU is required."));

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return BadRequest(ApiResponse<Product>.Fail($"Category with ID {request.CategoryId} does not exist."));

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId,
                Sku = request.Sku
            };

            var created = await _productRepository.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<Product>.Ok(created, "Product created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> Update(int id, [FromBody] UpdateProductRequest request)
        {
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                    return BadRequest(ApiResponse<Product>.Fail($"Category with ID {request.CategoryId.Value} does not exist."));
            }

            var updated = await _productRepository.UpdateAsync(id, request);
            if (updated == null)
                return NotFound(ApiResponse<Product>.Fail($"Product with ID {id} not found."));

            return Ok(ApiResponse<Product>.Ok(updated, "Product updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var deleted = await _productRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<bool>.Fail($"Product with ID {id} not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Product deleted successfully."));
        }
    }
}
