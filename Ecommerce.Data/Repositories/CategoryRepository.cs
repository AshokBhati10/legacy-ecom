using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Data.Mapping;

namespace Ecommerce.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EcommerceDbContext _db;

        public CategoryRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public Category GetById(int id)
        {
            var c = _db.Categories
                .Include(x => x.ChildCategories)
                .FirstOrDefault(x => x.Id == id && x.IsActive);
            return c.ToCore();
        }

        public IList<Category> GetActive()
        {
            var items = _db.Categories
                .Include(x => x.ChildCategories)
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .ToList();

            return items.Select(x => x.ToCore()).ToList();
        }
    }
}
