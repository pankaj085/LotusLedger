// Interface/IProductService.cs

using ProductInventory.Models;

namespace ProductInventory.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync(string? category = null, string? search = null);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Guid id, Product updatedProduct);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}