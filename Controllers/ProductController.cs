using Microsoft.AspNetCore.Mvc;
using ProductInventory.Interface;
using ProductInventory.Contracts;
using ProductInventory.Models;
using AutoMapper;

namespace ProductInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products with optional filtering and search.
        /// </summary>
        /// <param name="category">Filter by category (optional)</param>
        /// <param name="search">Search by name or description (optional)</param>
        /// <returns>List of products</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] string? category, [FromQuery] string? search)
        {
            var products = await _productService.GetAllAsync(category, search);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(new ApiResponse<IEnumerable<ProductDto>>(true, "Products fetched successfully", productDtos));
        }

        /// <summary>
        /// Get a specific product by ID.
        /// </summary>
        /// <param name="id">Product ID (Guid)</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse<string>(false, "Product not found"));

            return Ok(new ApiResponse<ProductDto>(true, "Product fetched", _mapper.Map<ProductDto>(product)));
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="dto">Product creation details</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0 || dto.StockQuantity < 0)
                return BadRequest(new ApiResponse<string>(false, "Invalid product input"));

            var product = _mapper.Map<Product>(dto);
            var createdProduct = await _productService.CreateAsync(product);

            var response = new ApiResponse<ProductDto>(
                true,
                "Product created successfully",
                _mapper.Map<ProductDto>(createdProduct)
            );

            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, response);
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id">Product ID (Guid)</param>
        /// <param name="dto">Updated product details</param>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            if (dto.Price <= 0 || dto.StockQuantity < 0)
                return BadRequest(new ApiResponse<string>(false, "Invalid product input"));

            var updatedEntity = _mapper.Map<Product>(dto);
            var updatedProduct = await _productService.UpdateAsync(id, updatedEntity);

            if (updatedProduct == null)
                return NotFound(new ApiResponse<string>(false, "Product not found"));

            return Ok(new ApiResponse<ProductDto>(true, "Product updated", _mapper.Map<ProductDto>(updatedProduct)));
        }

        /// <summary>
        /// Soft delete a product (mark as inactive).
        /// </summary>
        /// <param name="id">Product ID (Guid)</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.SoftDeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponse<string>(false, "Product not found"));

            return Ok(new ApiResponse<string>(true, "Product deleted successfully"));
        }
    }
}