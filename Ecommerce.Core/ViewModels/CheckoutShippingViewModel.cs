using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.ViewModels
{
    public class ShippingOptionViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
    }

    public class CheckoutShippingViewModel
    {
        [Required(ErrorMessage = "Please choose a shipping method.")]
        public string ShippingMethod { get; set; }

        public IList<ShippingOptionViewModel> Options { get; set; }

        public CheckoutShippingViewModel()
        {
            Options = new List<ShippingOptionViewModel>();
        }
    }
}
