using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebChatService.Startup))]

namespace WebChatService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapHubs();
        }
    }
}
