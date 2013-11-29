using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcApplication1.Helpers
{
    public class CustomAuthAttribute : AuthorizeAttribute
    {
        public string UserName { get; set; }
        private Helpers.AuthCookieProvider cookies = new Helpers.AuthCookieProvider();
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User != null)
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated || filterContext.HttpContext.User.Identity.Name == "")
                {
                    if (!(filterContext.Controller is MvcApplication1.Controllers.AccountController))
                        base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    Helpers.AuthCookieProvider cookie = new Helpers.AuthCookieProvider();
                    Helpers.AuthenticatedUserData user = cookie.GetAuthenticatedUserData(filterContext.HttpContext.Request);
                    if (user.AuthProvider == "Facebook")
                        filterContext.Result = new RedirectResult(Helpers.FacebookWrapper.LogOut(filterContext.HttpContext.Request));
                    if (user.AuthProvider == "GPlus")
                        filterContext.Result = new RedirectResult(Helpers.GooglePlusWrapper.LogOut());
                    if (user.AuthProvider == "Twitter")
                        filterContext.Result = new RedirectResult(Helpers.TwitterWrapper.LogOut());
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var cookies = new Helpers.AuthCookieProvider();
            if (cookies.GetAuthenticatedUserData(httpContext.Request) != null)
                UserName = cookies.GetAuthenticatedUserData(httpContext.Request).UserName;

            #region Authorization
            //Handle User authorization here
            if (UserName != null)
                if (!UserName.StartsWith("F"))
                    return false;
            #endregion

            return base.AuthorizeCore(httpContext);
        }
    }
}