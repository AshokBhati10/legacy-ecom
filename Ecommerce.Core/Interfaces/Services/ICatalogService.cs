using System.Collections.Generic;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface ICatalogService
    {
        ProductListViewModel GetListing(int? categoryId, string q, int page, int pageSize);
        ProductDetailViewModel GetDetail(int productId);
        IList<CategoryViewModel> GetCategories();
        IList<CategoryViewModel> GetCategoryTree(int? parentId);
    }
}
