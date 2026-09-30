using System.Data.Entity;
using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Data.Mapping;

namespace Ecommerce.Data.Repositories
{
    /// <summary>
    /// Database access boundary for products. All queries are LINQ-to-Entities;
    /// Include() is used so listing/detail never N+1.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceDbContext _db;

        public ProductRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public Product GetById(int id)
        {
            var p = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
                .Include(x => x.Variants)
                .FirstOrDefault(x => x.Id == id && x.IsActive);
            return p.ToCore();
        }

        public System.Collections.Generic.IList<Product> GetListing(int? categoryId, string q, int page, int size, out int totalCount)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 12;

            var query = _db.Products
                .Include(x => x.Images)
                .Where(x => x.IsActive);

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(x => x.Name.Contains(term) || x.Sku.Contains(term));
            }

            totalCount = query.Count();

            var items = query
                .OrderBy(x => x.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return items.Select(x => x.ToCore()).ToList();
        }

        public System.Collections.Generic.IList<Product> GetFeatured(int take)
        {
            var items = _db.Products
                .Include(x => x.Images)
                .Where(x => x.IsActive && x.IsFeatured)
                .OrderByDescending(x => x.CreatedDate)
                .Take(take)
                .ToList();

            return items.Select(x => x.ToCore()).ToList();
        }

        public System.Collections.Generic.IList<Product> GetRelated(int productId, int take)
        {
            var product = _db.Products
                .Where(x => x.Id == productId)
                .Select(x => new { x.CategoryId })
                .FirstOrDefault();
            if (product == null) return new System.Collections.Generic.List<Product>();

            var items = _db.Products
                .Include(x => x.Images)
                .Where(x => x.IsActive && x.Id != productId && x.CategoryId == product.CategoryId)
                .OrderByDescending(x => x.CreatedDate)
                .Take(take)
                .ToList();

            return items.Select(x => x.ToCore()).ToList();
        }
    }
}
