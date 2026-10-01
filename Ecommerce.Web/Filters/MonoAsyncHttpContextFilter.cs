using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Web.Filters
{
    /// <summary>
    /// Mono/XSP compatibility shim. Mono's HttpContext.Current is backed by
    /// CallContext.GetData (thread-local) instead of flowing with the
    /// ExecutionContext the way .NET Framework's does, so when an async
    /// controller action awaits, the continuation resumes with
    /// HttpContext.Current == null. Unity.Mvc5's dependency resolver reads
    /// HttpContext.Current while the view renders, which then throws
    /// NullReferenceException for every async action (e.g. Account/Login POST).
    /// Restoring Current here — from the filter context, which the framework
    /// passes explicitly — keeps async actions working on Mono. This is a
    /// no-op on .NET Framework / IIS, where Current is already set.
    /// </summary>
    public sealed class MonoAsyncHttpContextFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext != null && HttpContext.Current == null)
            {
                var app = filterContext.HttpContext.ApplicationInstance;
                if (app != null)
                    HttpContext.Current = app.Context;
            }
            base.OnActionExecuted(filterContext);
        }
    }
}
