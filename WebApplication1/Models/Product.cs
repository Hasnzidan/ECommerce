using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApplication1.Models
{
    public partial class Product
    {
        public Product()
        {
            Cart = new HashSet<Cart>();
            ProductImage = new HashSet<ProductImage>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        public string Photo { get; set; }

        public string ImageUrl => string.IsNullOrEmpty(Photo) ? "/images/default-product.jpg" : Photo;

        public int? CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Cart> Cart { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }
    }
}
