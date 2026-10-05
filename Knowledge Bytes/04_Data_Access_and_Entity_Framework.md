# 04 — Data Access and Entity Framework

How data gets out of SQL Server and back into plain objects.

---

### Byte 1: Repositories run the queries

**Builds on:** `01`, Byte 1

**In plain terms:**
Each repository answers questions about one area — products, categories, orders — using LINQ. This one fetches all active categories with their subcategories, in display order.

**The code:**
```csharp
var items = _db.Categories.Include(x => x.ChildCategories)
    .Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).ToList();
```

---

### Byte 2: CoreMapper translates at the boundary

**Builds on:** Byte 1

**In plain terms:**
Entity Framework tracks its own classes (`Data.Models`); the rest of the app uses clean copies (`Core.Entities`). `CoreMapper` converts between them, so EF types never leak upward.

**The code:**
```csharp
return items.Select(x => x.ToCore()).ToList();
```

---

### Byte 3: UnitOfWork is the single save button

**Builds on:** Byte 1

**In plain terms:**
Repositories change objects in memory but never save. `UnitOfWork.SaveChanges()` writes everything once — so placing an order either fully succeeds or fully fails. "The order isn't being saved" → trace whether `SaveChanges` was reached and whether it threw.

**The code:**
```csharp
_orders.Add(order);
_unitOfWork.SaveChanges();
```

---

### Byte 4: Database First — EF never builds the schema

**Builds on:** Bytes 1–3

**In plain terms:**
The database already exists; EF's mapping comes from `LegacyEcommerce.edmx`, and `Database.SetInitializer(null)` forbids EF from creating or migrating tables. Schema changes happen through the SQL scripts in `Database/`, never through code.

**The code:**
```csharp
static EcommerceDbContext()
{
    Database.SetInitializer<EcommerceDbContext>(null);  // no migrations, ever
}
```

---

## PUTTING IT TOGETHER

Repository runs a LINQ query → Entity Framework translates it to SQL → SQL Server returns rows → EF builds its entity objects → `CoreMapper` converts them to clean Core entities → the service gets plain objects. Writes go the other way and land with a single `UnitOfWork.SaveChanges()`.
