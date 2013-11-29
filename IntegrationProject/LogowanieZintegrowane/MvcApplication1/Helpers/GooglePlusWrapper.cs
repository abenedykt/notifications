using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using RestSharp;
using System;
using System.Threading;
using System.Web;

namespace MvcApplication1.Helpers
{
    public class GooglePlusWrapper
    {
        private static IAuthorizationState _authstate;
        private static bool isLoggingIn = false;
        private static TasksService _service;
        private static OAuth2Authenticator<WebServerClient> _authenticator;
        private static IAuthorizationState AuthState
        {
            get
            {
                return _authstate;
            }
        }
        public static string LogIn()
        {
            if (!isLoggingIn)
            {
                isLoggingIn = true;

                if (_service == null)
                {
                    _service = new TasksService(_authenticator = CreateAuthenticator());
                    //This constructor in case of another version of Apis. But another version won't work, so - whatever...
                    //_service = new TasksService(new BaseClientService.Initializer
                    //{
                    //    Authenticator = _authenticator = CreateAuthenticator(),
                    //    ApiKey = ClientCredentials.ApiKey
                    //});
                }

                try
                {
                    TaskLists taskLists = _service.Tasklists.List().Fetch();
                    throw new Exception();
                }
                catch (ThreadAbortException)
                {
                    isLoggingIn = false;
                    throw;
                }
                catch (Exception e)
                {
                    isLoggingIn = false;
                }
            }
            return CredentialsAndVariables.ReturnHome;
        }

        public static string HandleGoogleCallback(HttpResponseBase Response)
        {

            _authenticator.LoadAccessToken();
            var accessToken = AuthState.AccessToken;

            var client = new RestClient("https://www.googleapis.com/plus/v1/people/me?access_token=" + accessToken);
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            var content = response.Content;
            string displayNameTag = "\"displayName\": \"";
            int indexOfDisplayName = content.IndexOf(displayNameTag);
            int indexofEndOfName = content.IndexOf("\",", indexOfDisplayName);
            string name = content.Substring(indexOfDisplayName + displayNameTag.Length, indexofEndOfName - (indexOfDisplayName + displayNameTag.Length));

            Helpers.AuthCookieProvider cookie = new Helpers.AuthCookieProvider();
            cookie.SetAuthCookie(Response, name, new Helpers.AuthenticatedUserData() { UserName = name, SessionId = accessToken, UserDisplayName = name, AuthProvider = "GPlus" });

            return CredentialsAndVariables.ReturnHome;
        }

        public static string LogOut()
        {
            _service = null;
            _authstate = null;

            var logOutCookie = new Helpers.AuthCookieProvider();
            logOutCookie.SignOut();

            return @"https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue="+CredentialsAndVariables.LogOutRedirect;
        }


        private static OAuth2Authenticator<WebServerClient> CreateAuthenticator()
        {
            var provider = new WebServerClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = CredentialsAndVariables.ClientID;
            provider.ClientSecret = CredentialsAndVariables.ClientSecret;

            var authenticator = new OAuth2Authenticator<WebServerClient>(provider, GetAuthorization) { NoCaching = true };
            return authenticator;
        }

        private static class CredentialsAndVariables
        {
            static public string ClientID = "120137181883.apps.googleusercontent.com";
            static public string ClientSecret = "iFGrcHVv6p2U0xvS49sdBX5l";
            static public string[] Scopes = new string[] { "https://www.googleapis.com/auth/userinfo.profile" }; //"https://www.googleapis.com/auth/plus.me" }; //, "https://www.googleapis.com/auth/plus.login"
            static public string CallbackActionUrl = "http://localhost/Logowanie/Account/GoogleCallback";
            static public string LogOutRedirect = @"http://localhost/Logowanie/";
            public static string ReturnHome = "Logowanie//Home/Index";
            //static public string ApiKey = "AIzaSyByvZ4I3hpuOY_Fml06SbTGbR-UPlc6Hr0";
        
        }

        private static IAuthorizationState GetAuthorization(WebServerClient client)
        {
            string[] reqAuthScopes = CredentialsAndVariables.Scopes;
            IAuthorizationState state = AuthState;

            if (state != null)
            {
                if (state.AccessTokenExpirationUtc.Value.CompareTo(DateTime.Now.ToUniversalTime()) > 0)
                    return state;
                else
                    state = null;
            }
            state = client.ProcessUserAuthorization(new HttpRequestInfo(HttpContext.Current.Request));
            if (state != null && (!string.IsNullOrEmpty(state.AccessToken) || !string.IsNullOrEmpty(state.RefreshToken)))
            {
                if (state.RefreshToken == null)
                    state.RefreshToken = "";

                _authstate = state;
                return state;
            }
            client.RequestUserAuthorization(reqAuthScopes, "", new Uri(CredentialsAndVariables.CallbackActionUrl)); // Redirecting to Gmail LoginPage
            
            return null;
        }
    }
}