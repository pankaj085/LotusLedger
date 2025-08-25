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

        public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null || !existing.IsActive) return null;

            existing.Name = updatedProduct.Name;
            existing.Description = updatedProduct.Description;
            existing.Price = updatedProduct.Price;
            existing.StockQuantity = updatedProduct.StockQuantity;
            existing.Category = updatedProduct.Category;
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
    }
}
