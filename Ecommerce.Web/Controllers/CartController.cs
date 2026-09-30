using System.Linq;
using System.Web.Mvc;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Web.Helpers;
using Microsoft.AspNet.Identity;

namespace Ecommerce.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cart;

        public CartController(ICartService cart)
        {
            _cart = cart;
        }

        private SessionCartHelper CartSession
        {
            get { return new SessionCartHelper(HttpContext); }
        }

        // GET: /Cart
        public ActionResult Index()
        {
            return View(_cart.GetCart(CartSession.GetItems()));
        }

        // Header mini-cart fragment (child action + AJAX refresh).
        [ChildActionOnly]
        public ActionResult MiniCart()
        {
            return PartialView("_MiniCart", _cart.GetMiniCart(CartSession.GetItems()));
        }

        // POST: /Cart/Add — AJAX returns the refreshed mini-cart fragment.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int productId, int? variantId, int quantity = 1)
        {
            var result = _cart.AddItem(CartSession.GetItems(), productId, variantId, quantity);
            if (!result.Success)
            {
                Response.StatusCode = 400;
                return Content(result.Message);
            }

            CartSession.SaveItems(result.Data);
            PersistForUser();

            if (Request.IsAjaxRequest())
                return PartialView("_MiniCart", _cart.GetMiniCart(result.Data));

            return RedirectToAction("Index");
        }

        // POST: /Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int productId, int? variantId, int quantity)
        {
            var result = _cart.UpdateItem(CartSession.GetItems(), productId, variantId, quantity);
            if (result.Success)
            {
                CartSession.SaveItems(result.Data);
                PersistForUser();
            }
            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int productId, int? variantId)
        {
            var result = _cart.RemoveItem(CartSession.GetItems(), productId, variantId);
            if (result.Success)
            {
                CartSession.SaveItems(result.Data);
                PersistForUser();
            }
            return RedirectToAction("Index");
        }

        // POST: /Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Clear()
        {
            CartSession.SaveItems(_cart.ClearCart());
            PersistForUser();
            return RedirectToAction("Index");
        }

        private void PersistForUser()
        {
            if (User.Identity.IsAuthenticated)
                _cart.SaveForUser(User.Identity.GetUserId(), CartSession.GetItems());
        }
    }
}
