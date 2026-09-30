using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    /// <summary>
    /// Placed order. Created by CheckoutService.PlaceOrder via the Order repository.
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int? CustomerId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }

        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }

        public string ShippingMethod { get; set; }
        public string PaymentMethod { get; set; }

        public string ShipFirstName { get; set; }
        public string ShipLastName { get; set; }
        public string ShipEmail { get; set; }
        public string ShipPhone { get; set; }
        public string ShipStreet { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        public Order()
        {
            OrderLines = new List<OrderLine>();
        }
    }
}
