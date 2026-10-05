# 01 — System Architecture

One mental picture for the whole app: every click travels down through layers to the database, and data travels back up.

---

### Byte 1: The journey of every click

**Builds on:** None — starting point

**In plain terms:**
No matter what you click, the path is always the same. The request goes down: Controller → Service → Repository → Entity Framework → SQL Server. The answer comes back up through the same layers.

**The code:**
```text
YOU → Controller → Service → Repository → Entity Framework → SQL Server
                                              (data comes back up the same way)
```

If you memorize this one picture, you can trace any feature in the app. Each layer has exactly one job, which the next bytes explain.

---

### Byte 2: Four projects, four jobs

**Builds on:** Byte 1

**In plain terms:**
The code is split into four projects. Web talks to the user, Services makes decisions, Data talks to the database, and Core holds the shared contracts they all agree on.

**The code:**
```text
Ecommerce.Web       → pages, controllers, login, session cart
Ecommerce.Services  → business logic: pricing, cart math, orders
Ecommerce.Data      → Entity Framework, repositories, database mapping
Ecommerce.Core      → interfaces, entities, view models (no logic)
```

Services never touch the database directly and never see Entity Framework — they only call repository *interfaces* defined in Core. That is what keeps the layers separate.

---

### Byte 3: Core is the shared vocabulary

**Builds on:** Byte 2

**In plain terms:**
Core contains no logic — just the interfaces (like `ICatalogService`, `IProductRepository`) and the plain data objects passed between layers. Every layer references Core, so they can talk to each other without knowing concrete classes.

**The code:**
```csharp
// A controller only knows the interface — never the real class.
public HomeController(ICatalogService catalog)
{
    _catalog = catalog;
}
```

Unity (the dependency injection container) plugs the real implementations in at startup. This is why you can follow any flow by reading interfaces first.

---

## PUTTING IT TOGETHER

Think of the app as a sandwich: Web on top (user), Services in the middle (decisions), Data at the bottom (database), with Core as the language they share. A click travels down through these layers to SQL Server, and the data travels back up. The rest of this guide walks you through what each layer actually does.
