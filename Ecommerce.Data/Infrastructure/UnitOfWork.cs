using Ecommerce.Core.Interfaces;

namespace Ecommerce.Data.Infrastructure
{
    /// <summary>
    /// Unit of work over the per-request EcommerceDbContext.
    /// SaveChanges stays in the Data boundary; services commit through this.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceDbContext _context;
        private bool _disposed;

        public UnitOfWork(EcommerceDbContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}
