using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface IAccountService
    {
        OrderHistoryViewModel GetOrderHistory(string userId);
        Customer EnsureCustomerForUser(string userId, string email, string firstName, string lastName);
        Customer GetCustomerByUserId(string userId);
    }
}
