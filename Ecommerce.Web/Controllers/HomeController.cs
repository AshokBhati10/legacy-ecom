using System.Web.Mvc;
using Ecommerce.Core.Interfaces.Services;

namespace Ecommerce.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICatalogService _catalog;

        public HomeController(ICatalogService catalog)
        {
            _catalog = catalog;
        }

        public ActionResult Index()
        {
            // Hero + featured categories + a product grid (first page of the catalog).
            ViewBag.Categories = _catalog.GetCategories();
            var listing = _catalog.GetListing(null, null, 1, 8);
            return View(listing);
        }
    }
}
