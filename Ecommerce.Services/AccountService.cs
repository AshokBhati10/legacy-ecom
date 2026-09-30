using System;
using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Services
{
    public class AccountService : IAccountService
    {
        private readonly IOrderRepository _orders;
        private readonly ICustomerRepository _customers;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(
            IOrderRepository orders,
            ICustomerRepository customers,
            IUnitOfWork unitOfWork)
        {
            _orders = orders;
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public OrderHistoryViewModel GetOrderHistory(string userId)
        {
            var vm = new OrderHistoryViewModel();
            if (string.IsNullOrWhiteSpace(userId)) return vm;

            vm.Orders = _orders.GetByUserId(userId)
                .Select(o => new OrderSummaryViewModel
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    ItemCount = o.OrderLines.Sum(l => l.Quantity),
                    Total = o.Total
                })
                .ToList();

            return vm;
        }

        public Customer EnsureCustomerForUser(string userId, string email, string firstName, string lastName)
        {
            var existing = _customers.GetByUserId(userId);
            if (existing != null) return existing;

            var customer = new Customer
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedDate = DateTime.UtcNow
            };

            _customers.Add(customer);
            _unitOfWork.SaveChanges();

            return _customers.GetByUserId(userId);
        }

        public Customer GetCustomerByUserId(string userId)
        {
            return _customers.GetByUserId(userId);
        }
    }
}
