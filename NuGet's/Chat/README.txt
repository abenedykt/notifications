Aby uruchomi� fukcj� chatu, wykonaj nast�puj�ce czynno�ci:

-> dodaj nuget 'Chat' do projektu

-> dodaj skrypty: 
	*jquery
	*jquery-ui
	*jquery.signalR
	*<script src='@<namespace>.chatConfigurationClass.GetConfig().chatHub.Url'> </script>
	*~/Scripts/chat/chat.js
	*script>
        addToChat("marian kowalski", "mkowalski", '@<namespace>.chatConfigurationClass.GetConfig().service.Url')
    </script>

gdzie <namespace> to nazwa Twojego projektu

-> dodaj referencje do styli

   <link rel="stylesheet" type="text/css" href="~/Content/chat/chat.css" />

-> w web.configu dodaj w <configSections>:

<section name="chatConfiguration" type="<namespace>.chatConfigurationClass" requirePermission="false" />

-> w web.configu dodaj w <configuration>:

<chatConfiguration>
    <service url="http://zos-srv/chatserver"/>
    <chatHub url="http://zos-srv/chatserver/signalr/hubs"/>
 </chatConfiguration>

-> ustaw �cie�k� dla serwera z hubem

-> dodaj na stronie divy

	* <div id="divDraggable"></div> - w tym divie pokaze sie okienko prywatnej wiadomosci (najlepiej div na ca�� stron�)
	* <div id="chat"> </div> - w tym divie pojawi sie chat
