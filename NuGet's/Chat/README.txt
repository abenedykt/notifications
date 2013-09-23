Aby uruchomi� fukcj� chatu, wykonaj nast�puj�ce czynno�ci:

-> dodaj nuget 'Chat' do projektu

-> dodaj referencje do skryptow:

<script src="~/Scripts/jquery-2.0.3.js" type="text/javascript"> </script>
<script src="~/Scripts/jquery-ui-1.10.3.js"> </script>
<script src="~/Scripts/jquery.signalR-1.1.3.js"> </script>
<script src="~/signalr/hubs"> </script>
<script src="~/Scripts/chat.js" type="text/javascript"> </script>

-> dodaj referencje do styli

   <link rel="stylesheet" type="text/css" href="~/Content/chat.css" />
   <link rel="stylesheet" type="text/css" href="~/Content/draggableWindow.css" />
   <link rel="stylesheet" type="text/css" href="~/Content/themes/base/jquery.ui.all.css" />

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

-> na stronie, na kt�rej chcesz uruchomi� chat dodaj texboxy :
 	* <input id="chatName" type="text" > - dla nazwy u�ytkownika
	* <input id="chatId" type="text"/> -dla id u�ytkownika (id po kt�rym b�dzie on wyszukiwany w bazie)
	* <button type="button" onclick="addToChat()"> Zaloguj </button> - przycisk dodaj�cy u�ytkownika do chatu

Uwaga:
Nazw� u�ytkownika oraz jego numer id mo�esz r�wnie� przekazywa� w inny spos�b. Wystarczy zmodyfikowa� pocz�tkowy fragment skryptu 'chat.js', przypisuj�c ziennym inne warto�ci:

	var userName = $('#chatName').val();
    	var userId = $('#chatId').val();

-> dodaj na stronie divy

	* <div id="divDraggable"> - w tym divie pokaze sie okienko prywatnej wiadomosci (najlepiej div na ca�� stron�)
	* <div id="chat"> </div> - w tym divie pojawi sie chat
