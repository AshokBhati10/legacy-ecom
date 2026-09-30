using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.ViewModels
{
    public class ProductListViewModel
    {
        public IList<Product> Products { get; set; }
        public IList<Category> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string SearchQuery { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public ProductListViewModel()
        {
            Products = new List<Product>();
            Categories = new List<Category>();
            Page = 1;
            PageSize = 12;
        }
    }
}
