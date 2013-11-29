1. Krótki opis.

Logowanie zintegrowane umożliwia autentykację poprzez Facebooka,Google+ oraz Twitter'a. Zawiera gotowe statyczne klasy FacebookWrapper, GooglePlusWrapper oraz TwitterWrapper. Działanie opiera się na zasadzie przekierowania na stronę logowania danego portalu społecznościowego oraz oczekiwania na wiadomość zwrotną (Callback). Informacją zwrotną jest Access Token, który umożliwia nam komunikację z portalami w celu np. otrzymania informacji o użytkowniku. Informacje o zautoryzowanym użytkowniku wraz z jego Access Tokenem przechowywane są w AuthCookie.


2. Potrzebne dane przed implementacją.

Aby możliwe było korzystanie z zewnętrznej autentykacji należy utworzyć nową aplikację na deweloperskim koncie Facebooka/Google+/Twitter'a. Tam przyznane zostaną takie informacje jak ClientID i ClientSecret (ciągi znaków), które są niezbędne do poprawnego działania logowania. W tym samym miejscu należy również ustawić adres hosta (najlepiej wraz z portem), z którego dokonywane jest przekierowanie na zewnętrzną stronę logowania.
Uwagi:
Twitter nie pozwala na użycie nazwy "localhost" jako adresu aplikacji. Zaleca się użycie adresu 127.0.0.1 albo rzeczywistej nazwy strony.


3. Opis klas
a) FacebookWrapper- zawiera metody umożliwiające komunikację z Facebookiem w celu autentykacji.
⦁	Najważniejszą biblioteką wykorzystywaną przez tę klasę jest Facebook C# SDK (obecna wersja 6.0.10.0) zawierająca namespace Facebook. Wykorzystywane są również System.Web oraz System.Web.Mvc. 
Metody:
- public static string LogIn(HttpRequestBase Request, UrlHelper Url)
Zwraca unikalny (dla naszej aplikacji) adres URL do strony logowania Facebooka. Do jego stworzenia są wykorzystywane m. in. ClientID i ClientSecret. Zawiera on również adres strony (RedirectUri), do której nastąpi przekierowanie po udanym logowaniu. Typ odpowiedzi od serwera jest ustawiony na "code", co oznacza, że dostajemy w informacji zwrotnej unikalny kod autoryzacyjny (jest on przekazywany jako parametr do metody wywoływanej przy otrzymaniu odpowiedzi).
HttpRequestBase, UrlHelper Url- obiekty potrzebne do utworzenia RedirectUri
- public static dynamic HandleFacebookCallback(string code, HttpResponseBase Response, HttpRequestBase Request, UrlHelper Url)
Zwraca dynamiczny obiekt zawierający informacje o użytkowniku, który właśnie się zalogował do Facebooka. Metoda ta najpierw wysyła zapytanie do Facebooka w celu uzyskania Access Tokena, później pobiera informacje o zalogowanym użytkowniku a na końcu zapisuje AuthCookie, zawierające informacje o użytkowniku.
string code- kod otrzymany zaraz po zalogowaniu na stronie Facebooka,
HttpResponseBase Response- miejsce zapisu AuthCookie,
HttpRequestBase Request, UrlHelper Url- obiekty potrzebne do utworzenia RedirectUri.
- public static string LogOut(HttpRequestBase Request)
Zwraca adres, który wyloguje nas z Facebook'a a następnie przekieruje nas na stronę domową aplikacji. Metoda usuwa zapamiętane AuthCookie oraz wylogowuje użytkownika z jego konta na Facebooku.
HttpRequestBase Request- potrzebne do utworzenia adresu wylogowania oraz pobrania i usunięcia zapamiętanego AuthCookie.

Uwagi:
Wewnętrzna klasa prywatna CredentialsAndVariables zawiera pola z danymi niezbędnymi do działania logowania. 
ClientID, ClientSecret- stringi przyznane dla aplikacji przez Facebooka.
LogOutRedirect- adres przekierowania po wylogowaniu.
CallbackAction- nazwa akcji z kontrolera wywoływanej jako callback po zalogowaniu.
Scope- zestaw działań, na jakie otrzyma pozwolenie aplikacja (np. dostęp do listy znajomych, pisanie na osi czasu itp. Lista scope dostępna na stronach deweloperskich Facebooka).

b) GooglePlusWrapper- zawiera metody umożliwiające komunikację z Google+ w celu autentykacji.
⦁	W celach autentykacji korzystamy z zestawu bibliotek DotNetOpenAuth (obecna wersja 4.0.0.11165 oraz 4.3.0.0) oraz bibliotek Google.Apis (WAŻNE! wykorzystujemy wersję 0.9.4, która nie jest najnowszą implementacją Google Api - nowsze wersje znacznie się różnią, nie wszystkie części tej biblioteki występują w danej wersji, co powoduje problemy kompilacyjne). Wykorzystujemy bibliotekę RestSharp (obecna wersja 104.3.3.0) w celu pobrania informacji o użytkowniku.
Metody:
public static string LogIn()
Zwraca adres do strony domowej naszej aplikacji. Metoda tworzy serwis logowania (_service) w tym obiekt dokonujący autentykacji (do którego przekazywany jest ClientId oraz ClientSecret). Wraz z próbą odwołania się do konta użytkownika zostajemy przekierowani na stronę logowania do konta Google+. 

