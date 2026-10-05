# 02 — HTTP Request and MVC Flow

How a request enters the app, reaches a controller, and becomes HTML.

---

### Byte 1: Application startup runs once, in a fixed order

**Builds on:** None — starting point (see `01` for the layer map)

**In plain terms:**
When the app starts, `Global.asax.cs` runs five registrations in order: areas, global filters, routes, bundles, and finally the Unity DI container. Everything the app needs to serve requests is wired here, once.

**The code:**
```csharp
protected void Application_Start()
{
    AreaRegistration.RegisterAllAreas();
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
    UnityConfig.RegisterComponents();   // DI container — must be last
}
```

Unity is registered last so that every other subsystem is already configured before the container starts resolving controllers. If you add a new global filter or route, this is where it gets registered.

---

### Byte 2: One default route handles every URL

**Builds on:** Byte 1

**In plain terms:**
There is a single route template: `{controller}/{action}/{id}`. `/Product/Detail/1` maps to `ProductController.Detail(1)`; `/` falls back to `HomeController.Index` via defaults. No attribute routing, no areas in use.

**The code:**
```csharp
routes.MapRoute(
    name: "Default",
    url: "{controller}/{action}/{id}",
    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
);
```

The controllers in the project are `Home`, `Product`, `Cart`, `Checkout`, and `Account` — every URL in the app is one of those five controllers plus an action name. When tracing a request, start from the URL and find the matching controller action.

---

### Byte 3: Controllers are thin and constructor-injected

**Builds on:** Byte 2 (see `01`, Byte 2 for why interfaces are used)

**In plain terms:**
Controllers do three things: accept input, call a service, return a view. They never query the database or compute prices. Their dependencies arrive through the constructor, supplied by Unity.

**The code:**
```csharp
public class HomeController : Controller
{
    private readonly ICatalogService _catalog;

    public HomeController(ICatalogService catalog)   // Unity injects this
    {
        _catalog = catalog;
    }

    public ActionResult Index()
    {
        ViewBag.Categories = _catalog.GetCategories();
        var listing = _catalog.GetListing(null, null, 1, 8);
        return View(listing);
    }
}
```

Because controllers only depend on interfaces, the request flow is always *Controller → Service → Repository*. If an action is doing more than input/output shaping, that logic belongs in the service.

---

### Byte 4: Views render ViewModels; partials render fragments

**Builds on:** Byte 3

**In plain terms:**
Each view receives one ViewModel and renders HTML from it — no data access in Razor. Reusable fragments (mini-cart, product card, category tree) are partial views, some served both as child actions and as standalone AJAX endpoints.

**The code:**
```csharp
// CartController.MiniCart — deliberately NOT [ChildActionOnly]:
// the layout calls @Html.Action("MiniCart", "Cart"), and site.js
// also GETs /Cart/MiniCart directly after AJAX add-to-cart.
public ActionResult MiniCart()
{
    return PartialView("_MiniCart", _cart.GetMiniCart(CartSession.GetItems()));
}
```

That comment in the real code is load-bearing: marking `MiniCart` as `[ChildActionOnly]` would break the AJAX refresh. When you see a partial, check whether it's also an AJAX endpoint before restricting it.

---

### Byte 5: The cart lives in Session, owned by the Web layer

**Builds on:** Byte 3

**In plain terms:**
The shopping cart is stored in `HttpContext.Session` (InProc, 25-minute timeout per Web.config), managed by a small `SessionCartHelper` in the Web project. Services receive a plain `IEnumerable<CartItem>` and never touch `HttpContext` — a deliberate seam.

**The code:**
```csharp
private SessionCartHelper CartSession
{
    get { return new SessionCartHelper(HttpContext); }
}

public ActionResult Index()
{
    return View(_cart.GetCart(CartSession.GetItems()));  // Web reads session, service does math
}
```

This keeps the service layer web-ignorant and unit-testable: `CartService` works on lists, not on sessions. For logged-in users the controller also persists the cart to the database (`CartItems` table) so it survives session expiry — see `PersistForUser()` in `CartController`.

---

## PUTTING IT TOGETHER

IIS/XSP receives a URL → the default route picks a controller and action → Unity constructs the controller with its service dependencies → the action pulls input (route values, form posts, session cart) and calls services → services return ViewModels → the Razor view renders them into HTML. Cross-cutting state like the cart is staged in Session by Web-layer helpers, never inside business logic.
