Aby uruchomiæ fukcjê chatu, wykonaj nastêpuj¹ce czynnoœci:

-> dodaj nuget 'Chat' do projektu

-> dodaj referencje do skryptow:

 <script src="~/Scripts/jquery-1.8.2.js" type="text/javascript"> </script>
    <script src="~/Scripts/jquery-ui-1.8.24.js"> </script>
    <script src="~/Scripts/jquery.signalR-2.0.0.js"> </script>
    <script src="http://localhost:59537/signalr/hubs"> </script> 
    <script src="~/Scripts/chat/chat.js" type="text/javascript"> </script>

Uwaga: Nalezy ustawiæ poprawny adres url dla signalr/hubs !!!

-> dodaj referencje do styli

   <link rel="stylesheet" type="text/css" href="~/Content/chat/chat.css" />

-> dodaj plik startup.cs

[assembly: OwinStartup(typeof(MvcApp1.Startup))]

namespace MvcApp1
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapHubs();
        }
    }
}


-> ustaw œcie¿kê dla Huba w chat.js:

connection = $.hubConnection('http://localhost:59537/'); //adres huba
chatHub = connection.createHubProxy('ChatHub'); //nazwa huba

-> na stronie, na której chcesz uruchomiæ chat dodaj texboxy :
 	* <input id="chatName" type="text" >  dla nazwy u¿ytkownika
	* <input id="chatId" type="text"/> dla id u¿ytkownika (id po którym bêdzie on wyszukiwany w bazie)
	* <button type="button" onclick="addToChat()"> Zaloguj </button> - przycisk dodaj¹cy u¿ytkownika do chatu

Uwaga:
Nazwê u¿ytkownika oraz jego numer id mo¿esz równie¿ przekazywaæ w inny sposób. Wystarczy zmodyfikowaæ pocz¹tkowy fragment skryptu 'chat.js', przypisuj¹c inne wartoœci dla zmiennych sesyjnych:

	sessionStorage.setItem("name", $('#chatName').val());
    	sessionStorage.setItem("id", $('#chatId').val());
    
-> dodaj na stronie divy

	* <div id="divDraggable"></div> - w tym divie pokaze sie okienko prywatnej wiadomosci (najlepiej div na ca³¹ stronê)
	* <div id="chat"> </div> - w tym divie pojawi sie chat
