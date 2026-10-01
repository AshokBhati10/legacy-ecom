using System.Web.Mvc;
using Ecommerce.Web.Filters;

namespace Ecommerce.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // Mono/XSP: restores HttpContext.Current after async actions (no-op on IIS).
            filters.Add(new MonoAsyncHttpContextFilter());
        }
    }
}
