# 06 — Authentication and Dependency Injection

How login works, and how the app plugs its pieces together.

---

### Byte 1: Logging in, step by step

**Builds on:** `05`, Byte 3

**In plain terms:**
You submit the login form. `AccountController` asks ASP.NET Identity to verify your email and password against the `AspNetUsers` table. On success it merges your saved database cart into the session cart and sends you on your way.

**The code:**
```csharp
var result = await SignInManager.PasswordSignInAsync(
    model.Email, model.Password, model.RememberMe, shouldLockout: true);
// on success:
MergePersistedCart(signedInUser.Id);
```

Login has a side effect worth knowing: your persisted cart (from `CartItems`) is merged into the session cart at login, so nothing you saved is lost.

---

### Byte 2: A cookie keeps you logged in

**Builds on:** Byte 1

**In plain terms:**
After a successful login, OWIN middleware issues an encrypted cookie (25-minute sliding expiration). Each request after that carries the cookie, so the app knows who you are without asking again. Web.config explicitly disables old-style Forms auth — cookies are handled by OWIN.

**The code:**
```csharp
app.UseCookieAuthentication(new CookieAuthenticationOptions
{
    LoginPath = new PathString("/Account/Login"),
    ExpireTimeSpan = TimeSpan.FromMinutes(25),
});
```

"Logged out unexpectedly" → the cookie expired (25 min, same as the session timeout) or the app restarted without a fixed machine key (see `07`).

---

### Byte 3: Unity plugs interfaces into real classes

**Builds on:** `01`, Byte 3

**In plain terms:**
At startup, `UnityConfig` tells the app which concrete class implements each interface — one `DbContext` and one `UnitOfWork` per request, shared by every repository in that request. Controllers just declare what they need in their constructors.

**The code:**
```csharp
container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());
container.RegisterType<ICatalogService, CatalogService>(new HierarchicalLifetimeManager());
DependencyResolver.SetResolver(new UnityDependencyResolver(container));
```

"One order, one save" works because every repository in a request shares the same `DbContext` instance. If a service gets a null dependency, the Unity registrations are the place to check.

---

## PUTTING IT TOGETHER

Identity checks your password against `AspNetUsers`; OWIN hands you a cookie that proves who you are on later requests. Unity, at startup, wires every interface to its implementation with per-request lifetimes — so controllers receive ready-made services, and all repositories in one request share one database context.
