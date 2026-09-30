using System.Collections.Generic;

namespace Ecommerce.Data.Models
{
    /// <summary>
    /// Database First POCO mapped by LegacyEcommerce.edmx (conceptual model).
    /// Property names/types must match the EDMX conceptual model exactly.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int CategoryId { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int StockQuantity { get; set; }
        public System.DateTime CreatedDate { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
        public virtual ICollection<ProductVariant> Variants { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        public Product()
        {
            Images = new HashSet<ProductImage>();
            Variants = new HashSet<ProductVariant>();
            OrderLines = new HashSet<OrderLine>();
        }
    }
}
