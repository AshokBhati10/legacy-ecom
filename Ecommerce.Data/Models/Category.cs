using System.Collections.Generic;

namespace Ecommerce.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; }
        public virtual Category ParentCategory { get; set; }

        public Category()
        {
            Products = new HashSet<Product>();
            ChildCategories = new HashSet<Category>();
        }
    }
}
