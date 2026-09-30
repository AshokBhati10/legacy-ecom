using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.Web.Helpers;
using Ecommerce.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Ecommerce.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _account;
        private readonly ICartService _cart;

        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public AccountController()
        {
        }

        public AccountController(IAccountService account, ICartService cart)
        {
            _account = account;
            _cart = cart;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    // NOTE: the auth cookie is only visible on the next request,
                    // so resolve the user id directly instead of User.Identity.
                    var signedInUser = await UserManager.FindByEmailAsync(model.Email);
                    MergePersistedCart(signedInUser != null ? signedInUser.Id : null);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _account.EnsureCustomerForUser(user.Id, model.Email, model.FirstName, model.LastName);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                MergePersistedCart(user.Id);
                return RedirectToAction("Index", "Home");
            }

            AddErrors(result);
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Orders
        public ActionResult Orders()
        {
            var vm = _account.GetOrderHistory(User.Identity.GetUserId());
            return View(vm);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Merges any DB-persisted cart rows into the session cart on sign-in.
        /// </summary>
        private void MergePersistedCart(string userId)
        {
            if (_cart == null || string.IsNullOrWhiteSpace(userId))
                return;

            var session = new SessionCartHelper(HttpContext);
            var merged = session.GetItems().ToList();
            foreach (var saved in _cart.LoadForUser(userId))
            {
                var existing = merged.FirstOrDefault(x => x.ProductId == saved.ProductId && x.VariantId == saved.VariantId);
                if (existing != null)
                    existing.Quantity += saved.Quantity;
                else
                    merged.Add(new CartItem { ProductId = saved.ProductId, VariantId = saved.VariantId, Quantity = saved.Quantity });
            }
            session.SaveItems(merged);
            _cart.SaveForUser(userId, merged);
        }

        #endregion
    }
}