public static string HandleGoogleCallback(HttpResponseBase Response)
Zwraca adres do strony domowej naszej aplikacji. Z wykorzystaniem Access Token'a, za pośrednictwem biblioteki RestSharp wysyłamy zapytanie do Google+, które zwraca string w formacie JSON z informacjami o użytkowniku autentykowanym przez Access Token'a. Informacje te są zapisywane w AuthCookie analogicznie jak w przypadku Facebook'a.
HttpResponseBase Response - miejsce zapisu AuthCookie.

public static string LogOut()
Zwraca adres, który wyloguje nas z Google+ a następnie przekieruje nas na stronę domową aplikacji. Metoda usuwa zapamiętane AuthCookie oraz wylogowuje użytkownika z jego konta na Google+.

Uwagi:
Wewnętrzna klasa prywatna CredentialsAndVariables zawiera pola z danymi niezbędnymi do działania logowania. 
ClientID, ClientSecret- stringi przyznane dla aplikacji przez Google.
LogOutRedirect- adres przekierowania po wylogowaniu.
CallbackActionUrl- adres wywoływany jako callback po zalogowaniu.
ReturnHome - adres strony domowej aplikacji.


c) TwitterWrapper- zawiera metody umożliwiające komunikację z Twitter'em w celu autentykacji.
⦁	Najważniejszą biblioteką wykorzystywaną przez tę klasę jest TweetSharp(obecna nieoficjalna wersja 2.3.1.2) zawierająca namespace TweetSharp. Wykorzystywane są również System.Web oraz System.Web.Mvc. 
Metody:
- internal static string LogIn()
Zwraca unikalny (dla naszej aplikacji) adres URL do strony logowania Twitter'a. Do jego stworzenia są wykorzystywane m. in. ClientID i ClientSecret. Ustawia RequestToken'a który będzie wykorzystywany do otrzymania AccessToken'a. Otrzymujemy OAuthVerifier który weryfikuje użytkownika.

- internal static string HandleTwitterCallback(HttpResponseBase Response, HttpRequestBase Request)
Metoda ta najpierw wysyła zapytanie do Twitter'a w celu uzyskania Access Tokena, później pobiera informacje o zalogowanym użytkowniku a na końcu zapisuje AuthCookie, zawierające informacje o użytkowniku.
HttpResponseBase Response- miejsce zapisu AuthCookie,
HttpRequestBase Request- wykorzystywany do pobrania OAuthVerifier'a.
- internal static string LogOut()
Zwraca adres, który przekieruje nas na stronę domową aplikacji. Metoda usuwa zapamiętane AuthCookie oraz wylogowuje użytkownika z aplikacji.

Uwagi:
Wewnętrzna klasa prywatna CredentialsAndVariables zawiera pola z danymi niezbędnymi do działania logowania. 
ClientID, ClientSecret- stringi przyznane dla aplikacji przez Twittera.
LogOutRedirect- adres powrotny po wylogowaniu z aplikacji.
CallbackAction- nazwa akcji, do której nastapi powrót ze strony logowania Twittera.
!!!Metoda LogOut nie wylogowuje użytkownika z jego konta na Twitter'rze.


4. Opis działania filtra autoryzującego dostęp do aplikacji.
Klasa public class CustomAuthAttribute : AuthorizeAttribute jest odpowiedzialna za filtrowanie użytkowników, którzy zalogowali się pomyślnie przez jeden z portali społecznościowych, lecz nie powinni uzyskać autoryzacji w aplikacji.
Metody filtra są wywoływane przy każdym odwołaniu do strony. Jest za to odpowiedzialna metoda OnAuthorization.
Metodą AuthorizeCore sprawdza się, czy użytkownik powinien zostać zautoryzowany i mieć dostęp do aplikacji. Na początku odczytywane są dane z AuthCookie. Następnie powinna zostać zaimplementowana własna obsługa autoryzacji.  
Metoda HandleUnauthorizedRequest jest wywoływana, gdy do aplikacji próbował się zalogować nieautoryzowany użytkownik. Jeżeli użytkownik jest niezalogowany, wtedy następuje przekierowanie na stronę logowania. Jeżeli natomiast nastąpiło logowanie poprzez jeden z portali społecznościowych, a użytkownik nie został rozpoznany, wykonywane jest wylogowanie z jego konta.

5. Przykładowa implementacja AccountController'a:

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

            return Redirect("/Home/Index");   
        }
   }