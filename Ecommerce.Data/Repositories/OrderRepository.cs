using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Data.Mapping;

namespace Ecommerce.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceDbContext _db;

        public OrderRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public Order GetById(int id)
        {
            var o = _db.Orders
                .Include(x => x.OrderLines)
                .FirstOrDefault(x => x.Id == id);
            return o.ToCore();
        }

        public Order GetByOrderNumber(string orderNumber)
        {
            var o = _db.Orders
                .Include(x => x.OrderLines)
                .FirstOrDefault(x => x.OrderNumber == orderNumber);
            return o.ToCore();
        }

        public IList<Order> GetByUserId(string userId)
        {
            var items = _db.Orders
                .Include(x => x.OrderLines)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.OrderDate)
                .ToList();

            return items.Select(x => x.ToCore()).ToList();
        }

        public void Add(Order order)
        {
            _db.Orders.Add(order.ToData());
        }
    }
}
