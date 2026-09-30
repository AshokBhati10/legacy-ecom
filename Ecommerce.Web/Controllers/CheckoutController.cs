using System.Web.Mvc;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.Web.Helpers;
using Microsoft.AspNet.Identity;

namespace Ecommerce.Web.Controllers
{
    /// <summary>
    /// Multi-step checkout wizard: Address → Shipping → Payment → Confirmation.
    /// Wizard state lives in Session; totals always come from the services.
    /// </summary>
    public class CheckoutController : Controller
    {
        private const string AddressKey = "Checkout.Address";
        private const string ShippingMethodKey = "Checkout.ShippingMethod";
        private const string LastOrderKey = "Checkout.LastOrderId";

        private readonly ICheckoutService _checkout;
        private readonly ICartService _cart;
        private readonly IAccountService _account;

        public CheckoutController(ICheckoutService checkout, ICartService cart, IAccountService account)
        {
            _checkout = checkout;
            _cart = cart;
            _account = account;
        }

        private SessionCartHelper CartSession
        {
            get { return new SessionCartHelper(HttpContext); }
        }

        // ---- Step 1: Address -------------------------------------------------

        public ActionResult Address()
        {
            if (CartSession.GetItems().Count == 0)
                return RedirectToAction("Index", "Cart");

            var vm = Session[AddressKey] as CheckoutAddressViewModel ?? new CheckoutAddressViewModel();

            // Prefill for signed-in shoppers.
            if (User.Identity.IsAuthenticated && string.IsNullOrWhiteSpace(vm.Email))
            {
                var customer = _account.GetCustomerByUserId(User.Identity.GetUserId());
                if (customer != null)
                {
                    vm.Email = customer.Email;
                    vm.FirstName = customer.FirstName;
                    vm.LastName = customer.LastName;
                }
                else
                {
                    vm.Email = User.Identity.Name;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Address(CheckoutAddressViewModel vm)
        {
            if (CartSession.GetItems().Count == 0)
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
                return View(vm);

            Session[AddressKey] = vm;
            return RedirectToAction("Shipping");
        }

        // ---- Step 2: Shipping ------------------------------------------------

        public ActionResult Shipping()
        {
            var address = Session[AddressKey] as CheckoutAddressViewModel;
            if (address == null)
                return RedirectToAction("Address");
            if (CartSession.GetItems().Count == 0)
                return RedirectToAction("Index", "Cart");

            var vm = new CheckoutShippingViewModel
            {
                ShippingMethod = Session[ShippingMethodKey] as string ?? "Standard",
                Options = _checkout.GetShippingOptions(CartSession.GetItems())
            };
            ViewBag.Summary = _checkout.BuildSummary(CartSession.GetItems(), vm.ShippingMethod);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Shipping(CheckoutShippingViewModel vm)
        {
            var address = Session[AddressKey] as CheckoutAddressViewModel;
            if (address == null)
                return RedirectToAction("Address");

            vm.Options = _checkout.GetShippingOptions(CartSession.GetItems());
            if (!ModelState.IsValid)
            {
                ViewBag.Summary = _checkout.BuildSummary(CartSession.GetItems(), vm.ShippingMethod);
                return View(vm);
            }

            Session[ShippingMethodKey] = vm.ShippingMethod;
            return RedirectToAction("Payment");
        }

        // ---- Step 3: Payment -------------------------------------------------

        public ActionResult Payment()
        {
            var address = Session[AddressKey] as CheckoutAddressViewModel;
            var shippingMethod = Session[ShippingMethodKey] as string;
            if (address == null)
                return RedirectToAction("Address");
            if (string.IsNullOrWhiteSpace(shippingMethod))
                return RedirectToAction("Shipping");
            if (CartSession.GetItems().Count == 0)
                return RedirectToAction("Index", "Cart");

            ViewBag.Summary = _checkout.BuildSummary(CartSession.GetItems(), shippingMethod);
            return View(new CheckoutPaymentViewModel { PaymentMethod = "CashOnDelivery" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(CheckoutPaymentViewModel vm)
        {
            var address = Session[AddressKey] as CheckoutAddressViewModel;
            var shippingMethod = Session[ShippingMethodKey] as string;
            if (address == null)
                return RedirectToAction("Address");
            if (string.IsNullOrWhiteSpace(shippingMethod))
                return RedirectToAction("Shipping");

            var items = CartSession.GetItems();
            if (items.Count == 0)
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
            {
                ViewBag.Summary = _checkout.BuildSummary(items, shippingMethod);
                return View(vm);
            }

            string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
            var result = _checkout.PlaceOrder(items, address, shippingMethod, vm.PaymentMethod, userId);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.Summary = _checkout.BuildSummary(items, shippingMethod);
                return View(vm);
            }

            // Order placed: clear cart and wizard state, remember the order id
            // in session so Confirmation cannot be guessed by URL.
            CartSession.Clear();
            if (User.Identity.IsAuthenticated)
                _cart.SaveForUser(userId, CartSession.GetItems());
            Session.Remove(AddressKey);
            Session.Remove(ShippingMethodKey);
            Session[LastOrderKey] = result.Data.Id;

            return RedirectToAction("Confirmation", new { id = result.Data.Id });
        }

        // ---- Step 4: Confirmation --------------------------------------------

        public ActionResult Confirmation(int id)
        {
            var lastOrderId = Session[LastOrderKey] as int?;
            if (!lastOrderId.HasValue || lastOrderId.Value != id)
                return HttpNotFound();

            var order = _checkout.GetOrder(id);
            if (order == null)
                return HttpNotFound();

            return View(order);
        }
    }
}
