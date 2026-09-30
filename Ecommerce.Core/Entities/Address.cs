namespace Ecommerce.Core.Entities
{
    /// <summary>
    /// Saved address belonging to a customer.
    /// </summary>
    public class Address
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Label { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public bool IsDefault { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
