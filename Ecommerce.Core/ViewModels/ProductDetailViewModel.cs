using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public IList<ProductImage> Images { get; set; }
        public IList<ProductVariant> Variants { get; set; }
        public IList<ProductCardViewModel> RelatedProducts { get; set; }
        public int? SelectedVariantId { get; set; }

        public ProductDetailViewModel()
        {
            Images = new List<ProductImage>();
            Variants = new List<ProductVariant>();
            RelatedProducts = new List<ProductCardViewModel>();
        }
    }
}
