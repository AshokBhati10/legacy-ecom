using System;
using System.Collections.Generic;

namespace Ecommerce.Data.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }

        public Customer()
        {
            Orders = new HashSet<Order>();
            Addresses = new HashSet<Address>();
        }
    }
}
