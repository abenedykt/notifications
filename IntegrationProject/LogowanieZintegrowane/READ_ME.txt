1. Kr�tki opis.

Logowanie zintegrowane umo�liwia autentykacj� poprzez Facebooka oraz Google+. Zawiera gotowe statyczne klasy FacebookWrapper oraz GooglePlusWrapper. Dzia�anie opiera si� na zasadzie przekierowania na stron� logowania danego portalu spo�eczno�ciowego oraz oczekiwania na wiadomo�� zwrotn� (Callback). Informacj� zwrotn� jest Access Token, kt�ry umo�liwia nam komunikacj� z portalami w celu np. otrzymania informacji o u�ytkowniku. Informacje o zautoryzowanym u�ytkowniku wraz z jego Access Tokenem przechowywane s� w AuthCookie.


2. Potrzebne dane przed implementacj�.

Aby mo�liwe by�o korzystanie z zewn�trznej autentykacji nale�y utworzy� now� aplikacj� na deweloperskim koncie Facebooka/Google+. Tam przyznane zostan� takie informacje jak ClientID i ClientSecret (ci�gi znak�w), kt�re s� niezb�dne do poprawnego dzia�ania logowania. W tym samym miejscu nale�y r�wnie� ustawi� adres hosta (najlepiej wraz z portem), z kt�rego dokonywane jest przekierowanie na zewn�trzn� stron� logowania.


3. Opis klas
a) FacebookWrapper- zawiera metody umo�liwiaj�ce komunikacj� z Facebookiem w celu autentykacji.
?	Najwa�niejsz� bibliotek� wykorzystywan� przez t� klas� jest Facebook C# SDK (obecna wersja 6.0.10.0) zawieraj�ca namespace Facebook. Wykorzystywane s� r�wnie� System.Web oraz System.Web.Mvc. 
Metody:
- public static string LogIn(HttpRequestBase Request, UrlHelper Url)
Zwraca unikalny (dla naszej aplikacji) adres URL do strony logowania Facebooka. Do jego stworzenia s� wykorzystywane m. in. ClientID i ClientSecret. Zawiera on r�wnie� adres strony (RedirectUri), do kt�rej nast�pi przekierowanie po udanym logowaniu. Typ odpowiedzi od serwera jest ustawiony na "code", co oznacza, �e dostajemy w informacji zwrotnej unikalny kod autoryzacyjny (jest on przekazywany jako parametr do metody wywo�ywanej przy otrzymaniu odpowiedzi).
HttpRequestBase, UrlHelper Url- obiekty potrzebne do utworzenia RedirectUri
- public static dynamic HandleFacebookCallback(string code, HttpResponseBase Response, HttpRequestBase Request, UrlHelper Url)
Zwraca dynamiczny obiekt zawieraj�cy informacje o u�ytkowniku, kt�ry w�a�nie si� zalogowa� do Facebooka. Metoda ta najpierw wysy�a zapytanie do Facebooka w celu uzyskania Access Tokena, p�niej pobiera informacje o zalogowanym u�ytkowniku a na ko�cu zapisuje AuthCookie, zawieraj�ce informacje o u�ytkowniku.
string code- kod otrzymany zaraz po zalogowaniu na stronie Facebooka,
HttpResponseBase Response- miejsce zapisu AuthCookie,
HttpRequestBase, UrlHelper Url- obiekty potrzebne do utworzenia RedirectUri.
- public static string LogOut(HttpRequestBase Request)
Zwraca adres, kt�ry wyloguje nas z Facebook'a a nast�pnie przekieruje nas na stron� domow� aplikacji. Metoda usuwa zapami�tane AuthCookie oraz wylogowuje u�ytkownika z jego konta na Facebooku.
HttpRequestBase Request- potrzebne do utworzenia adresu wylogowania oraz pobrania i usuni�cia zapami�tanego AuthCookie.

Uwagi:
Wewn�trzna klasa prywatna CredentialsAndVariables zawiera pola z danymi niezb�dnymi do dzia�ania logowania. 
ClientID, ClientSecret- stringi przyznane dla aplikacji przez Facebooka.
LogOutRedirect- adres przekierowania po wylogowaniu.
CallbackAction- nazwa akcji z kontrolera wywo�ywanej jako callback po zalogowaniu.
Scope- zestaw dzia�a�, na jakie otrzyma pozwolenie aplikacja (np. dost�p do listy znajomych, pisanie na osi czasu itp. Lista scope dost�pna na stronach deweloperskich Facebooka).

