using Facebook;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Helpers
{
    public static class FacebookWrapper
    {
        private static HttpRequestBase request;
        private static UrlHelper url;
        public static string LogIn(HttpRequestBase Request, UrlHelper Url)
        {
            request = Request;
            url = Url;

            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = CredentialsAndVariables.ClientID,
                client_secret = CredentialsAndVariables.ClientSecret,
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = CredentialsAndVariables.Scope
            });


            return loginUrl.AbsoluteUri;
        }

        public static dynamic HandleFacebookCallback(string code, HttpResponseBase Response, HttpRequestBase Request, UrlHelper Url)
        {
            request = Request;
            url = Url;

            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = CredentialsAndVariables.ClientID,
                client_secret = CredentialsAndVariables.ClientSecret,
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;
            fb.AccessToken = accessToken;

            dynamic me = fb.Get("me");
            
            /* Prosty sposób na wpisywanie na oś czasu wiadomości
            dynamic parameters = new ExpandoObject();
            parameters.message = "Hello World!" + DateTime.Now.ToShortTimeString();
            result = fb.Post("me/feed", parameters);
            */
            Helpers.AuthCookieProvider cookie = new Helpers.AuthCookieProvider();
            cookie.SetAuthCookie(Response, me.name, new Helpers.AuthenticatedUserData() { UserName = me.name, SessionId = accessToken, UserDisplayName = me.name, AuthProvider="Facebook" });

            return me;
        }

        public static string LogOut(HttpRequestBase Request)
        {
            var logOutCookie = new Helpers.AuthCookieProvider();
            var fb = new FacebookClient(logOutCookie.GetAuthenticatedUserData(Request).SessionId);
            var logoutParameters = new Dictionary<string, object>
                  {
                      {"access_token", logOutCookie.GetAuthenticatedUserData(Request).SessionId},
                      { "next", CredentialsAndVariables.LogOutRedirect }
                  };


            logOutCookie.SignOut();
            var NavigateUrl = fb.GetLogoutUrl(logoutParameters);

            return NavigateUrl.AbsoluteUri;
        }

        private static class CredentialsAndVariables
        {
            static public string ClientID = "637975946224454";
            static public string ClientSecret = "717513aec382e8372fa2381ad9617063";
            static public string LogOutRedirect = "http://127.0.0.1/Logowanie/";
            static public string CallbackAction = "FacebookCallback";
            static public string Scope = "email, publish_stream";

        }
        private static Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = url.Action(CredentialsAndVariables.CallbackAction);
                return uriBuilder.Uri;
            }
        }
    }
}
