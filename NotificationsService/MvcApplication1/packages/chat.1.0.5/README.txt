Aby uruchomi� fukcj� chatu, wykonaj nast�puj�ce czynno�ci:

-> dodaj nuget 'Chat' do projektu

-> dodaj na stronie skrypty: 
	*jquery
	*jquery-ui
	*signalR

-> w web.configu dodaj w <configSections>:

<section name="chatConfiguration" type="ChatConfig.ChatConfigurationClass" requirePermission="false" />

-> w web.configu dodaj w <configuration>:

<chatConfiguration>
    <service url="http://zos-srv/chatserver"/>
</chatConfiguration>

-> ustaw �cie�k� dla serwera z hubem

->aby zalogowc sie do chatu, na stronie umie�c:

 @Chat.Login(<nazwa_uzytkownika>, <id_u�ytkownika>);
