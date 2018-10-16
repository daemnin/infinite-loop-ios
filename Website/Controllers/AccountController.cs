using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Website.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn(string redirectUri = "/")
        {
            redirectUri = Uri.IsWellFormedUriString(redirectUri, UriKind.Relative) ?
                          redirectUri :
                          Url.Action(nameof(HomeController.Index), "Home");

            if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                return Challenge(
                    new AuthenticationProperties { RedirectUri = redirectUri },
                    OpenIdConnectDefaults.AuthenticationScheme);
            }

            return RedirectToAction(redirectUri);
        }

        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction(nameof(HomeController.Index), "Home");

            return View();
        }

        public IActionResult AccessDenied() => View();
    }
}