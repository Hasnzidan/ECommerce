using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class SouqcomContext : IdentityDbContext
    {
        public SouqcomContext(DbContextOptions<SouqcomContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
                new Category { Id = 2, Name = "Clothing", Description = "Fashion and apparel" },
                new Category { Id = 3, Name = "Books", Description = "Books and publications" }
            );

            // Configure decimal precision for Product prices
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            
            builder.Entity<Product>()
                .Property(p => p.OldPrice)
                .HasPrecision(18, 2);
        }
    }
}
