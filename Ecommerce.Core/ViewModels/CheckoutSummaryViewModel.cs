using System.Collections.Generic;

namespace Ecommerce.Core.ViewModels
{
    /// <summary>
    /// Order totals shown on the shipping/payment steps.
    /// </summary>
    public class CheckoutSummaryViewModel
    {
        public IList<CartLineViewModel> Items { get; set; }
        public int ItemCount { get; set; }
        public decimal SubTotal { get; set; }
        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }

        public CheckoutSummaryViewModel()
        {
            Items = new List<CartLineViewModel>();
        }
    }
}
