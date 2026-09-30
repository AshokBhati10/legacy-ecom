using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    /// <summary>
    /// Catalog product. Plain POCO shared by all layers; never an EF type in views.
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
        public DateTime CreatedDate { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
        public virtual ICollection<ProductVariant> Variants { get; set; }

        public Product()
        {
            Images = new List<ProductImage>();
            Variants = new List<ProductVariant>();
        }

        /// <summary>Effective selling price (sale price wins when present).</summary>
        public decimal EffectivePrice
        {
            get { return SalePrice.HasValue && SalePrice.Value > 0 ? SalePrice.Value : Price; }
        }
    }
}
