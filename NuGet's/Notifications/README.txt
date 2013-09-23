Aby uruchomiæ fukcjê powiadomieñ, wykonaj nastêpuj¹ce czynnoœci:

-> dodaj nuget 'Notifications' do projektu

-> dodaj referencje do skryptow:

<script src="~/Scripts/jquery-2.0.3.js" type="text/javascript"> </script>
<script src="~/Scripts/jquery.signalR-1.1.3.js"> </script>
<script src="~/signalr/hubs"> </script>
<script src="~/Scripts/notifications.js" type="text/javascript" hidden=""> </script>

-> dodaj referencje do styli

    <link rel="stylesheet" type="text/css" href="~/Content/jquery.pnotify.css" />
    <link rel="stylesheet" type="text/css" href="~/Content/jquery.pnotify.icons.css" />

-> zarejestruj typy dla Dependency Injection( przyk³ad dla Autofac'a):

	private static void AutofacConfiguration()
        {
            var builder = new ContainerBuilder();

            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            builder.RegisterType<RavenRepository>().As<IDataRepository>();
            builder.RegisterType<ChatApplication>().As<IChatApplication>();
            builder.RegisterType<Factory>().As<IFactory>();
        
            var container = builder.Build();
            var resolver = new AutofacDependencyResolver(container);
            GlobalHost.DependencyResolver = resolver;
        }

Uwaga!!!
Stosuj¹c Autofac, wymagany jest równie¿ Autofac SignalR Integration

-> dodaj do global.asax

RouteTable.Routes.MapHubs();

-> na stronie, na której chcesz mieæ interfejs do obs³ugi powiadomieñ dodaj:
 	* <input id="notesName" type="text" > - dla nazwy u¿ytkownika
	* <input id="notesId" type="text"/> - dla id u¿ytkownika ( id po którym bêdzie on wyszukiwany w bazie)
	* <button type="button" onclick="addToNotes()"> Zaloguj </button> - przycisk dodaj¹cy u¿ytkownika do chatu
	* <div id="ActiveUsersNotification"> - div wyœwietlaj¹cy aktywnych u¿ytkowników z mozliwoœci¹ zaznaczenia, do których ma byæ wys³ane powiadomienie
	* <textarea id="txtNotification"> - miejsce na wpisanie treœci wiadomoœci
	* <button id="sendNotification"> - przycisk wysy³aj¹cy powiadomienie
	* <ul id="receivedNotifications" /> - lista wyœwietlaj¹ca historiê otrzymanych powiadomieñ
	* <ul id="sendNotifications" /> - lista wyœwietlaj¹ca historie wys³anych powiadomieñ

Uwaga:
Nazwê u¿ytkownika oraz jego numer id mo¿esz równie¿ przekazywaæ w inny sposób. Wystarczy zmodyfikowaæ pocz¹tkowy fragment skryptu 'chat.js', przypisuj¹c zmiennym inne wartoœci:

	var userName = $('#notesName').val();
    	var userId = $('#notesId').val();

