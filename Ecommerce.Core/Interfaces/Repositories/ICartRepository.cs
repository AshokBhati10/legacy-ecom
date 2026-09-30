using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    /// <summary>
    /// Optional persisted cart for authenticated users.
    /// The live cart is session-based; this is the durable backing store.
    /// </summary>
    public interface ICartRepository
    {
        IList<CartItem> GetByUserId(string userId);
        void Save(string userId, IEnumerable<CartItem> items);
        void Clear(string userId);
    }
}
