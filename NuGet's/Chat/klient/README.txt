Aby uruchomi� fukcj� chatu, wykonaj nast�puj�ce czynno�ci:

-> dodaj nuget 'Chat' do projektu

-> dodaj referencje do skryptow: 
	*jquery
	*jquery-ui
	*jquery.signalR
	*http://zos-srv/chatserver/signalr/hubs
	*~/Scripts/chat/chat.js


Uwaga: Nalezy ustawi� poprawny adres url dla signalr/hubs !!!

-> dodaj referencje do styli

   <link rel="stylesheet" type="text/css" href="~/Content/chat/chat.css" />


-> ustaw �cie�k� dla Huba w chat.js:

connection = $.hubConnection('http://zos-srv/chatserver'); //adres huba
chatHub = connection.createHubProxy('ChatHub'); //nazwa huba

-> na stronie, na kt�rej chcesz uruchomi� chat dodaj texboxy :
 	* <input id="chatName" type="text" >  dla nazwy u�ytkownika
	* <input id="chatId" type="text"/> dla id u�ytkownika (id po kt�rym b�dzie on wyszukiwany w bazie)
	* <button type="button" onclick="addToChat()"> Zaloguj </button> - przycisk dodaj�cy u�ytkownika do chatu

Uwaga:
Nazw� u�ytkownika oraz jego numer id mo�esz r�wnie� przekazywa� w inny spos�b. Wystarczy zmodyfikowa� pocz�tkowy fragment skryptu 'chat.js', przypisuj�c inne warto�ci dla zmiennych sesyjnych:

	sessionStorage.setItem("name", $('#chatName').val());
    	sessionStorage.setItem("id", $('#chatId').val());
    
-> dodaj na stronie divy

	* <div id="divDraggable"></div> - w tym divie pokaze sie okienko prywatnej wiadomosci (najlepiej div na ca�� stron�)
	* <div id="chat"> </div> - w tym divie pojawi sie chat
