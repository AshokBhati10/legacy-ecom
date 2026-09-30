using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;

namespace Ecommerce.Data.Repositories
{
    /// <summary>
    /// Optional persisted cart for authenticated users.
    /// </summary>
    public class CartRepository : ICartRepository
    {
        private readonly EcommerceDbContext _db;

        public CartRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public IList<CartItem> GetByUserId(string userId)
        {
            return _db.CartItems
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.DateCreated)
                .Select(x => new CartItem
                {
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    Quantity = x.Quantity
                })
                .ToList();
        }

        public void Save(string userId, IEnumerable<CartItem> items)
        {
            var existing = _db.CartItems.Where(x => x.UserId == userId).ToList();
            _db.CartItems.RemoveRange(existing);

            var now = DateTime.UtcNow;
            foreach (var item in items ?? new List<CartItem>())
            {
                _db.CartItems.Add(new Models.CartItem
                {
                    UserId = userId,
                    ProductId = item.ProductId,
                    VariantId = item.VariantId,
                    Quantity = item.Quantity,
                    DateCreated = now
                });
            }
        }

        public void Clear(string userId)
        {
            var existing = _db.CartItems.Where(x => x.UserId == userId).ToList();
            _db.CartItems.RemoveRange(existing);
        }
    }
}
