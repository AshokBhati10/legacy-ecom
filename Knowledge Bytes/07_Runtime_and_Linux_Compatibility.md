# 07 — Runtime and Linux/Mono Compatibility

This file is different: it lists the unusual things that exist **only because this Windows app runs on Linux/Mono**. None of this is business logic.

---

### Byte 1: Normal architecture vs. Linux adaptations

**Builds on:** None — starting point

**In plain terms:**
Everything in files 01–06 is the app's normal design and works the same on Windows. This file collects the workarounds needed to run it under Mono/XSP on Linux. If a bug happens only on Linux, start here.

---

### Byte 2: Mono loses track of the request during async code

**Builds on:** Byte 1

**In plain terms:**
On Windows, `HttpContext.Current` survives `await`; on Mono it becomes null, which crashed every async page (like login). A small global filter restores it after each action — a no-op on Windows.

**The code:**
```csharp
// Filters/MonoAsyncHttpContextFilter.cs
if (filterContext != null && HttpContext.Current == null)
    HttpContext.Current = filterContext.HttpContext.ApplicationInstance.Context;
```

---

### Byte 3: Linux builds embed EF metadata by hand

**Builds on:** Byte 1 (see `04`, Byte 4)

**In plain terms:**
EF normally extracts mapping files from the EDMX during a Windows build, but Mono's build tool silently skips that step — so the app crashed at runtime with `MetadataException`. The fix: the three mapping files are checked into the repo and embedded only on Linux.

**The code:**
```xml
<ItemGroup Condition="'$(OS)' == 'Unix'">
  <EmbeddedResource Include="LegacyEcommerce.csdl">
    <LogicalName>LegacyEcommerce.csdl</LogicalName>
```

**If you edit the EDMX**, regenerate these three files or Linux builds will use a stale mapping.

---

### Byte 4: Three smaller fixes

**Builds on:** Byte 1

**In plain terms:**
- The `.sln` declared the Web project with the wrong project-type GUID, so Visual Studio couldn't load it — one GUID changed, no code impact.
- Unity needs `System.Runtime.CompilerServices.Unsafe` 4.5.2 pinned; without it the app crashes at startup with an assembly-load error.
- Don't modify `bin/` while the Linux host (xsp4) is running — it kills the process. Restart it after every rebuild.

---

## PUTTING IT TOGETHER

These are all platform patches, not features: an async-context shim for Mono, checked-in EF metadata for the Linux build tool, a project GUID fix, a dependency pin, and a restart rule for the Linux host. On Windows/IIS most of them do nothing. When something breaks only on Linux, check this file before suspecting the application logic.
