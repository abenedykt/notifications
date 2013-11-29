using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace MvcApplication1.Helpers
{
    public class AuthCookieProvider
    {
        public void SetAuthCookie(HttpResponseBase response, string username, AuthenticatedUserData userData)
        {
            FormsAuthentication.SetAuthCookie(userData.UserDisplayName, false);
            var cookie = FormsAuthentication.GetAuthCookie(userData.UserDisplayName, false);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            if (ticket == null)
                return;
            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration,
                ticket.IsPersistent, SerializeUserData(userData), ticket.CookiePath);
            var encTicket = FormsAuthentication.Encrypt(newTicket);
            
            cookie.Value = encTicket;
            response.Cookies.Add(cookie);
        }

        private string SerializeUserData(AuthenticatedUserData userData)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(userData);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public AuthenticatedUserData GetAuthenticatedUserData(HttpRequestBase request)
        {
            var cookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var decrypted = FormsAuthentication.Decrypt(cookie.Value);
                if ((decrypted != null) && !string.IsNullOrEmpty(decrypted.UserData))
                {
                    return DeserializeUserData(decrypted.UserData);
                }
            }
            return null;
        }

        public AuthenticatedUserData GetAuthenticatedUserData(HttpResponseBase response)
        {
            var cookie = response.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var decrypted = FormsAuthentication.Decrypt(cookie.Value);
                if ((decrypted != null) && !string.IsNullOrEmpty(decrypted.UserData))
                {
                    return DeserializeUserData(decrypted.UserData);
                }
            }
            return null;
        }

        private AuthenticatedUserData DeserializeUserData(string userData)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<AuthenticatedUserData>(userData);
        }
    }


    public class AuthenticatedUserData
    {
        public string UserName;
        public string SessionId { get; set; }
        public string UserDisplayName { get; set; }
        public string AuthProvider { get; set; }
    }

    public class CookieProvider
    {
        public void SaveCookie(HttpResponseBase response,
                               string key,
                               string value)
        {
            var cookie = new HttpCookie(key, value) { Expires = DateTime.Now.AddDays(30) };
            response.AppendCookie(cookie);
        }

        public bool ReadBoolFromCookie(HttpRequestBase response, string key)
        {
            var cookie = response.Cookies[key];
            bool cookieAsBool;

            return cookie != null && (bool.TryParse(cookie.Value, out cookieAsBool) && cookieAsBool);
        }
    }
}