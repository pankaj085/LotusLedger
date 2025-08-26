// Controllers/ProductController.cs

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
        /// <param name="pageNumber">Page number for pagination (optional, default is 1)</param>
        /// <param name="pageSize">Number of items per page (optional, default is 10)</param>
        /// <returns>List of products</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? category, 
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)   // default 10 items per page
        {
            var products = await _productService.GetAllAsync(category, search, pageNumber, pageSize);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(new ApiResponse<IEnumerable<ProductDto>>(
                true, 
                "Products fetched successfully", 
                productDtos
            ));
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
        /// Partially update an existing product (PATCH).
        /// Only provided fields will be updated.
        /// </summary>
        /// <param name="id">Product ID (Guid)</param>
        /// <param name="dto">Fields to update</param>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<IActionResult> PatchUpdate(Guid id, [FromBody] UpdateProductDto dto)
        {
            if (dto.Price.HasValue && dto.Price <= 0)
                return BadRequest(new ApiResponse<string>(false, "Price must be greater than 0."));

            if (dto.StockQuantity.HasValue && dto.StockQuantity < 0)
                return BadRequest(new ApiResponse<string>(false, "StockQuantity cannot be negative."));

            var updatedProduct = await _productService.PatchUpdateAsync(id, dto);

            if (updatedProduct == null)
                return NotFound(new ApiResponse<string>(false, "Product not found."));

            return Ok(new ApiResponse<ProductDto>(
                true,
                "Product updated successfully.",
                _mapper.Map<ProductDto>(updatedProduct)
            ));
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
            if (dto.Price.HasValue && dto.Price <= 0)
                return BadRequest(new ApiResponse<string>(false, "Price must be greater than zero"));

            var existingProduct = await _productService.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new ApiResponse<string>(
                    false,
                    $"Product with ID {id} not found."
                ));
            }

            _mapper.Map(dto, existingProduct); // Only updates non-null values

            await _productService.UpdateAsync(id, existingProduct);

            return Ok(new ApiResponse<Product>(
                true,
                $"Product with ID {id} updated successfully.",
                existingProduct
            ));
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

        /// <summary>
        /// Get active products with low stock, with optional category and search filtering.
        /// </summary>
        /// <param name="category">Optional category filter</param>
        /// <param name="search">Optional search by name or description</param>
        /// <param name="threshold">Stock threshold (default 25)</param>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10)</param>
        [HttpGet("low-stock")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<IActionResult> GetLowStock(
            [FromQuery] string? category = null,
            [FromQuery] string? search = null,
            [FromQuery] int threshold = 25,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var lowStockProducts = await _productService.GetLowStockAsync(category, search, threshold, pageNumber, pageSize);

            if (!lowStockProducts.Any())
                return NotFound(new ApiResponse<string>(false, "No low-stock products found."));

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(lowStockProducts);

            return Ok(new ApiResponse<IEnumerable<ProductDto>>(
                true,
                $"Low-stock products (stock < {threshold}) retrieved successfully.",
                productDtos
            ));
        }


        /// <summary>
        /// Get a list of inactive products with optional filtering.
        /// </summary>
        /// <param name="category">Filter by category (optional)</param>
        /// <param name="search">Search by name or description (optional)</param>
        /// <param name="pageNumber">Page number for pagination (default 1)</param>
        /// <param name="pageSize">Page size for pagination (default 10)</param>
        [HttpGet("inactive")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<IActionResult> GetInactive(
            [FromQuery] string? category = null,
            [FromQuery] string? search = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var inactiveProducts = await _productService.GetInactiveAsync(category, search, pageNumber, pageSize);

            if (!inactiveProducts.Any())
                return NotFound(new ApiResponse<string>(false, "No inactive products found."));

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(inactiveProducts);

            return Ok(new ApiResponse<IEnumerable<ProductDto>>(
                true,
                "Inactive products retrieved successfully",
                productDtos
            ));
        }

        /// <summary>
        /// Reactivate a soft-deleted (inactive) product.
        /// Only works if the product exists and is currently inactive.
        /// </summary>
        /// <param name="id">Product ID (Guid)</param>
        [HttpPut("{id:guid}/reactivate")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<IActionResult> Reactivate(Guid id)
        {
            var reactivated = await _productService.ReactivateAsync(id);
            if (reactivated == null)
            {
                // We explicitly searched ONLY inactive; not found means either:
                // - product doesn't exist, or
                // - product is already active.
                return NotFound(new ApiResponse<string>(
                    false,
                    "Product not found or already active."
                ));
            }

            return Ok(new ApiResponse<ProductDto>(
                true,
                "Product reactivated successfully.",
                _mapper.Map<ProductDto>(reactivated)
            ));
        }

        /// <summary>
        /// Permanently delete a product from the inventory.
        /// Only inactive products can be deleted permanently.
        /// </summary>
        /// <param name="id">Product ID (Guid)</param>
        /// <returns>API response indicating the result of the operation</returns>
        [HttpDelete("delete/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 204)] // No Content
        [ProducesResponseType(typeof(ApiResponse<string>), 404)] // Not Found
        [ProducesResponseType(typeof(ApiResponse<string>), 400)] // Bad Request
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductPermanentlyAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<string>(
                    false,
                    "Only inactive products can be deleted, or product not found."
                ));
            }

            return Ok(new ApiResponse<string>(
                true,
                "Product deleted permanently."
            ));
        }
    }
}