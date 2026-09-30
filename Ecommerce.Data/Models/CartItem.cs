namespace Ecommerce.Data.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public System.DateTime DateCreated { get; set; }
    }
}
