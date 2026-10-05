# 04 — Data Access and Entity Framework

How the app talks to SQL Server: Database First EF, repositories, and the mapper that keeps EF inside the Data project.

---

### Byte 1: Database First — the EDMX owns the mapping

**Builds on:** `01`, Byte 4

**In plain terms:**
"Database First" means the database schema already exists and Entity Framework's mapping is generated from it, stored in `LegacyEcommerce.edmx`. The code never creates or migrates the schema — that is an explicit, enforced choice.

**The code:**
```csharp
static EcommerceDbContext()
{
    // Database First against an existing database: never let EF
    // try to create or migrate the schema.
    Database.SetInitializer<EcommerceDbContext>(null);
}

public EcommerceDbContext() : base("name=EcommerceDb") { }

public virtual DbSet<Product> Products { get; set; }
public virtual DbSet<Order> Orders { get; set; }
// ... 9 DbSets total
```

`SetInitializer(null)` is the one line that makes Database First safe: without it, EF might try to create or migrate the database on first use. Schema changes are made with the SQL scripts in `Database/`, never by EF.

---

### Byte 2: Repositories are the only classes that query

**Builds on:** Byte 1

**In plain terms:**
Each repository wraps one aggregate's queries in LINQ: filtering to active records, eager-loading navigation properties with `Include`, and ordering. Services call these methods; they never write LINQ against `DbSet` themselves.

**The code:**
```csharp
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
```

Notice the final `.ToCore()` — every repository method translates EF-tracked models into Core entities before returning. The repository is also where query policy lives (e.g. "inactive products are invisible"), so that rule is enforced for every caller.

---

### Byte 3: CoreMapper is the boundary EF never crosses

**Builds on:** Byte 2 (see `01`, Byte 3 for the two entity sets)

**In plain terms:**
`CoreMapper` (internal to the Data project) converts `Ecommerce.Data.Models` objects into `Ecommerce.Core.Entities` objects. It is the single doorway between the EF world and the rest of the app — and it only opens outward for reads; writes go through separate `ToData` methods used when saving.

**The code:**
```csharp
/// Translates EDMX-mapped entities (Ecommerce.Data.Models) to the
/// framework-free Core contracts (Ecommerce.Core.Entities) at the
/// repository boundary. EF types never leave the Data project.
internal static class CoreMapper
{
    public static CoreEntities.Category ToCore(this DataModels.Category c) { ... }
    public static DataModels.Order ToData(this CoreEntities.Order o) { ... }
}
```

If you add a column to a table, the ripple is contained: update the EDMX mapping, the Data model, and the corresponding `ToCore`/`ToData` methods. Services and controllers are untouched unless they need the new field.

---

### Byte 4: UnitOfWork owns SaveChanges

**Builds on:** Byte 2

**In plain terms:**
Repositories add and modify entities, but none of them calls `SaveChanges`. That call lives in `UnitOfWork`, which wraps the single per-request `DbContext`. A service that changes several aggregates calls `SaveChanges` once, so the whole operation commits together.

**The code:**
```csharp
// CheckoutService.PlaceOrder:
_orders.Add(order);
_unitOfWork.SaveChanges();   // one commit for the whole order
```

The `UnitOfWork` is registered per HTTP request in Unity (see `06`), so every repository injected into a service shares the same `DbContext` instance within a request. That shared context is what makes the single `SaveChanges` cover everything the request changed.

---

### Byte 5: The connection string carries the EDMX metadata

**Builds on:** Byte 1

**In plain terms:**
The `EcommerceDb` connection string doesn't just point at SQL Server — it embeds the paths to the three EF mapping documents (conceptual, storage, mapping) that the build extracts from the EDMX. EF loads them as assembly resources at runtime.

**The code:**
```xml
<add name="EcommerceDb"
     connectionString="metadata=res://*/LegacyEcommerce.csdl|res://*/LegacyEcommerce.ssdl|res://*/LegacyEcommerce.msl;provider=System.Data.SqlClient;provider connection string=&quot;Server=.\SQLEXPRESS;Database=LegacyEcommerceDb;...&quot;"
     providerName="System.Data.EntityClient" />
```

`res://*/LegacyEcommerce.csdl` means "find the embedded resource named `LegacyEcommerce.csdl` in a loaded assembly." If those resources are missing from `Ecommerce.Data.dll`, EF throws `MetadataException` on first use — which is exactly the Linux build problem documented in `07`.

---

## PUTTING IT TOGETHER

`EcommerceDbContext` exposes the tables as `DbSet`s but creates nothing (`SetInitializer(null)`). Repositories run the LINQ queries and translate results to Core entities through `CoreMapper`, so EF types never escape the Data project. `UnitOfWork.SaveChanges` commits once per request over the shared per-request context. And the whole mapping rides into the app as embedded resources named by the `EcommerceDb` connection string.
