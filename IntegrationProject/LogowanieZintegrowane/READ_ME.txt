1. Krótki opis.

Logowanie zintegrowane umo¿liwia autentykacjê poprzez Facebooka oraz Google+. Zawiera gotowe statyczne klasy FacebookWrapper oraz GooglePlusWrapper. Dzia³anie opiera siê na zasadzie przekierowania na stronê logowania danego portalu spo³ecznoœciowego oraz oczekiwania na wiadomoœæ zwrotn¹ (Callback). Informacj¹ zwrotn¹ jest Access Token, który umo¿liwia nam komunikacjê z portalami w celu np. otrzymania informacji o u¿ytkowniku. Informacje o zautoryzowanym u¿ytkowniku wraz z jego Access Tokenem przechowywane s¹ w AuthCookie.


2. Potrzebne dane przed implementacj¹.

Aby mo¿liwe by³o korzystanie z zewnêtrznej autentykacji nale¿y utworzyæ now¹ aplikacjê na deweloperskim koncie Facebooka/Google+. Tam przyznane zostan¹ takie informacje jak ClientID i ClientSecret (ci¹gi znaków), które s¹ niezbêdne do poprawnego dzia³ania logowania. W tym samym miejscu nale¿y równie¿ ustawiæ adres hosta (najlepiej wraz z portem), z którego dokonywane jest przekierowanie na zewnêtrzn¹ stronê logowania.


3. Opis klas
a) FacebookWrapper- zawiera metody umo¿liwiaj¹ce komunikacjê z Facebookiem w celu autentykacji.
?	Najwa¿niejsz¹ bibliotek¹ wykorzystywan¹ przez tê klasê jest Facebook C# SDK (obecna wersja 6.0.10.0) zawieraj¹ca namespace Facebook. Wykorzystywane s¹ równie¿ System.Web oraz System.Web.Mvc. 
Metody:
- public static string LogIn(HttpRequestBase Request, UrlHelper Url)
Zwraca unikalny (dla naszej aplikacji) adres URL do strony logowania Facebooka. Do jego stworzenia s¹ wykorzystywane m. in. ClientID i ClientSecret. Zawiera on równie¿ adres strony (RedirectUri), do której nast¹pi przekierowanie po udanym logowaniu. Typ odpowiedzi od serwera jest ustawiony na "code", co oznacza, ¿e dostajemy w informacji zwrotnej unikalny kod autoryzacyjny (jest on przekazywany jako parametr do metody wywo³ywanej przy otrzymaniu odpowiedzi).
HttpRequestBase, UrlHelper Url- obiekty potrzebne do utworzenia RedirectUri
- public static dynamic HandleFacebookCallback(string code, HttpResponseBase Response, HttpRequestBase Request, UrlHelper Url)
Zwraca dynamiczny obiekt zawieraj¹cy informacje o u¿ytkowniku, który w³aœnie siê zalogowa³ do Facebooka. Metoda ta najpierw wysy³a zapytanie do Facebooka w celu uzyskania Access Tokena, póŸniej pobiera informacje o zalogowanym u¿ytkowniku a na koñcu zapisuje AuthCookie, zawieraj¹ce informacje o u¿ytkowniku.
string code- kod otrzymany zaraz po zalogowaniu na stronie Facebooka,
HttpResponseBase Response- miejsce zapisu AuthCookie,
HttpRequestBase, UrlHelper Url- obiekty potrzebne do utworzenia RedirectUri.
- public static string LogOut(HttpRequestBase Request)
Zwraca adres, który wyloguje nas z Facebook'a a nastêpnie przekieruje nas na stronê domow¹ aplikacji. Metoda usuwa zapamiêtane AuthCookie oraz wylogowuje u¿ytkownika z jego konta na Facebooku.
HttpRequestBase Request- potrzebne do utworzenia adresu wylogowania oraz pobrania i usuniêcia zapamiêtanego AuthCookie.

Uwagi:
Wewnêtrzna klasa prywatna CredentialsAndVariables zawiera pola z danymi niezbêdnymi do dzia³ania logowania. 
ClientID, ClientSecret- stringi przyznane dla aplikacji przez Facebooka.
LogOutRedirect- adres przekierowania po wylogowaniu.
CallbackAction- nazwa akcji z kontrolera wywo³ywanej jako callback po zalogowaniu.
Scope- zestaw dzia³añ, na jakie otrzyma pozwolenie aplikacja (np. dostêp do listy znajomych, pisanie na osi czasu itp. Lista scope dostêpna na stronach deweloperskich Facebooka).

