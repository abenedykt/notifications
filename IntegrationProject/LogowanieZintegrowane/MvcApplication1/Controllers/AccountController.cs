using Facebook;
using MvcApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcApplication1.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Twitter()
        {
            return Redirect(Helpers.TwitterWrapper.LogIn());
        }

        public ActionResult TwitterCallback()
        {
            return Redirect(Helpers.TwitterWrapper.HandleTwitterCallback(Request, Response));
        }

        public ActionResult Facebook()
        {
            return Redirect(Helpers.FacebookWrapper.LogIn(Request,Url));
        }

        public ActionResult FacebookCallback(string code)
        {
            ViewBag.me = Helpers.FacebookWrapper.HandleFacebookCallback(code,Response,Request,Url);
            return RedirectToAction("Index", "Home",ViewBag.me);
        }

        public ActionResult Google()
        {
            return Redirect(GooglePlusWrapper.LogIn());
        }

        public ActionResult GoogleCallback()
        {
            Helpers.GooglePlusWrapper.HandleGoogleCallback(HttpContext.Response);
            
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Helpers.AuthCookieProvider cookie = new Helpers.AuthCookieProvider();
            Helpers.AuthenticatedUserData user = cookie.GetAuthenticatedUserData(Request);
            if(user.AuthProvider=="Facebook")
                return Redirect(Helpers.FacebookWrapper.LogOut(Request));
            if (user.AuthProvider == "GPlus")
                return Redirect(Helpers.GooglePlusWrapper.LogOut());
            if (user.AuthProvider == "Twitter")
                return Redirect(Helpers.TwitterWrapper.LogOut());

            return RedirectToAction("Index", "Home"); 
        }

        [Authorize]
        public ActionResult AuthorizedContent()
        {
            return View("View1");
        }
    }
}
