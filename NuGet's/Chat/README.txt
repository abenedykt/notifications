Aby uruchomi� fukcj� chatu, wykonaj nast�puj�ce czynno�ci:


-> dodaj nuget 'Chat' do projektu


-> dodaj referencje do skryptow:

<script src="~/Scripts/jquery-1.6.4.js" type="text/javascript"> </script>
<script src="~/Scripts/jquery-ui-1.8.11.js"> </script>
<script src="~/Scripts/jquery.signalR-1.1.3.js"> </script>
<script src="~/signalr/hubs"> </script>
<script src="~/Scripts/chat/chat.js" type="text/javascript"> </script>


-> dodaj referencje do styli

   <link rel="stylesheet" type="text/css" href="~/Content/chat/chat.css" />
   <link rel="stylesheet" type="text/css" href="~/Content/chat/draggableWindowChat.css" />
   <link rel="stylesheet" type="text/css" href="~/Content/themes/base/jquery.ui.all.css" />


-> dodaj do global.asax

RouteTable.Routes.MapHubs();


-> w hubie ChatHub.cs zmie� parametry konstruktora dla RavenRepository, podaj�c adres, gdzie znajduje si� baza danych, oraz nazw� bazy:

_application = new ChatApplication(new Factory(new RavenRepository("http://localhost:8080", "chat")));


-> na stronie, na kt�rej chcesz uruchomi� chat dodaj texboxy :
 	* <input id="chatName" type="text" > - dla nazwy u�ytkownika
	* <input id="chatId" type="text"/> -dla id u�ytkownika (id po kt�rym b�dzie on wyszukiwany w bazie)
	* <button type="button" onclick="addToChat()"> Zaloguj </button> - przycisk dodaj�cy u�ytkownika do chatu

Uwaga:
Nazw� u�ytkownika oraz jego numer id mo�esz r�wnie� przekazywa� w inny spos�b. Wystarczy zmodyfikowa� pocz�tkowy fragment skryptu 'chat.js', przypisuj�c inne warto�ci dla zmiennych sesyjnych:

	sessionStorage.setItem("name", $('#chatName').val());
    	sessionStorage.setItem("id", $('#chatId').val());


-> dodaj na stronie divy

	* <div id="divDraggable"></div> - w tym divie pokaze sie okienko prywatnej wiadomosci (najlepiej div na ca�� stron�)
	* <div id="chat"> </div> - w tym divie pojawi sie chat
