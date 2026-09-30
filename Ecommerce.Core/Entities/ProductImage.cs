namespace Ecommerce.Core.Entities
{
    /// <summary>
    /// Image belonging to a product (gallery / Fancybox).
    /// </summary>
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; }
        public string AltText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMain { get; set; }

        public virtual Product Product { get; set; }
    }
}
