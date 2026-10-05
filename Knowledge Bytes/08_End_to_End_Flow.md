# 08 — End-to-End Request Flow

Two complete traces through every layer, plus the gotchas that bite newcomers.

---

### Byte 1: Trace — loading the homepage (`GET /`)

**Builds on:** `01`–`04`

**In plain terms:**
The default route maps `/` to `HomeController.Index`. The controller asks `ICatalogService` for categories and a product listing; the service queries repositories; repositories run EF LINQ and map to Core entities; the service builds a `ProductListViewModel`; the view renders it.

**The code:**
```csharp
// 1. Controller (Web)
public ActionResult Index()
{
    ViewBag.Categories = _catalog.GetCategories();          // 2. Service
    var listing = _catalog.GetListing(null, null, 1, 8);    // 2. Service
    return View(listing);                                    // 5. View
}
// 3. Service -> _categories.GetActive()                     // 3. Repository
// 4. Repository: _db.Categories.Include(...).Where(...).ToList()
//                then .Select(x => x.ToCore())              // 4. CoreMapper
```

Five hops, each in its layer: route → controller → service → repository (+mapper) → view. This is the shape of *every* read in the app. If the homepage is slow, the query to inspect is in the repository; if the display is wrong, the mapping to inspect is in the service.

---

### Byte 2: Trace — adding to cart (`POST /Cart/Add`)

**Builds on:** Byte 1 (see `02`, Byte 5 and `03`, Byte 3)

**In plain terms:**
The controller pulls the session cart via `SessionCartHelper`, hands the plain list to `CartService.AddItem`, gets back a `ServiceResult` with the new list, saves it to session (and to the DB if logged in), then returns either a refreshed `_MiniCart` partial (AJAX) or a redirect.

**The code:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Add(int productId, int? variantId, int quantity = 1)
{
    var result = _cart.AddItem(CartSession.GetItems(), productId, variantId, quantity);
    if (!result.Success)
    {
        Response.StatusCode = 400;
        return Content(result.Message);
    }
    CartSession.SaveItems(result.Data);
    PersistForUser();                                        // DB copy for logged-in users
    if (Request.IsAjaxRequest())
        return PartialView("_MiniCart", _cart.GetMiniCart(result.Data));
    return RedirectToAction("Index");
}
```

Notice the controller orchestrates three persistence concerns (session, DB, response format) while the service only does cart math. `ServiceResult` is the contract between them: success → new list, failure → message + 400.

---

### Byte 3: Trace — placing an order (checkout wizard)

**Builds on:** Bytes 1, 2 (see `03`, Byte 4)

**In plain terms:**
Checkout is a four-step wizard (Address → Shipping → Payment → Confirmation) with each step's input stashed in Session. The final POST calls `CheckoutService.PlaceOrder`, which re-prices everything from the live catalog, snapshots the totals and line items into an `Order`, saves it through the repository + `UnitOfWork`, and returns the saved order for the confirmation page.

**The code:**
```csharp
// CheckoutController (final step)
var result = _checkout.PlaceOrder(
    CartSession.GetItems(), address, shippingMethod, paymentMethod,
    User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null);
if (result.Success)
{
    CartSession.Clear();          // cart is consumed by the order
    Session["Checkout.LastOrderId"] = result.Data.Id;
    return RedirectToAction("Confirmation");
}
```

Totals are never trusted from the client or the session — `PlaceOrder` recomputes subtotal, shipping, and tax from current product data. The wizard state in Session is just the *inputs*; the money is always recalculated server-side at commit time.

---

### Byte 4: Gotchas for newcomers

**Builds on:** Bytes 1–3 (see `07` for platform gotchas)

**In plain terms:**
Four things that surprise people in this codebase:

1. **The cart has two homes.** Anonymous carts live in Session only; logged-in users *also* get a `CartItems` DB row via `PersistForUser`. If a logged-in user's cart looks stale, check which copy you're reading.
2. **Prices are never stored in the cart.** Every cart render re-fetches product prices (`CartService.BuildLine`). A price change applies to existing carts immediately — and `OrderLine` snapshots exist precisely so *orders* don't change.
3. **EF metadata names are load-bearing.** The strings `LegacyEcommerce.csdl/.ssdl/.msl` appear in Web.config, the csproj `LogicalName`s, and the checked-in filenames. Rename one without renaming all and you get `MetadataException`.
4. **Guest checkout is fully supported.** `PlaceOrder` accepts an empty `userId`; the order is recorded with the typed address. Don't assume `CustomerId` is non-null when writing order-related code.

---

## PUTTING IT TOGETHER

Every flow in this app is the same five-hop pattern — route, controller, service, repository, view — with the service layer as the decision-maker and the controller as the HTTP/session orchestrator. Writes go through `UnitOfWork.SaveChanges` once per request; reads return ViewModels shaped for one view. The cart bridges session and database, checkout recomputes all money at commit time, and orders snapshot history so the past can't be rewritten by a price change.
