namespace Ecommerce.Core.Entities
{
    /// <summary>
    /// A single line in the shopping cart. The live cart lives in
    /// HttpContext.Session; this entity is also the persisted-cart row shape.
    /// </summary>
    public class CartItem
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
    }
}
