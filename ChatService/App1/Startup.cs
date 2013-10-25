using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(App1.Startup))]

namespace App1
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            app.MapHubs();

        }
    }
}
