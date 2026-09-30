namespace Ecommerce.Core.Entities
{
    /// <summary>
    /// Sellable variant of a product (e.g. size / colour). Adjusts the base price.
    /// </summary>
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal PriceAdjustment { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        public virtual Product Product { get; set; }
    }
}
