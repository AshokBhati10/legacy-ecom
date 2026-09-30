# Legacy eCommerce — ASP.NET MVC 5 (.NET Framework 4.7)

A legacy eCommerce web application built exactly to the class specification:
ASP.NET MVC 5 with Razor, Entity Framework 6.4.4 (Database First / EDMX),
SQL Server Express, Unity DI, OWIN cookie authentication (ASP.NET Identity 2),
server-rendered views plus jQuery AJAX HTML partial replacement.

No ASP.NET Core, no Web API/SPA, no extra libraries — this is a period-correct
.NET Framework 4.7 application.

## Solution structure

```
LegacyEcommerce.sln
├── Ecommerce.Core      # Entities, ViewModels, repository/service interfaces (no EF/MVC/HttpContext)
├── Ecommerce.Data      # LegacyEcommerce.edmx, EcommerceDbContext, repositories, UnitOfWork
├── Ecommerce.Services  # Catalog, Cart, Checkout, Account services + PriceCalculator
├── Ecommerce.Web       # MVC 5 app: controllers, Razor views, session cart, OWIN/Identity, Unity
└── Database            # CreateDatabase.sql (schema + Identity tables + SQL login), SeedData.sql
```

Dependency flow (strict): `Controller → Service → Repository → DbContext → SQL Server`.
Controllers never touch EF/DbContext/SQL; services never touch DbContext/Razor/MVC/HttpContext;
repositories are the database boundary; views receive ViewModels/Core contracts, never EDMX entities.

## Prerequisites

- Visual Studio 2019 or later (with .NET Framework 4.7 targeting pack)
- SQL Server Express (`.\SQLEXPRESS`)
- NuGet package restore (packages are **not** committed)

## Setup

1. Open `LegacyEcommerce.sln` in Visual Studio and restore NuGet packages.
2. Create the database (run in order, via `sqlcmd` or SSMS):
   ```
   sqlcmd -S .\SQLEXPRESS -i Database\CreateDatabase.sql
   sqlcmd -S .\SQLEXPRESS -i Database\SeedData.sql
   ```
   `CreateDatabase.sql` creates `LegacyEcommerceDb`, the catalog/order schema,
   the ASP.NET Identity 2.0 tables (same database), and the least-privilege SQL
   login `legacy_app_user`.
3. Set the real password (replace `__SET_YOUR_PASSWORD_HERE__`) in **both**:
   - `Database\CreateDatabase.sql` (the `CREATE LOGIN` statement), and
   - `Ecommerce.Web\Web.config` (the `EcommerceDb` connection string).
4. Press F5 — the app runs on IIS Express. The EDMX connection
   (`metadata=res://*/...`) loads its mapping from the compiled assembly.

## Key flows

- **Catalog** — `GET /Product` (full page) and `GET /Product/Filter` (returns the
  `_ProductList` partial; used by the AJAX filter form and pager).
- **Cart** — session cart owned by the Web layer (`SessionCartHelper` over
  `HttpContext.Session`, InProc, 25-minute timeout). `POST /Cart/Add` is AJAX and
  refreshes the header mini-cart via `$('#mini-cart').load('/Cart/MiniCart')`.
  Signed-in users also get their cart persisted to `CartItems` and merged on login.
- **Checkout** — wizard: Address → Shipping → Payment → Confirmation. Totals are
  always computed server-side by `CheckoutService`/`PriceCalculator`. The payment
  step is demo-only (no payment gateway): card fields are validated and never stored.
- **Account** — Identity 2.0 register/login/logoff with OWIN cookies; order history
  (`/Account/Orders`) is `[Authorize]`-protected and rendered with DataTables.
- Every POST (including AJAX) carries an anti-forgery token.

## Pricing rules (demo)

- Standard shipping $4.99, free on orders of $50+; Express shipping $12.99.
- Tax 8% on the subtotal. See `Ecommerce.Services/Pricing/PriceCalculator.cs`.

## Deployment (production)

- Full IIS, application pool: .NET CLR version **v4.0**, Managed pipeline mode **Integrated**.
- Publish `Ecommerce.Web`; point the `EcommerceDb` / `DefaultConnection` connection
  strings at the production SQL Server; keep real credentials out of source control.

## Notes

- Fancybox 3.5.7 is vendored (`Content/fancybox`, `Scripts/fancybox`) since it is no
  longer distributed via NuGet; everything else restores from NuGet.
- `Ecommerce.Web/Content/images/products/` holds the seed image paths — drop real
  product images there (see its README); missing images fall back to `no-image.png`.
