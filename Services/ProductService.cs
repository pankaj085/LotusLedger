// Services/ProductService.cs

using ProductInventory.Contracts; 
using Microsoft.EntityFrameworkCore;
using ProductInventory.AppDataContext;
using ProductInventory.Interface;
using ProductInventory.Models;

namespace ProductInventory.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        // Create a new product and save it to the database
        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // Get all active products with optional filtering, search, and pagination
        public async Task<IEnumerable<Product>> GetAllAsync(string? category, string? search, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Products
                .Where(p => p.IsActive); // Only fetch active products

            // Filter by category if provided
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category.ToLower() == category.ToLower());

            // Search by name or description if provided
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()) || 
                                        p.Description.ToLower().Contains(search.ToLower()));

            // Apply pagination
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Get a specific active product by ID
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        // Get a specific inactive product by ID
        public async Task<Product?> GetInactiveByIdAsync(Guid id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsActive);
        }

        // Partially update an active product with provided details
        public async Task<Product?> PatchUpdateAsync(Guid id, UpdateProductDto dto)
        {
            var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            if (existing == null) return null;

            // Update only the fields that are provided
            if (!string.IsNullOrWhiteSpace(dto.Name)) existing.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description)) existing.Description = dto.Description;
            if (dto.Price.HasValue) existing.Price = dto.Price.Value;
            if (dto.StockQuantity.HasValue) existing.StockQuantity = dto.StockQuantity.Value;
            if (!string.IsNullOrWhiteSpace(dto.Category)) existing.Category = dto.Category;

            existing.UpdatedAt = DateTime.UtcNow; // Update the timestamp

            await _context.SaveChangesAsync();
            return existing;
        }

        // Fully update an active product with new details
        public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct)
        {
            var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            if (existing == null) return null;

            // Replace existing fields with new values
            existing.Name = updatedProduct.Name ?? existing.Name;
            existing.Description = updatedProduct.Description ?? existing.Description;
            if (updatedProduct.Price != default) existing.Price = updatedProduct.Price;
            if (updatedProduct.StockQuantity != default) existing.StockQuantity = updatedProduct.StockQuantity;
            existing.Category = updatedProduct.Category ?? existing.Category;
            existing.UpdatedAt = DateTime.UtcNow; // Update the timestamp

            await _context.SaveChangesAsync();
            return existing;
        }

        // Get all active products with low stock, with optional filtering and pagination
        public async Task<IEnumerable<Product>> GetLowStockAsync(
            string? category = null,
            string? search = null,
            int threshold = 25,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Products
                .Where(p => p.IsActive && p.StockQuantity < threshold); // Filter by low stock

            // Filter by category if provided
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category.ToLower() == category.ToLower());

            // Search by name or description if provided
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()) ||
                                        p.Description.ToLower().Contains(search.ToLower()));

            // Apply pagination
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Soft delete a product by marking it as inactive
        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return false;

            product.IsActive = false; // Mark as inactive
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all inactive products with optional filtering and pagination
        public async Task<IEnumerable<Product>> GetInactiveAsync(string? category = null, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Products.Where(p => !p.IsActive); // Only fetch inactive products

            // Filter by category if provided
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category.ToLower() == category.ToLower());

            // Search by name or description if provided
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()) ||
                                        p.Description.ToLower().Contains(search.ToLower()));

            // Apply pagination
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Permanently delete a product from the database
        public async Task<bool> DeleteProductPermanentlyAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsActive)
            {
                return false; // Product not found or is still active
            }

            _context.Products.Remove(product); // Remove from database
            await _context.SaveChangesAsync();
            return true; // Deletion successful
        }

        // Reactivate an inactive product
        public async Task<Product?> ReactivateAsync(Guid id)
        {
            var product = await GetInactiveByIdAsync(id); // Fetch inactive product
            if (product == null) return null;

            product.IsActive = true; // Mark as active
            product.UpdatedAt = DateTime.UtcNow; // Update the timestamp
            await _context.SaveChangesAsync();
            return product;
        }

    }
}
