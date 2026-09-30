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
            vm.Categories = _catalog.GetCategoryTree(null);
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

        // GET: /Product/CategoryTree?parentId=1
        // Returns one level of the nested category tree as an HTML fragment.
        // Called by @Html.Action("CategoryTree", "Product") for the root level
        // and by jQuery when the user expands a node (lazy loading).
        [HttpGet]
        public ActionResult CategoryTree(int? parentId)
        {
            var vm = _catalog.GetCategoryTree(parentId);
            return PartialView("_CategoryTree", vm);
        }
    }
}
