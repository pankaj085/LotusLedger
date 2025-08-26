// Interface/IProductService.cs

using ProductInventory.Models;
using ProductInventory.Contracts;

namespace ProductInventory.Interface
{
    public interface IProductService
    {
        // General GET methods for retrieving products
        Task<IEnumerable<Product>> GetAllAsync(
            string? category,
            string? search,
            int pageNumber = 1,
            int pageSize = 10);
        Task<IEnumerable<Product>> GetLowStockAsync(
            string? category = null,
            string? search = null,
            int threshold = 25,
            int pageNumber = 1,
            int pageSize = 10);
        Task<IEnumerable<Product>> GetInactiveAsync(
            string? category = null,
            string? search = null,
            int pageNumber = 1,
            int pageSize = 10);
        Task<Product?> GetByIdAsync(Guid id); // for active products
        Task<Product?> GetInactiveByIdAsync(Guid id); // for inactive products

        // POST method for creating a new product
        Task<Product> CreateAsync(Product product);

        // PUT and PATCH methods for updating existing products
        Task<Product?> UpdateAsync(Guid id, Product updatedProduct);
        Task<Product?> PatchUpdateAsync(Guid id, UpdateProductDto dto);

        // DELETE methods for removing or deactivating products
        Task<bool> SoftDeleteAsync(Guid id);
        Task<bool> DeleteProductPermanentlyAsync(Guid id);

        // PUT method for reactivating a product
        Task<Product?> ReactivateAsync(Guid id);
    }
}
