using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    /// <summary>
    /// Store customer profile linked to an ASP.NET Identity user (UserId).
    /// </summary>
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
            Orders = new List<Order>();
            Addresses = new List<Address>();
        }

        public string FullName
        {
            get { return (FirstName + " " + LastName).Trim(); }
        }
    }
}
