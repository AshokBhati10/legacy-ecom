# 05 — Database and Data Model

What's inside SQL Server: the tables, the setup scripts, and where logins are checked.

---

### Byte 1: Fourteen tables in three groups

**Builds on:** None — starting point

**In plain terms:**
One database, `LegacyEcommerceDb`, holds everything: the catalog (Categories, Products, ProductImages, ProductVariants), the commerce records (Customers, Addresses, CartItems, Orders, OrderLines), and the login tables (AspNetUsers, AspNetRoles, and friends).

**The code:**
```sql
CREATE TABLE [dbo].[Products] ...
CREATE TABLE [dbo].[Orders] ...
CREATE TABLE [dbo].[AspNetUsers] ...
```

The `AspNet*` tables are ASP.NET Identity's — that is where login credentials live. Everything else is the shop.

---

### Byte 2: Scripts build it, not code

**Builds on:** Byte 1 (see `04`, Byte 4)

**In plain terms:**
`Database/CreateDatabase.sql` creates the database, tables, and the `legacy_app_user` login the app connects with. `Database/SeedData.sql` inserts a demo catalog (6 categories, 12 products) so the site works immediately after setup.

**The code:**
```sql
CREATE LOGIN [legacy_app_user] WITH PASSWORD = N'__SET_YOUR_PASSWORD_HERE__',
    CHECK_POLICY = OFF;
```

The app never connects as sa — it uses this limited login. Set the real password in your local Web.config only; never commit it.

---

### Byte 3: Your login is checked against AspNetUsers

**Builds on:** Byte 1

**In plain terms:**
When you log in, ASP.NET Identity looks up your email in `AspNetUsers` and verifies the password hash. Your shop profile (name, order history) lives separately in `Customers`, linked by your user ID — guests can check out without either.

**The code:**
```text
AspNetUsers  → who can log in (email + password hash)
Customers    → shop profile (name, phone), linked by UserId
```

"Login fails" → the problem is in Identity/`AspNetUsers`. "Order history is empty" → check the `Customers`/`Orders` link.

---

## PUTTING IT TOGETHER

SQL scripts create the database and seed the catalog; the app connects as a limited login. Shop data and login data share one database but live in separate table groups, joined only by user ID. Entity Framework reads them through the mapping — it never creates them.
