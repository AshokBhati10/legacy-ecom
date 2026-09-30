namespace Ecommerce.Core.Entities
{
    /// <summary>
    /// Snapshot line of an order. Product/variant names and prices are copied
    /// at order time so history survives later catalog changes.
    /// </summary>
    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
