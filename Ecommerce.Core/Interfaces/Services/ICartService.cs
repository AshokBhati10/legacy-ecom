using System.Collections.Generic;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    /// <summary>
    /// Cart business logic. Operates on plain cart-item lists; the Web layer
    /// owns the HttpContext.Session storage. No HttpContext here.
    /// </summary>
    public interface ICartService
    {
        CartViewModel GetCart(IEnumerable<CartItem> items);
        MiniCartViewModel GetMiniCart(IEnumerable<CartItem> items);
        ServiceResult<IEnumerable<CartItem>> AddItem(IEnumerable<CartItem> items, int productId, int? variantId, int quantity);
        ServiceResult<IEnumerable<CartItem>> UpdateItem(IEnumerable<CartItem> items, int productId, int? variantId, int quantity);
        ServiceResult<IEnumerable<CartItem>> RemoveItem(IEnumerable<CartItem> items, int productId, int? variantId);
        IEnumerable<CartItem> ClearCart();
        void SaveForUser(string userId, IEnumerable<CartItem> items);
        IList<CartItem> LoadForUser(string userId);
    }
}
