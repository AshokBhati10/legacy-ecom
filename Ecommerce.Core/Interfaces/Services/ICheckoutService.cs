using System.Collections.Generic;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface ICheckoutService
    {
        CheckoutSummaryViewModel BuildSummary(IEnumerable<CartItem> items, string shippingMethod);
        IList<ShippingOptionViewModel> GetShippingOptions(IEnumerable<CartItem> items);
        ServiceResult<Order> PlaceOrder(
            IEnumerable<CartItem> items,
            CheckoutAddressViewModel address,
            string shippingMethod,
            string paymentMethod,
            string userId);
        Order GetOrder(int orderId);
    }
}
