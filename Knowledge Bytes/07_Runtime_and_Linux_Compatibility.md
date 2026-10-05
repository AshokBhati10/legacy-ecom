# 07 — Runtime and Linux/Mono Compatibility

Adaptations required to run this Windows-first app under Mono/XSP on Linux. These are **platform concerns, not business architecture** — on IIS/Windows most of them are inert.

---

### Byte 1: Mono loses HttpContext.Current across async awaits

**Builds on:** None — starting point (platform concern)

**In plain terms:**
On .NET Framework, `HttpContext.Current` flows with the async execution context; on Mono it is thread-local, so after an `await` it comes back `null`. Unity.Mvc5's resolver reads `HttpContext.Current` while rendering views, which then threw `NullReferenceException` for every async action (e.g. the login POST).

**The code:**
```csharp
// Filters/MonoAsyncHttpContextFilter.cs — registered globally
public override void OnActionExecuted(ActionExecutedContext filterContext)
{
    if (filterContext != null && HttpContext.Current == null)
    {
        var app = filterContext.HttpContext.ApplicationInstance;
        if (app != null)
            HttpContext.Current = app.Context;   // restore from the explicit filter context
    }
    base.OnActionExecuted(filterContext);
}
```

The fix restores `Current` from the filter context, which MVC passes explicitly and therefore survives the await. On IIS this is a no-op (`Current` is already set). If you add a new async controller action and see null-reference crashes only on Linux, this filter is the first place to look.

---

### Byte 2: xbuild can't run EntityDeploy, so metadata is checked in

**Builds on:** `04`, Byte 5 (the `res://*/` connection string)

**In plain terms:**
EF's build task splits the EDMX into `.csdl`/`.ssdl`/`.msl` resources — but its build DLL targets MSBuild 15.1, and Mono's xbuild 14 silently drops the task's outputs. The build "succeeds" with no metadata embedded, and EF throws `MetadataException` at runtime. The repo works around this by checking in the three split files and embedding them only on Unix.

**The code:**
```xml
<!-- Ecommerce.Data.csproj -->
<ItemGroup Condition="'$(OS)' == 'Unix'">
  <EmbeddedResource Include="LegacyEcommerce.csdl">
    <LogicalName>LegacyEcommerce.csdl</LogicalName>
  </EmbeddedResource>
  <!-- ... .msl, .ssdl ... -->
</ItemGroup>
```

The `LogicalName`s exactly match what EntityDeploy produces on Windows, so the `metadata=res://*/...` connection string resolves unchanged on both platforms. Windows builds skip this group entirely (`$(OS)` is `Windows_NT` there) and use normal EntityDeploy. **If you edit the EDMX**, regenerate the three files (e.g. copy them from `obj\<Configuration>\edmxResourcesToEmbed\` after a Windows build) or Linux builds will embed stale mapping.

---

### Byte 3: The .sln project-type GUID was wrong for the Web project

**Builds on:** None — starting point (platform/tooling concern)

**In plain terms:**
`Ecommerce.Web` was declared in the `.sln` with the *Web Site* project-type GUID instead of the *C# project* GUID, so Visual Studio refused to load it as a normal project. The fix changed one GUID in `LegacyEcommerce.sln` — no code impact.

This is purely a tooling fix: xbuild never cared, but any developer opening the solution on Windows would hit it immediately. If the Web project ever shows as "unavailable" in VS, check the project-type GUID first.

---

### Byte 4: System.Runtime.CompilerServices.Unsafe is pinned to 4.5.2

**Builds on:** `06`, Byte 3 (Unity wiring)

**In plain terms:**
Unity.Container 5.11.1 declares a dependency on `System.Runtime.CompilerServices.Unsafe` 4.0.4.1, and the app crashed at runtime with an assembly-load error for exactly that version. The fix pins `System.Runtime.CompilerServices.Unsafe` to 4.5.2 (with a binding redirect), which satisfies the reference on both Windows and Mono.

The lesson: when a `FileLoadException`/`TypeLoadException` names a specific assembly version, trace the actual dependency chain (here: Unity → Unsafe) rather than assuming a newer package will fix it. The pin is in `packages.config`/`Web.config` — don't "clean it up" as an unused-looking old package.

---

### Byte 5: InProc session and machineKey behave differently on XSP

**Builds on:** `02`, Byte 5 (session cart)

**In plain terms:**
Two runtime behaviors differ under XSP/Mono: (1) there is no `<machineKey>` configured, so auth cookies and anti-forgery tokens become invalid on every app restart; (2) touching `bin/` while xsp4 runs triggers an AppDomain recycle that kills the host process — restart xsp4 after every rebuild.

Neither affects the architecture — the session cart, cookie auth, and anti-forgery tokens all work the same *within* a process lifetime. But for a stable Linux deployment you'd want a fixed `<machineKey>` in Web.config and a process manager that restarts xsp4 on recycle.

---

## PUTTING IT TOGETHER

None of these change what the application *does* — they change what it takes to *run* it outside IIS. The async filter patches a Mono threading difference, the checked-in EDMX artifacts patch an xbuild/MSBuild task incompatibility, the GUID and Unsafe pins patch tooling and assembly-resolution gaps, and the session notes are operational caveats. When debugging a Linux-only failure, check this file before assuming a business-logic bug.
