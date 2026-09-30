using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Ecommerce.Web.Models;

namespace Ecommerce.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // One OWIN context per request for the Identity managers.
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // OWIN cookie authentication (ASP.NET Identity 2.0).
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = TimeSpan.FromMinutes(25),
                SlidingExpiration = true
            });
        }
    }
}
