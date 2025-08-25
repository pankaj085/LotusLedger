// AppDataContext/AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using ProductInventory.Models;


namespace ProductInventory.AppDataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
        }
    }
}
