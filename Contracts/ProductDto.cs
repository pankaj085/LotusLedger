namespace ProductInventory.Contracts
{
    /// <summary>
    /// DTO for returning product details in API response
    /// </summary>
    public class ProductDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
