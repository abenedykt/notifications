using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MvcApp1.Startup))]

namespace MvcApp1
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapHubs();
        }
    }
}
