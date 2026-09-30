using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Data.Mapping;

namespace Ecommerce.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EcommerceDbContext _db;

        public CustomerRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public Customer GetById(int id)
        {
            var c = _db.Customers.FirstOrDefault(x => x.Id == id);
            return c.ToCore();
        }

        public Customer GetByUserId(string userId)
        {
            var c = _db.Customers.FirstOrDefault(x => x.UserId == userId);
            return c.ToCore();
        }

        public void Add(Customer customer)
        {
            _db.Customers.Add(customer.ToData());
        }

        public void AddAddress(Address address)
        {
            _db.Addresses.Add(address.ToData());
        }
    }
}
