using System;
using System.Collections.Generic;

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
            ProductImages = new HashSet<ProductImages>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int? Catid { get; set; }
        public string Photo { get; set; }
        public string Type { get; set; }
        public string SupplierName { get; set; }
        public DateTime? EntryDate { get; set; }
        public string ReviewUrl { get; set; }
        public int? Quantity { get; set; }
        public decimal? Priceafterdiscount { get; set; }

        public virtual Category Cat { get; set; }
        public virtual ICollection<Cart> Cart { get; set; }
        public virtual ICollection<ProductImages> ProductImages { get; set; }
    }
}
