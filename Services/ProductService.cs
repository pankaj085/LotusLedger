// Services/ProductService.cs

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

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? category = null, string? search = null)
        {
            var query = _context.Products
                .Where(p => p.IsActive); // only active products

            // Filter by category if provided
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => EF.Functions.ILike(p.Category, $"%{category}%"));
            }

            // Search by name or description (case-insensitive)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    EF.Functions.ILike(p.Name, $"%{search}%") ||
                    EF.Functions.ILike(p.Description, $"%{search}%"));
            }

            // Optional: sort by price ascending
            query = query.OrderBy(p => p.Price);

            return await query.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Product?> GetInactiveByIdAsync(Guid id)
        {
            // only for inactive
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsActive);
        }

        public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct)
        {
            var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            if (existing == null) return null;

            existing.Name = updatedProduct.Name ?? existing.Name;
            existing.Description = updatedProduct.Description ?? existing.Description;
            if (updatedProduct.Price != default) existing.Price = updatedProduct.Price;
            if (updatedProduct.StockQuantity != default) existing.StockQuantity = updatedProduct.StockQuantity;
            existing.Category = updatedProduct.Category ?? existing.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return false;

            product.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetInactiveAsync()
        {
            return await _context.Products
                .Where(p => !p.IsActive)
                .ToListAsync();
        }

        public async Task<bool> DeleteProductPermanentlyAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsActive)
            {
                return false; // Product not found or is still active
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true; // Deletion successful
        }


        public async Task<Product?> ReactivateAsync(Guid id)
        {
            // ONLY reactivate if currently INACTIVE
            var product = await GetInactiveByIdAsync(id);
            if (product == null) return null;

            product.IsActive = true;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return product;
        }

    }
}
