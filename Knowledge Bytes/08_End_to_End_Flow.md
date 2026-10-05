# 08 — End-to-End Flow

Follow a click through the whole app, and learn where to look when something breaks.

---

### Byte 1: You open the homepage

**Builds on:** `01`–`04`

**In plain terms:**
You open `/`. The route picks `HomeController.Index`, which asks `CatalogService` for categories and products. The service asks the repositories, which query SQL Server through Entity Framework. The data travels back up, gets packed into a view model, and the view renders it.

**The code:**
```text
Browser → HomeController → CatalogService → CategoryRepository
    → Entity Framework → SQL Server (Categories, Products tables)
... back up ...
SQL Server → EF → Repository → CoreMapper → Service → ViewModel
    → Razor view → HTML → Browser
```

---

### Byte 2: You add something to the cart

**Builds on:** Byte 1

**In plain terms:**
You click "Add to cart". `CartController` takes your session cart (a simple list in server memory), `CartService` validates and reprices it, and the controller saves the new list back to the session. If you're logged in, a copy is also saved to the `CartItems` table so it survives session expiry.

**The code:**
```text
Where is the cart?  → server session (memory), 25-minute timeout
Logged in as well?  → plus the CartItems table in SQL Server
```

---

### Byte 3: You place an order

**Builds on:** Bytes 1, 2

**In plain terms:**
You finish the checkout wizard (address → shipping → payment). `CheckoutService` recomputes all totals from live product data, snapshots names and prices into a new `Order`, and `UnitOfWork.SaveChanges()` writes the order and its lines to SQL Server in one go. Your cart is then cleared.

**The code:**
```text
CheckoutController → CheckoutService.PlaceOrder → OrderRepository
    → UnitOfWork.SaveChanges() → Orders + OrderLines tables
```

---

### Byte 4: Where to look if it breaks

**Builds on:** Bytes 1–3

**In plain terms:**
Follow the data. Blank product page? Walk down: view → `ProductController` → `CatalogService` → `ProductRepository` → EF → SQL Server. Wrong total? `PriceCalculator`. Login fails? Identity/`AspNetUsers` (`06`). Order not saved? Check `SaveChanges` was reached (`03`, Byte 3). Data in the database but not on screen? The gap is usually the repository filter or the `CoreMapper` mapping.

---

## PUTTING IT TOGETHER

Every feature is the same round trip: browser → controller → service → repository → Entity Framework → SQL Server, and back up through mapper, service, view model, and view. Controllers orchestrate, services decide, repositories fetch, and the database persists. When something breaks, start at the symptom and walk the chain — the layer where the data goes wrong is the layer with the bug.