b) GooglePlusWrapper- zawiera metody umo�liwiaj�ce komunikacj� z Google+ w celu autentykacji.
?	W celach autentykacji korzystamy z zestawu bibliotek DotNetOpenAuth (obecna wersja 4.0.0.11165 oraz 4.3.0.0) oraz bibliotek Google.Apis (WA�NE! wykorzystujemy wersj� 0.9.4, kt�ra nie jest najnowsz� implementacj� Google Api - nowsze wersje znacznie si� r�ni�, nie wszystkie cz�ci tej biblioteki wyst�puj� w danej wersji, co powoduje problemy kompilacyjne). Wykorzystujemy bibliotek� RestSharp (obecna wersja 104.3.3.0) w celu pobrania informacji o u�ytkowniku.
Metody:
public static string LogIn()
Zwraca adres do strony domowej naszej aplikacji. Metoda tworzy serwis logowania (_service) w tym obiekt dokonuj�cy autentykacji (do kt�rego przekazywany jest ClientId oraz ClientSecret). Wraz z pr�b� odwo�ania si� do konta u�ytkownika zostajemy przekierowani na stron� logowania do konta Google+. 

public static string HandleGoogleCallback(HttpResponseBase Response)
Zwraca adres do strony domowej naszej aplikacji. Z wykorzystaniem Access Token'a, za po�rednictwem biblioteki RestSharp wysy�amy zapytanie do Google+ kt�re zwraca string w formacie JSON z informacjami o u�ytkowniku autentykowanym przez Access Token'a. Informacje te s� zapisywane w AuthCookie analogicznie jak w przypadku Facebook'a.
HttpResponseBase Response - miejsce zapisu AuthCookie.

public static string LogOut()
Zwraca adres, kt�ry wyloguje nas z Google+ a nast�pnie przekieruje nas na stron� domow� aplikacji. Metoda usuwa zapami�tane AuthCookie oraz wylogowuje u�ytkownika z jego konta na Google+.

Uwagi:
Wewn�trzna klasa prywatna CredentialsAndVariables zawiera pola z danymi niezb�dnymi do dzia�ania logowania. 
ClientID, ClientSecret- stringi przyznane dla aplikacji przez Facebooka.
LogOutRedirect- adres przekierowania po wylogowaniu.
CallbackActionUrl- adres wywo�ywany jako callback po zalogowaniu.
Scopes- zestaw dzia�a�, na jakie otrzyma pozwolenie aplikacja (np. dost�p do listy znajomych, pisanie na osi czasu itp. Lista scope dost�pna na stronach deweloperskich Facebooka).
ReturnHome - adres strony domowej aplikacji.


4. Opis dzia�ania filtra autoryzuj�cego dost�p do aplikacji.
Klasa public class CustomAuthAttribute : AuthorizeAttribute jest odpowiedzialna za filtrowanie u�ytkownik�w, kt�rzy zalogowali si� pomy�lnie przez jeden z portali spo�eczno�ciowych, lecz nie powinni uzyska� autoryzacji w aplikacji.
Metody filtra s� wywo�ywane przy ka�dym odwo�aniu do strony. Jest za to odpowiedzialna metoda OnAuthorization.
Metod� AuthorizeCore sprawdza si�, czy u�ytkownik powinien zosta� zautoryzowany i mie� dost�p do aplikacji. Na pocz�tku odczytywane s� dane z AuthCookie. Nast�pnie powinna zosta� zaimplementowana w�asna obs�uga autoryzacji.  
Metoda HandleUnauthorizedRequest jest wywo�ywana, gdy do aplikacji pr�bowa� si� zalogowa� nieautoryzowany u�ytkownik. Je�eli u�ytkownik jest niezalogowany, wtedy nast�puje przekierowanie na stron� logowania. Je�eli natomiast nast�pi�o logowanie poprzez jeden z portali spo�eczno�ciowych, a u�ytkownik nie zosta� rozpoznany, wykonywane jest wylogowanie z jego konta.

5. Przyk�adowa implementacja AccountController'a:

public class AccountController : Controller
    {
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

            return Redirect("/Home/Index");   
        }
   }