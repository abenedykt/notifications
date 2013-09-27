Aby uruchomiæ fukcjê chatu, wykonaj nastêpuj¹ce czynnoœci:


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


-> w hubie ChatHub.cs zmieñ parametry konstruktora dla RavenRepository, podaj¹c adres, gdzie znajduje siê baza danych, oraz nazwê bazy:

_application = new ChatApplication(new Factory(new RavenRepository("http://localhost:8080", "chat")));


-> na stronie, na której chcesz uruchomiæ chat dodaj texboxy :
 	* <input id="chatName" type="text" > - dla nazwy u¿ytkownika
	* <input id="chatId" type="text"/> -dla id u¿ytkownika (id po którym bêdzie on wyszukiwany w bazie)
	* <button type="button" onclick="addToChat()"> Zaloguj </button> - przycisk dodaj¹cy u¿ytkownika do chatu

Uwaga:
Nazwê u¿ytkownika oraz jego numer id mo¿esz równie¿ przekazywaæ w inny sposób. Wystarczy zmodyfikowaæ pocz¹tkowy fragment skryptu 'chat.js', przypisuj¹c inne wartoœci dla zmiennych sesyjnych:

	sessionStorage.setItem("name", $('#chatName').val());
    	sessionStorage.setItem("id", $('#chatId').val());


-> dodaj na stronie divy

	* <div id="divDraggable"></div> - w tym divie pokaze sie okienko prywatnej wiadomosci (najlepiej div na ca³¹ stronê)
	* <div id="chat"> </div> - w tym divie pojawi sie chat
