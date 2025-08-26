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
            if (dto.Price.HasValue && dto.Price <= 0)
                return BadRequest(new ApiResponse<string>(false, "Price must be greater than zero"));
            // var existingProduct = await _productService.GetByIdAsync(id);
            // if (existingProduct == null)
            //     return NotFound();

            // _mapper.Map(dto, existingProduct); // Only updates non-null values

            // await _productService.UpdateAsync(id, existingProduct);
            // return NoContent();

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
        /// Get a list of inactive products.
        /// </summary>
        /// <returns>List of inactive products</returns>
        [HttpGet("inactive")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Product>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<IEnumerable<Product>>>> GetInactiveProducts()
        {
            var inactiveProducts = await _productService.GetInactiveAsync();
            if (inactiveProducts == null || !inactiveProducts.Any())
            {
                return NotFound(new ApiResponse<string>(
                    false,
                    "No inactive products found."
                ));
            }

            return Ok(new ApiResponse<IEnumerable<Product>>(
                true,
                "Inactive products retrieved successfully",
                inactiveProducts
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


        /// <summary>
        /// Reactivate a soft-deleted (inactive) product.
        /// Only works if the product exists AND is currently inactive.
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
    }
}