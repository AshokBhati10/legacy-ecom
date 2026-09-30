using System.Web.Mvc;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Data;
using Ecommerce.Data.Infrastructure;
using Ecommerce.Data.Repositories;
using Ecommerce.Services;
using Unity;
using Unity.Lifetime;
using Unity.Mvc5;

namespace Ecommerce.Web
{
    /// <summary>
    /// Unity DI composition root. Registrations use HierarchicalLifetimeManager:
    /// Unity.Mvc5's UnityDependencyResolver creates a child container per HTTP
    /// request (BeginScope), so this is the per-request lifetime in Unity 5.
    /// (The older PerRequestLifetimeManager API no longer exists in Unity 5.x.)
    /// Controllers receive services through constructor injection only.
    /// </summary>
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // Data boundary: one DbContext / UnitOfWork per HTTP request, via the
            // per-request child container created by UnityDependencyResolver.
            container.RegisterType<EcommerceDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());

            // Repositories
            container.RegisterType<IProductRepository, ProductRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICategoryRepository, CategoryRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICartRepository, CartRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderRepository, OrderRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICustomerRepository, CustomerRepository>(new HierarchicalLifetimeManager());

            // Services
            container.RegisterType<ICatalogService, CatalogService>(new HierarchicalLifetimeManager());
            container.RegisterType<ICartService, CartService>(new HierarchicalLifetimeManager());
            container.RegisterType<ICheckoutService, CheckoutService>(new HierarchicalLifetimeManager());
            container.RegisterType<IAccountService, AccountService>(new HierarchicalLifetimeManager());

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
