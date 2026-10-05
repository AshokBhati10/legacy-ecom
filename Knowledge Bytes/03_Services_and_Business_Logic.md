# 03 — Services and Business Logic

Where the application's rules live: the four services in `Ecommerce.Services`.

---

### Byte 1: Services depend on interfaces, never on EF

**Builds on:** `01`, Bytes 2–4

**In plain terms:**
The Services project references only `Ecommerce.Core` — not `Ecommerce.Data`. Each service takes repository *interfaces* in its constructor. This is the enforcement mechanism for the layering: business logic physically cannot touch Entity Framework.

**The code:**
```csharp
public CatalogService(IProductRepository products, ICategoryRepository categories)
{
    _products = products;
    _categories = categories;
}
```

There are four services: `CatalogService` (browsing), `CartService` (cart math), `CheckoutService` (order placement), and `AccountService` (order history, customer profile). All business rules — pricing, quantities, order numbers — live in these classes, not in controllers.

---

### Byte 2: CatalogService shapes data for display

**Builds on:** Byte 1

**In plain terms:**
`CatalogService` is read-only. It fetches entities from repositories and converts them into ViewModels: product cards, detail pages with images/variants/related products, and the category tree. It also clamps paging inputs.

**The code:**
```csharp
public ProductListViewModel GetListing(int? categoryId, string q, int page, int pageSize)
{
    if (page < 1) page = 1;
    if (pageSize < 1 || pageSize > 100) pageSize = 12;

    int totalCount;
    var items = _products.GetListing(categoryId, q, page, pageSize, out totalCount);
    // ... maps to ProductCardViewModel, computes TotalPages
}
```

Note the private `ToCardViewModel`/`ToCategoryViewModel` mappers: translating entities to view shapes is the service's job, keeping both controllers and views free of mapping code.

---

### Byte 3: CartService computes over plain item lists

**Builds on:** Byte 1 (see `02`, Byte 5 for where the lists come from)

**In plain terms:**
`CartService` never sees a session or a database cart directly. It receives `IEnumerable<CartItem>` (each just ProductId/VariantId/Quantity), looks up current product data, and returns priced line items. Quantity is capped at 99 per line.

**The code:**
```csharp
public ServiceResult<IEnumerable<CartItem>> AddItem(
    IEnumerable<CartItem> items, int productId, int? variantId, int quantity)
{
    if (quantity < 1) quantity = 1;
    if (quantity > MaxQuantityPerLine) quantity = MaxQuantityPerLine;

    var product = _products.GetById(productId);
    if (product == null)
        return ServiceResult<IEnumerable<CartItem>>.Fail("Product not found.");
    // ... merges into existing line or adds new one
}
```

Because prices are re-fetched from the product on every call (`BuildLine`), the cart always reflects current prices — nothing stale is stored in the cart itself. Persistence (session save, per-user DB save) is the controller's job after the service returns.

---

### Byte 4: CheckoutService turns a cart into a persisted order

**Builds on:** Bytes 1, 3

**In plain terms:**
`CheckoutService.PlaceOrder` is the transactional heart of the app: it re-prices the cart, snapshots product names/prices into order lines (so later price changes don't rewrite history), generates an order number, saves via the repository + unit of work, and re-reads the saved order to confirm.

**The code:**
```csharp
var order = new Order
{
    OrderNumber = GenerateOrderNumber(),   // LE-YYYYMMDD-XXXXXX
    Status = "Placed",
    SubTotal = cart.SubTotal,
    ShippingCost = shipping,
    TaxAmount = tax,
    Total = total,
    // ... shipping address snapshot, order lines snapshot
};
_orders.Add(order);
_unitOfWork.SaveChanges();

var saved = _orders.GetByOrderNumber(order.OrderNumber);
if (saved == null)
    return ServiceResult<Order>.Fail("The order could not be saved.");
```

Two details matter: the order *snapshots* product names and unit prices into `OrderLine` rows (an order is a historical record, not a live view of the catalog), and the final re-read guards against a silent save failure.

---

### Byte 5: ServiceResult and PriceCalculator keep rules consistent

**Builds on:** Bytes 3, 4

**In plain terms:**
Services never throw for expected failures (unknown product, empty cart). They return `ServiceResult<T>` — a small `Success`/`Message`/`Data` wrapper — and controllers translate that into HTTP responses. All money math funnels through the static `PriceCalculator` so cart, mini-cart, and checkout agree.

**The code:**
```csharp
// Pricing is centralized — every caller gets the same numbers:
vm.SubTotal  = PriceCalculator.CalculateSubTotal(vm.Items);
var shipping = PriceCalculator.CalculateShippingCost(cart.SubTotal, method);
var tax      = PriceCalculator.CalculateTax(cart.SubTotal);
```

If you need to change how shipping or tax works, there is exactly one place to look: `Ecommerce.Services/Pricing/PriceCalculator.cs`. Controllers only decide what HTTP status a `ServiceResult` becomes (e.g. `CartController.Add` returns 400 on failure).

---

## PUTTING IT TOGETHER

Services are the application's brain: `CatalogService` for browsing, `CartService` for cart math over plain lists, `CheckoutService` for order placement, `AccountService` for the customer's profile and history. They talk only to Core interfaces, report outcomes via `ServiceResult<T>`, and share one pricing engine. Controllers orchestrate (HTTP in, session handling, HTTP out); services decide.
