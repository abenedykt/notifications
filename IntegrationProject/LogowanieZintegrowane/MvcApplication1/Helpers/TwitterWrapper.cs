using Hammock.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TweetSharp;

namespace MvcApplication1.Helpers
{
    public class TwitterWrapper
    {
        public static bool IsTempTokenActive = false;
        private static TwitterService _service;
        private static OAuthRequestToken _unauthorizedToken;
        private static OAuthAccessToken accessToken;

        internal static string LogIn()
        {
            _service = new TwitterService(CredentialsAndVariables.ClientID,CredentialsAndVariables.ClientSecret);
            
            _unauthorizedToken = _service.GetRequestToken();
            string url;// = _service.GetAuthorizationUri(_unauthorizedToken).ToString();
            url = _service.GetAuthenticationUrl(_unauthorizedToken).AbsoluteUri;
            return url;
        }

        internal static string HandleTwitterCallback(HttpRequestBase Request, HttpResponseBase Response)
        {
            string verifier = Request.Url.AbsoluteUri.Substring(Request.Url.AbsoluteUri.IndexOf("oauth_verifier=") + "oauth_verifier=".Length);
            accessToken = _service.GetAccessToken(_unauthorizedToken,verifier);

            Helpers.CookieProvider provider = new CookieProvider();
            provider.SaveCookie(Response, "Temp_auth", accessToken.ScreenName);
            IsTempTokenActive = true;
            Helpers.AuthCookieProvider cookie = new Helpers.AuthCookieProvider();
            cookie.SetAuthCookie(Response, accessToken.ScreenName, new Helpers.AuthenticatedUserData()
            {
                UserName = accessToken.ScreenName,
                SessionId = accessToken.Token,
                UserDisplayName = accessToken.ScreenName,
                AuthProvider = "Twitter"
            });


            return CredentialsAndVariables.CallbackAction;
        }

        internal static string LogOut()
        {
            _service = null;
            _unauthorizedToken = null;
            accessToken = null;
            

            var logOutCookie = new Helpers.AuthCookieProvider();
            logOutCookie.SignOut();

            return CredentialsAndVariables.LogOutRedirect;
        }

        private static class CredentialsAndVariables
        {
            static public string ClientID = "Jtfi9OImkf8rXUkfV4N5jQ";
            static public string ClientSecret = "DC1vNJtwEuHueYDdRlSFaGdvZNA4lqHSVsEWEf8Rw";
            static public string LogOutRedirect = "http://127.0.0.1/Logowanie/";
            static public string CallbackAction = "/Logowanie/Home/Index";
        }
    }
}