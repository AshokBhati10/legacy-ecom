namespace Ecommerce.Core.ViewModels
{
    /// <summary>
    /// Small view model for the reusable product tile (_ProductCard.cshtml).
    /// Views bind to this, never to the Product entity.
    /// </summary>
    public class ProductCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ThumbnailUrl { get; set; }

        public decimal DisplayPrice
        {
            get { return SalePrice.HasValue && SalePrice.Value > 0 ? SalePrice.Value : Price; }
        }
    }
}
