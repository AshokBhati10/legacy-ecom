using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Product GetById(int id);
        IList<Product> GetListing(int? categoryId, string q, int page, int size, out int totalCount);
        IList<Product> GetFeatured(int take);
        IList<Product> GetRelated(int productId, int take);
    }
}
