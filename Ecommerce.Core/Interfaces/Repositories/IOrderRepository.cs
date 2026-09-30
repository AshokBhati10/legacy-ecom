using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Order GetById(int id);
        Order GetByOrderNumber(string orderNumber);
        IList<Order> GetByUserId(string userId);
        void Add(Order order);
    }
}
