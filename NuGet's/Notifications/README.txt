Aby uruchomi� fukcj� powiadomie�, wykonaj nast�puj�ce czynno�ci:

-> dodaj nuget 'Notifications' do projektu

-> dodaj referencje do skryptow:

<script src="~/Scripts/jquery-2.0.3.js" type="text/javascript"> </script>
<script src="~/Scripts/jquery.signalR-1.1.3.js"> </script>
<script src="~/signalr/hubs"> </script>
<script src="~/Scripts/notifications.js" type="text/javascript" hidden=""> </script>

-> dodaj referencje do styli

    <link rel="stylesheet" type="text/css" href="~/Content/jquery.pnotify.css" />
    <link rel="stylesheet" type="text/css" href="~/Content/jquery.pnotify.icons.css" />

-> zarejestruj typy dla Dependency Injection( przyk�ad dla Autofac'a):

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
Stosuj�c Autofac, wymagany jest r�wnie� Autofac SignalR Integration

-> dodaj do global.asax

RouteTable.Routes.MapHubs();

-> na stronie, na kt�rej chcesz mie� interfejs do obs�ugi powiadomie� dodaj:
 	* <input id="notesName" type="text" > - dla nazwy u�ytkownika
	* <input id="notesId" type="text"/> - dla id u�ytkownika ( id po kt�rym b�dzie on wyszukiwany w bazie)
	* <button type="button" onclick="addToNotes()"> Zaloguj </button> - przycisk dodaj�cy u�ytkownika do chatu
	* <div id="ActiveUsersNotification"> - div wy�wietlaj�cy aktywnych u�ytkownik�w z mozliwo�ci� zaznaczenia, do kt�rych ma by� wys�ane powiadomienie
	* <textarea id="txtNotification"> - miejsce na wpisanie tre�ci wiadomo�ci
	* <button id="sendNotification"> - przycisk wysy�aj�cy powiadomienie
	* <ul id="receivedNotifications" /> - lista wy�wietlaj�ca histori� otrzymanych powiadomie�
	* <ul id="sendNotifications" /> - lista wy�wietlaj�ca historie wys�anych powiadomie�

Uwaga:
Nazw� u�ytkownika oraz jego numer id mo�esz r�wnie� przekazywa� w inny spos�b. Wystarczy zmodyfikowa� pocz�tkowy fragment skryptu 'chat.js', przypisuj�c zmiennym inne warto�ci:

	var userName = $('#notesName').val();
    	var userId = $('#notesId').val();

