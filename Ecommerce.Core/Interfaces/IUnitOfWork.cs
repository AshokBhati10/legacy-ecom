using System;

namespace Ecommerce.Core.Interfaces
{
    /// <summary>
    /// Unit-of-work boundary. SaveChanges stays in the Data layer;
    /// services call it to commit a business transaction.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
    }
}
