using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR.Client;

[assembly: OwinStartup(typeof(App2.Startup))]

namespace App2
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalVar.connection = new HubConnection("http://localhost:61122/signalr", useDefaultUrl: false);
            GlobalVar.chat = GlobalVar.connection.CreateHubProxy("ChatServiceHub");
            GlobalVar.connection.Start().Wait();
            app.MapHubs();
        }
    }
}
