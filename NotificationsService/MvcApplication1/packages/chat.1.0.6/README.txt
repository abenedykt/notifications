Aby uruchomi� fukcj� chatu, wykonaj nast�puj�ce czynno�ci:

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

-> ustaw �cie�k� dla serwera z hubem

->aby zalogowc sie do chatu, na stronie umie�c:

 @Chat.login(<nazwa_uzytkownika>, <id_u�ytkownika>);

-> dodaj na stronie divy dla przycisku chatu oraz miejsca, gdzie ma wyskakiwa� okienko prywatnej wiadomo�ci

	<div id="divDraggable"></div> 
	<div id="chat"> </div> 
