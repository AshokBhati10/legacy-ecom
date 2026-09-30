using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.ViewModels
{
    public class CheckoutAddressViewModel
    {
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        [StringLength(30)]
        public string Phone { get; set; }

        [Required, StringLength(250)]
        public string Street { get; set; }

        [Required, StringLength(100)]
        public string City { get; set; }

        [Required, StringLength(100)]
        public string State { get; set; }

        [Required, StringLength(20)]
        public string PostalCode { get; set; }

        [Required, StringLength(100)]
        public string Country { get; set; }
    }
}
