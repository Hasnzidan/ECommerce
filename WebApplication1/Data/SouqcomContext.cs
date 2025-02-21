using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class SouqcomContext : DbContext
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
            // Configure Cart entity
            builder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.ProductId)
                    .IsRequired();

                entity.Property(e => e.Qty)
                    .IsRequired()
                    .HasDefaultValue(1);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Cart)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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
