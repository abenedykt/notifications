using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;

namespace App1
{
   public class MvcApplication : HttpApplication
    {
        

        protected void Application_Start()
        {
            GlobalVar.connection = new HubConnection("http://localhost:61122//signalr", useDefaultUrl: false);
            GlobalVar.chat = GlobalVar.connection.CreateHubProxy("ChatServiceHub");
            GlobalVar.connection.Start().Wait();

  
            AreaRegistration.RegisterAllAreas();
            
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
       

    }

       
    }