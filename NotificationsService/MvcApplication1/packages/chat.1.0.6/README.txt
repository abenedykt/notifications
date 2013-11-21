Aby uruchomiæ fukcjê chatu, wykonaj nastêpuj¹ce czynnoœci:

-> dodaj nuget 'Chat' do projektu

-> dodaj skrypty: 
	*jquery
	*jquery-ui
	*signalR.Client

-> dodaj referencje do styli Content/chat/chat.css

-> w web.configu dodaj w <configSections>:

<section name="chatConfiguration" type="ChatConfig.chatConfigurationClass" requirePermission="false" />

-> w web.configu dodaj w <configuration>:

<chatConfiguration>
    <service url="http://zos-srv/chatserver"/>
    <chatHub url="http://zos-srv/chatserver/signalr/hubs"/>
</chatConfiguration>

-> ustaw œcie¿kê dla serwera z hubem

->aby zalogowc sie do chatu, na stronie umieœc:

 @Chat.login(<nazwa_uzytkownika>, <id_u¿ytkownika>);

-> dodaj na stronie divy dla przycisku chatu oraz miejsca, gdzie ma wyskakiwaæ okienko prywatnej wiadomoœci

	<div id="divDraggable"></div> 
	<div id="chat"> </div> 
