# 02 — Request and MVC Flow

What happens between typing a URL and seeing a page.

---

### Byte 1: A URL becomes a controller action

**Builds on:** `01`, Byte 1

**In plain terms:**
You open `/Product/Detail/1`. One route rule — `{controller}/{action}/{id}` — turns that into "call `Detail(1)` on `ProductController`". `/` opens `HomeController.Index` by default.

**The code:**
```csharp
routes.MapRoute(
    name: "Default",
    url: "{controller}/{action}/{id}",
    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
);
```

Every page in this app is one of five controllers (Home, Product, Cart, Checkout, Account) plus an action name. To trace any page, start from its URL.

---

### Byte 2: The controller asks, never fetches

**Builds on:** Byte 1

**In plain terms:**
The controller's job is tiny: take the input, ask a service for the data, hand the result to a view. It never queries the database or calculates anything.

**The code:**
```csharp
public ActionResult Detail(int id)
{
    var vm = _catalog.GetDetail(id);   // the service does the work
    if (vm == null)
        return HttpNotFound();
    return View(vm);                    // the view only renders
}
```

If a page shows wrong data, the bug is almost never in the controller — look at the service or below it.

---

### Byte 3: Views render view models

**Builds on:** Byte 2

**In plain terms:**
The `vm` above is a *view model* — a simple object shaped exactly for one page (product + its images + related products). The Razor view just turns it into HTML. No database calls in views, ever.

**The code:**
```text
Controller → returns View(vm) → Razor view renders vm → HTML → your browser
```

View models are the contract between the service layer and the UI. If the page needs new data, the service adds it to the view model — the view just displays what it is given.

---

## PUTTING IT TOGETHER

URL → route → controller action → service call → view model → Razor view → HTML. The controller is a receptionist: it takes your request, passes it to the service, and hands the answer to the view. Business logic lives one layer down, in Services.
