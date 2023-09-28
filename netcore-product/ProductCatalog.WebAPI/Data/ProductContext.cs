using Microsoft.EntityFrameworkCore;
using ProductCatalog.WebAPI.Models;

namespace ProductCatalog.WebAPI.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration for entities

            base.OnModelCreating(modelBuilder);
        }
    }
}
