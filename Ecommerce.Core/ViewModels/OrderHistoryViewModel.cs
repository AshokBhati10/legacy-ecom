using System;
using System.Collections.Generic;

namespace Ecommerce.Core.ViewModels
{
    public class OrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public IList<OrderSummaryViewModel> Orders { get; set; }

        public OrderHistoryViewModel()
        {
            Orders = new List<OrderSummaryViewModel>();
        }
    }
}
