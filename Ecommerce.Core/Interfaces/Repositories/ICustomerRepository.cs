using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Customer GetById(int id);
        Customer GetByUserId(string userId);
        void Add(Customer customer);
        void AddAddress(Address address);
    }
}
