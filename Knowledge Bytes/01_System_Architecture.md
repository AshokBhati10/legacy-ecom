# 01 — System Architecture

How the four projects fit together and why the boundaries exist.

---

### Byte 1: Four projects, one direction of dependency

**Builds on:** None — starting point

**In plain terms:**
The solution is split into four class libraries plus the web app. Dependencies only ever point inward: Web depends on everything, Services and Data both depend on Core, and Core depends on nothing in the solution.

**The code:**
```text
Ecommerce.Web       -> Core, Data, Services, EF, Identity, OWIN
Ecommerce.Services  -> Core
Ecommerce.Data      -> Core, EntityFramework
Ecommerce.Core      -> (framework assemblies only)
```

This is a classic layered (onion-style) layout. The rule is simple: nothing in the inner layers knows about the outer ones. `Ecommerce.Services` never references `Ecommerce.Data` — it only sees repository *interfaces* declared in Core. That means you can swap the data layer without touching business logic.

---

### Byte 2: Ecommerce.Core is the contract layer

**Builds on:** Byte 1

**In plain terms:**
Core contains no logic and no framework dependencies. It holds the interfaces every layer programs against, plus the plain data shapes (entities and view models) passed between layers.

**The code:**
```text
Ecommerce.Core/
  Entities/      Product, Category, Order, CartItem, Customer, ...
  ViewModels/    ProductDetailViewModel, CartViewModel, CheckoutSummaryViewModel, ...
  Interfaces/
    Repositories/  IProductRepository, ICategoryRepository, IOrderRepository, ...
    Services/      ICatalogService, ICartService, ICheckoutService, IAccountService
    IUnitOfWork.cs
  Common/        ServiceResult.cs, PagedResult.cs
```

Because interfaces live here, `Ecommerce.Web` can construct a `HomeController` that only knows `ICatalogService`, and the real `CatalogService` gets wired in at startup by the DI container. Core is the vocabulary the layers share.

---

### Byte 3: Two parallel sets of entity classes

**Builds on:** Byte 2

**In plain terms:**
There are two `Category` classes: `Ecommerce.Data.Models.Category` (what Entity Framework tracks) and `Ecommerce.Core.Entities.Category` (what the rest of the app uses). They look almost identical, but they serve different masters.

**The code:**
```csharp
// Ecommerce.Data.Models — the EF-tracked shape
namespace Ecommerce.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // ... navigation properties EF understands
    }
}
```

The Data models are allowed to be EF-flavored (virtual navigation properties, `HashSet` collections). The Core entities are framework-free plain objects. The translation happens in exactly one place — `CoreMapper` in the Data project — so EF types never leak into Services or Web. This is deliberate: the business layer stays testable and persistence-ignorant.

---

### Byte 4: Ecommerce.Data owns all database knowledge

**Builds on:** Bytes 1, 3

**In plain terms:**
Everything that touches the database lives in `Ecommerce.Data`: the `EcommerceDbContext`, one repository class per aggregate, the `UnitOfWork`, and the mapper from Byte 3. No other project contains a SQL string or a LINQ-to-Entities query.

**The code:**
```text
Ecommerce.Data/
  EcommerceDbContext.cs        # Database First DbContext (9 DbSets)
  Repositories/                # ProductRepository, CategoryRepository, ...
  Infrastructure/UnitOfWork.cs # SaveChanges lives here
  Mapping/CoreMapper.cs        # Data.Models -> Core.Entities translation
  LegacyEcommerce.edmx         # the EF Database First model definition
```

If you need to know *how* products are fetched (eager loading, filters), you look here and nowhere else. Services ask for `IProductRepository` and get Core entities back — they never know EF exists.

---

### Byte 5: ViewModels are the Web layer's contract with Services

**Builds on:** Byte 2

**In plain terms:**
Controllers never hand raw entities to views. Services assemble purpose-built `*ViewModel` classes (e.g. `ProductDetailViewModel` bundles a product plus its images, variants, and related products) so each view gets exactly what it needs in one object.

**The code:**
```csharp
// HomeController — thin: ask the service, hand the result to the view
public ActionResult Index()
{
    ViewBag.Categories = _catalog.GetCategories();
    var listing = _catalog.GetListing(null, null, 1, 8);
    return View(listing);   // listing is a ProductListViewModel
}
```

This keeps Razor views dumb: no database calls, no business math, just rendering. All shaping of data happens in Services before the view ever sees it.

---

## PUTTING IT TOGETHER

A request enters `Ecommerce.Web`, where a controller asks a service interface for a ViewModel. The service (in `Ecommerce.Services`) applies business rules and calls repository interfaces. The repository (in `Ecommerce.Data`) runs EF queries, translates results to Core entities via `CoreMapper`, and returns them. `Ecommerce.Core` supplies the shared contracts that make this possible without any layer referencing a concrete class from another. The web app is the only place that knows how the pieces are wired together — via Unity at startup.
