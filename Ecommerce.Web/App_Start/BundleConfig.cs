using System.Web.Optimization;

namespace Ecommerce.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // CSS
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/DataTables/css/jquery.dataTables.css",
                "~/Content/fancybox/jquery.fancybox.min.css",
                "~/Content/Site.css"));

            // jQuery core (3.4.1)
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-3.4.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.12.1.js"));

            // jQuery Validation + Unobtrusive
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js"));

            // jQuery Unobtrusive Ajax
            bundles.Add(new ScriptBundle("~/bundles/ajax").Include(
                "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/DataTables/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/fancybox").Include(
                "~/Scripts/fancybox/jquery.fancybox.min.js"));

            // Application behaviour (filtering, cart, checkout wizard, anti-forgery)
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/site.js"));
        }
    }
}
