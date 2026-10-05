# 03 — Services and Business Logic

Where the app makes its decisions: pricing, cart rules, order creation.

---

### Byte 1: Four services, each with one job

**Builds on:** `01`, Byte 2

**In plain terms:**
Services contain every business rule. `CatalogService` handles browsing, `CartService` handles cart math, `CheckoutService` creates orders, `AccountService` handles order history and customer profiles. Controllers call them; repositories feed them data.

**The code:**
```csharp
// Services only talk to repository *interfaces* —
// they have no idea Entity Framework exists.
public CatalogService(IProductRepository products, ICategoryRepository categories)
```

---

### Byte 2: Cart math happens on plain lists

**Builds on:** Byte 1

**In plain terms:**
You click "Add to cart". The controller pulls your cart (a simple list of product IDs and quantities) from the session and hands it to `CartService`, which validates the product, merges quantities (max 99 per line), and re-fetches current prices.

**The code:**
```csharp
var result = _cart.AddItem(CartSession.GetItems(), productId, variantId, quantity);
CartSession.SaveItems(result.Data);
```

---

### Byte 3: Placing an order snapshots everything

**Builds on:** Byte 1

**In plain terms:**
You click "Place order". `CheckoutService` re-prices the cart from live data, copies product names and prices into the order (so future price changes can't rewrite history), generates an order number like `LE-20261005-A1B2C3`, and saves it.

**The code:**
```csharp
_orders.Add(order);
_unitOfWork.SaveChanges();   // one commit for the whole order
```

---

### Byte 4: One pricing engine, one result pattern

**Builds on:** Bytes 2, 3

**In plain terms:**
All money math funnels through `PriceCalculator`, so the cart page, mini-cart, and checkout can never disagree. Services report problems with `ServiceResult` (success flag + message) instead of throwing. Wrong total? Look at `PriceCalculator`.

**The code:**
```csharp
var shipping = PriceCalculator.CalculateShippingCost(cart.SubTotal, method);
var tax = PriceCalculator.CalculateTax(cart.SubTotal);
```

---

## PUTTING IT TOGETHER

Services are the app's brain: they validate, calculate, and decide, using data from repositories. Controllers orchestrate (HTTP in, session handling, HTTP out); services do the thinking. One shared `PriceCalculator` keeps every total consistent, and `ServiceResult` carries success or failure back up to the controller.
