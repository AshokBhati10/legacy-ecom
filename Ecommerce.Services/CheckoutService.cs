using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.Services.Pricing;

namespace Ecommerce.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartService _cartService;
        private readonly IOrderRepository _orders;
        private readonly ICustomerRepository _customers;
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutService(
            ICartService cartService,
            IOrderRepository orders,
            ICustomerRepository customers,
            IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _orders = orders;
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public CheckoutSummaryViewModel BuildSummary(IEnumerable<CartItem> items, string shippingMethod)
        {
            var cart = _cartService.GetCart(items);
            var method = PriceCalculator.NormalizeShippingMethod(shippingMethod);
            var shipping = PriceCalculator.CalculateShippingCost(cart.SubTotal, method);
            var tax = PriceCalculator.CalculateTax(cart.SubTotal);

            return new CheckoutSummaryViewModel
            {
                Items = cart.Items,
                ItemCount = cart.ItemCount,
                SubTotal = cart.SubTotal,
                ShippingMethod = method,
                ShippingCost = shipping,
                TaxAmount = tax,
                Total = PriceCalculator.CalculateTotal(cart.SubTotal, shipping, tax)
            };
        }

        public IList<ShippingOptionViewModel> GetShippingOptions(IEnumerable<CartItem> items)
        {
            var cart = _cartService.GetCart(items);
            return PriceCalculator.GetShippingOptions(cart.SubTotal);
        }

        public ServiceResult<Order> PlaceOrder(
            IEnumerable<CartItem> items,
            CheckoutAddressViewModel address,
            string shippingMethod,
            string paymentMethod,
            string userId)
        {
            var cart = _cartService.GetCart(items);
            if (cart.IsEmpty)
                return ServiceResult<Order>.Fail("Your cart is empty.");

            if (address == null)
                return ServiceResult<Order>.Fail("A shipping address is required.");

            var method = PriceCalculator.NormalizeShippingMethod(shippingMethod);
            var shipping = PriceCalculator.CalculateShippingCost(cart.SubTotal, method);
            var tax = PriceCalculator.CalculateTax(cart.SubTotal);
            var total = PriceCalculator.CalculateTotal(cart.SubTotal, shipping, tax);

            Customer customer = null;
            if (!string.IsNullOrWhiteSpace(userId))
                customer = _customers.GetByUserId(userId);

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                CustomerId = customer != null ? (int?)customer.Id : null,
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Placed",
                SubTotal = cart.SubTotal,
                ShippingCost = shipping,
                TaxAmount = tax,
                Total = total,
                ShippingMethod = method,
                PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "CashOnDelivery" : paymentMethod,
                ShipFirstName = address.FirstName,
                ShipLastName = address.LastName,
                ShipEmail = address.Email,
                ShipPhone = address.Phone,
                ShipStreet = address.Street,
                ShipCity = address.City,
                ShipState = address.State,
                ShipPostalCode = address.PostalCode,
                ShipCountry = address.Country,
                OrderLines = cart.Items.Select(l => new OrderLine
                {
                    ProductId = l.ProductId,
                    VariantId = l.VariantId,
                    ProductName = l.ProductName,
                    VariantName = l.VariantName,
                    Sku = null,
                    UnitPrice = l.UnitPrice,
                    Quantity = l.Quantity,
                    LineTotal = l.LineTotal
                }).ToList()
            };

            _orders.Add(order);
            _unitOfWork.SaveChanges();

            var saved = _orders.GetByOrderNumber(order.OrderNumber);
            if (saved == null)
                return ServiceResult<Order>.Fail("The order could not be saved.");

            return ServiceResult<Order>.Ok(saved, "Order placed successfully.");
        }

        public Order GetOrder(int orderId)
        {
            return _orders.GetById(orderId);
        }

        private string GenerateOrderNumber()
        {
            // LE-YYYYMMDD-XXXXXX; retried on the (unlikely) collision.
            for (var attempt = 0; attempt < 5; attempt++)
            {
                var number = "LE-" + DateTime.UtcNow.ToString("yyyyMMdd")
                    + "-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
                if (_orders.GetByOrderNumber(number) == null)
                    return number;
            }
            return "LE-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();
        }
    }
}