b) GooglePlusWrapper- zawiera metody umo¿liwiaj¹ce komunikacjê z Google+ w celu autentykacji.
?	W celach autentykacji korzystamy z zestawu bibliotek DotNetOpenAuth (obecna wersja 4.0.0.11165 oraz 4.3.0.0) oraz bibliotek Google.Apis (WA¯NE! wykorzystujemy wersjê 0.9.4, która nie jest najnowsz¹ implementacj¹ Google Api - nowsze wersje znacznie siê ró¿ni¹, nie wszystkie czêœci tej biblioteki wystêpuj¹ w danej wersji, co powoduje problemy kompilacyjne). Wykorzystujemy bibliotekê RestSharp (obecna wersja 104.3.3.0) w celu pobrania informacji o u¿ytkowniku.
Metody:
public static string LogIn()
Zwraca adres do strony domowej naszej aplikacji. Metoda tworzy serwis logowania (_service) w tym obiekt dokonuj¹cy autentykacji (do którego przekazywany jest ClientId oraz ClientSecret). Wraz z prób¹ odwo³ania siê do konta u¿ytkownika zostajemy przekierowani na stronê logowania do konta Google+. 

public static string HandleGoogleCallback(HttpResponseBase Response)
Zwraca adres do strony domowej naszej aplikacji. Z wykorzystaniem Access Token'a, za poœrednictwem biblioteki RestSharp wysy³amy zapytanie do Google+ które zwraca string w formacie JSON z informacjami o u¿ytkowniku autentykowanym przez Access Token'a. Informacje te s¹ zapisywane w AuthCookie analogicznie jak w przypadku Facebook'a.
HttpResponseBase Response - miejsce zapisu AuthCookie.

public static string LogOut()
Zwraca adres, który wyloguje nas z Google+ a nastêpnie przekieruje nas na stronê domow¹ aplikacji. Metoda usuwa zapamiêtane AuthCookie oraz wylogowuje u¿ytkownika z jego konta na Google+.

Uwagi:
Wewnêtrzna klasa prywatna CredentialsAndVariables zawiera pola z danymi niezbêdnymi do dzia³ania logowania. 
ClientID, ClientSecret- stringi przyznane dla aplikacji przez Facebooka.
LogOutRedirect- adres przekierowania po wylogowaniu.
CallbackActionUrl- adres wywo³ywany jako callback po zalogowaniu.
Scopes- zestaw dzia³añ, na jakie otrzyma pozwolenie aplikacja (np. dostêp do listy znajomych, pisanie na osi czasu itp. Lista scope dostêpna na stronach deweloperskich Facebooka).
ReturnHome - adres strony domowej aplikacji.


4. Opis dzia³ania filtra autoryzuj¹cego dostêp do aplikacji.
Klasa public class CustomAuthAttribute : AuthorizeAttribute jest odpowiedzialna za filtrowanie u¿ytkowników, którzy zalogowali siê pomyœlnie przez jeden z portali spo³ecznoœciowych, lecz nie powinni uzyskaæ autoryzacji w aplikacji.
Metody filtra s¹ wywo³ywane przy ka¿dym odwo³aniu do strony. Jest za to odpowiedzialna metoda OnAuthorization.
Metod¹ AuthorizeCore sprawdza siê, czy u¿ytkownik powinien zostaæ zautoryzowany i mieæ dostêp do aplikacji. Na pocz¹tku odczytywane s¹ dane z AuthCookie. Nastêpnie powinna zostaæ zaimplementowana w³asna obs³uga autoryzacji.  
Metoda HandleUnauthorizedRequest jest wywo³ywana, gdy do aplikacji próbowa³ siê zalogowaæ nieautoryzowany u¿ytkownik. Je¿eli u¿ytkownik jest niezalogowany, wtedy nastêpuje przekierowanie na stronê logowania. Je¿eli natomiast nast¹pi³o logowanie poprzez jeden z portali spo³ecznoœciowych, a u¿ytkownik nie zosta³ rozpoznany, wykonywane jest wylogowanie z jego konta.

5. Przyk³adowa implementacja AccountController'a:

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