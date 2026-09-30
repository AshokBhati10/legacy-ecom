using System.Collections.Generic;

namespace Ecommerce.Core.ViewModels
{
    public class CartLineViewModel
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public string ThumbnailUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class CartViewModel
    {
        public IList<CartLineViewModel> Items { get; set; }
        public int ItemCount { get; set; }
        public decimal SubTotal { get; set; }

        public CartViewModel()
        {
            Items = new List<CartLineViewModel>();
        }

        public bool IsEmpty
        {
            get { return Items == null || Items.Count == 0; }
        }
    }
}
