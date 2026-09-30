using System.Data.Entity;
using Ecommerce.Data.Models;

namespace Ecommerce.Data
{
    /// <summary>
    /// Database First DbContext. Mapping comes from LegacyEcommerce.edmx
    /// (embedded at build via EntityDeploy); this class only exposes the
    /// entity sets. Database First: no migrations, no initializer.
    /// </summary>
    public class EcommerceDbContext : DbContext
    {
        static EcommerceDbContext()
        {
            // Database First against an existing database: never let EF
            // try to create or migrate the schema.
            Database.SetInitializer<EcommerceDbContext>(null);
        }

        public EcommerceDbContext()
            : base("name=EcommerceDb")
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderLine> OrderLines { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductVariant> ProductVariants { get; set; }
    }
}
