# 06 — Authentication and Dependency Injection

How login works (OWIN cookie auth + ASP.NET Identity) and how objects get wired (Unity).

---

### Byte 1: OWIN cookie authentication, not Forms auth

**Builds on:** None — starting point (see `05`, Byte 3 for the Identity tables)

**In plain terms:**
Login is handled by OWIN middleware, not old-style Forms Authentication (`<authentication mode="None" />` in Web.config says exactly that). On startup, the app registers a cookie middleware that issues a 25-minute sliding-expiration auth cookie and redirects anonymous users to `/Account/Login`.

**The code:**
```csharp
public void ConfigureAuth(IAppBuilder app)
{
    app.CreatePerOwinContext(ApplicationDbContext.Create);
    app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
    app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

    app.UseCookieAuthentication(new CookieAuthenticationOptions
    {
        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        LoginPath = new PathString("/Account/Login"),
        ExpireTimeSpan = TimeSpan.FromMinutes(25),
        SlidingExpiration = true
    });
}
```

`Startup.cs` (marked with `[assembly: OwinStartup]`) calls this. The 25-minute cookie lifetime intentionally matches the 25-minute session timeout — cart and login expire together.

---

### Byte 2: ASP.NET Identity 2.0 manages the credentials

**Builds on:** Byte 1

**In plain terms:**
`ApplicationUser` (the login account) extends Identity's `IdentityUser`; `ApplicationDbContext` (Identity's EF context) uses the plain `DefaultConnection` string. The `AccountController` uses `ApplicationUserManager`/`ApplicationSignInManager` for register, login, and password flows — all standard Identity 2.0.

**The code:**
```csharp
public class ApplicationUser : IdentityUser
{
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
        UserManager<ApplicationUser> manager)
    {
        var userIdentity = await manager.CreateIdentityAsync(
            this, DefaultAuthenticationTypes.ApplicationCookie);
        return userIdentity;
    }
}
```

Identity is a separate subsystem from the shop: it knows nothing about carts or orders. The bridge is `User.Identity.GetUserId()` — controllers pass that string ID into services, which look up the `Customers`/`CartItems`/`Orders` rows belonging to it.

---

### Byte 3: Unity is the composition root, per request

**Builds on:** `01`, Byte 2

**In plain terms:**
`UnityConfig.RegisterComponents()` (called from `Application_Start`) is the one place where interfaces are mapped to implementations. Everything is registered with `HierarchicalLifetimeManager`, which — combined with Unity.Mvc5's per-request child container — means one `DbContext`/`UnitOfWork` per HTTP request, shared by all repositories in that request.

**The code:**
```csharp
// Data boundary: one DbContext / UnitOfWork per HTTP request, via the
// per-request child container created by UnityDependencyResolver.
container.RegisterType<EcommerceDbContext>(new HierarchicalLifetimeManager());
container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());
container.RegisterType<IProductRepository, ProductRepository>(new HierarchicalLifetimeManager());
// ... services ...
DependencyResolver.SetResolver(new UnityDependencyResolver(container));
```

This is why `CheckoutService` can call `_orders.Add()` and `_unitOfWork.SaveChanges()` and have it cover everything: both resolve to the same per-request context. (Note: the pinned `System.Runtime.CompilerServices.Unsafe 4.5.2` package exists because Unity 5.11.1 requires it — see `07`, Byte 4.)

---

### Byte 4: Controllers get services through constructors only

**Builds on:** Byte 3

**In plain terms:**
No controller ever calls `new CatalogService()` or touches the container. Unity inspects each controller's constructor and supplies the registered implementations. This is what makes the layers substitutable — and the controllers unit-testable with fakes.

**The code:**
```csharp
public CheckoutController(
    ICheckoutService checkout, ICartService cart, IAccountService account)
{
    _checkout = checkout;
    _cart = cart;
    _account = account;
}
```

The dependency chain per request looks like: `CheckoutController` → `CheckoutService` → (`CartService`, `IOrderRepository`, `ICustomerRepository`, `IUnitOfWork`) → `EcommerceDbContext`. Unity builds the whole graph; the code just declares what it needs.

---

### Byte 5: Two user concepts — login vs. customer

**Builds on:** Bytes 2, 4

**In plain terms:**
`ApplicationUser`/`AspNetUsers` is the *credential* (email + password hash, managed by Identity). `Customer`/`Customers` is the *shop profile* (name, phone, order history). They're linked by the string `UserId`, and `AccountService.EnsureCustomerForUser` creates the profile row on demand.

**The code:**
```csharp
public Customer EnsureCustomerForUser(string userId, string email,
    string firstName, string lastName)
{
    var existing = _customers.GetByUserId(userId);
    if (existing != null) return existing;
    // ... creates the Customers row linked to this login
}
```

Guest checkout works without either: `CheckoutService.PlaceOrder` accepts a null/empty `userId` and still records the order with the typed-in address. Login is a convenience (saved addresses, order history, persisted cart), not a requirement to buy.

---

## PUTTING IT TOGETHER

OWIN cookie middleware (25-minute sliding cookie) plus ASP.NET Identity 2.0 handle who you are; the shop's `Customers` table records who you are *to the store*, linked by user ID. Unity wires everything at startup with per-request lifetimes so one `DbContext` serves a whole request, and controllers simply declare the services they need in their constructors.
