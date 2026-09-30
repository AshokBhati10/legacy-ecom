using System.Web.Mvc;
using Ecommerce.Core.Interfaces.Services;

namespace Ecommerce.Web.Controllers
{
    public class ProductController : Controller
    {
        private const int PageSize = 12;
        private readonly ICatalogService _catalog;

        public ProductController(ICatalogService catalog)
        {
            _catalog = catalog;
        }

        // GET: /Product?categoryId=2&q=phone&page=1
        public ActionResult Index(int? categoryId, string q, int page = 1)
        {
            var vm = _catalog.GetListing(categoryId, q, page, PageSize);
            ViewBag.SelectedCategoryId = vm.SelectedCategoryId;
            if (Request.IsAjaxRequest())
                return PartialView("_ProductList", vm);
            return View(vm);
        }

        // GET: /Product/Detail/5
        public ActionResult Detail(int id)
        {
            var vm = _catalog.GetDetail(id);
            if (vm == null)
                return HttpNotFound();
            return View(vm);
        }

        // GET: /Product/Filter?categoryId=2&q=phone&page=1
        // Returns the product grid HTML fragment for AJAX filtering.
        [HttpGet]
        public ActionResult Filter(int? categoryId, string q, int page = 1)
        {
            var vm = _catalog.GetListing(categoryId, q, page, PageSize);
            return PartialView("_ProductList", vm);
        }
    }
}
