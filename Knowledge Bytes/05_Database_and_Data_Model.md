# 05 — Database and Data Model

The SQL Server database: 14 tables, the setup scripts, and how Identity shares the same database.

---

### Byte 1: Fourteen tables in three groups

**Builds on:** `04`, Byte 1

**In plain terms:**
The database holds the catalog (what you sell), the commerce records (carts, orders, customers), and the ASP.NET Identity tables (who can log in) — all in one database, `LegacyEcommerceDb`.

**The code:**
```sql
-- Catalog
CREATE TABLE [dbo].[Categories] ...
CREATE TABLE [dbo].[Products] ...
CREATE TABLE [dbo].[ProductImages] ...
CREATE TABLE [dbo].[ProductVariants] ...
-- Commerce
CREATE TABLE [dbo].[Customers] ...
CREATE TABLE [dbo].[Addresses] ...
CREATE TABLE [dbo].[CartItems] ...     -- persisted carts for logged-in users
CREATE TABLE [dbo].[Orders] ...
CREATE TABLE [dbo].[OrderLines] ...
-- Identity (ASP.NET Identity 2.0)
CREATE TABLE [dbo].[AspNetUsers] ...
CREATE TABLE [dbo].[AspNetRoles] ...
CREATE TABLE [dbo].[AspNetUserRoles] ...
CREATE TABLE [dbo].[AspNetUserClaims] ...
CREATE TABLE [dbo].[AspNetUserLogins] ...
```

`CartItems` bridges the anonymous and logged-in worlds: the cart normally lives in Session (see `02`, Byte 5), but for signed-in users a copy is persisted here so it survives session expiry. `OrderLines` snapshots product data at purchase time (see `03`, Byte 4).

---

### Byte 2: The database is built by scripts, not by EF

**Builds on:** Byte 1 (see `04`, Byte 1 for why EF never touches schema)

**In plain terms:**
`Database/CreateDatabase.sql` creates the database, tables, the `legacy_app_user` SQL login, and grants; `Database/SeedData.sql` inserts the demo catalog (categories, products). You run these with `sqlcmd` — EF migrations are not used and don't exist here.

**The code:**
```sql
IF DB_ID(N'LegacyEcommerceDb') IS NULL
BEGIN
    CREATE DATABASE [LegacyEcommerceDb];
END
GO
-- ... tables ...
IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name = N'legacy_app_user')
BEGIN
    CREATE LOGIN [legacy_app_user] WITH PASSWORD = N'__SET_YOUR_PASSWORD_HERE__',
        CHECK_POLICY = OFF;
END
```

The app connects as `legacy_app_user` (a least-privilege login with `db_datareader`/`db_datawriter`), not as sa. Replace the placeholder password in the script and mirror it in Web.config's connection strings — locally only, never committed.

---

### Byte 3: Identity tables live beside the shop tables

**Builds on:** Byte 1

**In plain terms:**
The `AspNet*` tables are ASP.NET Identity's membership store, and they sit in the same `LegacyEcommerceDb` as the catalog. There are two user concepts: `AspNetUsers` (login credentials, managed by Identity) and `Customers` (shop profile: name, email, linked to a login via `UserId`).

**The code:**
```csharp
// ApplicationDbContext — Identity's context, plain SqlClient:
public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) { }
```

Two connection strings, two contexts, one database: `EcommerceDb` (EntityClient + EDMX metadata) drives the shop via `EcommerceDbContext`; `DefaultConnection` (plain SqlClient) drives login via `ApplicationDbContext`. `AccountService.EnsureCustomerForUser` creates the `Customers` row the first time a login needs a shop profile.

---

### Byte 4: Seed data makes the app runnable immediately

**Builds on:** Byte 2

**In plain terms:**
`SeedData.sql` inserts a small demo catalog (a handful of categories and products) so the storefront renders something on first run. It's the same data the Linux runtime verification used to prove the app works end-to-end.

**The code:**
```sql
-- (SeedData.sql)
Seed data inserted.
-- verified: 6 categories, 12 products
```

After running both scripts, `GET /` should show categories and a product grid with zero code changes — a quick smoke test that the database, connection strings, and EF mapping are all healthy before you touch anything else.

---

## PUTTING IT TOGETHER

`CreateDatabase.sql` builds `LegacyEcommerceDb` with catalog, commerce, and Identity tables plus a least-privilege login; `SeedData.sql` fills the catalog. EF never manages schema — it only reads the mapping from the EDMX. Identity and the shop share one database but use separate connection strings and contexts, and the `Customers` table links a login (`AspNetUsers`) to a shop profile.
