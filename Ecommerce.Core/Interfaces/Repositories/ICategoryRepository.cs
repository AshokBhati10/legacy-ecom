using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Category GetById(int id);
        IList<Category> GetActive();
        IList<Category> GetChildren(int? parentId);
    }
}
