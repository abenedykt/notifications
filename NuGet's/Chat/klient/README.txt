Aby uruchomiæ fukcjê chatu, wykonaj nastêpuj¹ce czynnoœci:

-> dodaj nuget 'Chat' do projektu

-> dodaj referencje do skryptow: 
	*jquery
	*jquery-ui
	*jquery.signalR
	*http://zos-srv/chatserver/signalr/hubs
	*~/Scripts/chat/chat.js


Uwaga: Nalezy ustawiæ poprawny adres url dla signalr/hubs !!!

-> dodaj referencje do styli

   <link rel="stylesheet" type="text/css" href="~/Content/chat/chat.css" />


-> ustaw œcie¿kê dla Huba w chat.js:

connection = $.hubConnection('http://zos-srv/chatserver'); //adres huba
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
