Aby uruchomiæ fukcjê chatu, wykonaj nastêpuj¹ce czynnoœci:

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

-> ustaw œcie¿kê dla serwera z hubem

->aby zalogowc sie do chatu, na stronie umieœc:

 @Chat.Login(<nazwa_uzytkownika>, <id_u¿ytkownika>);
