using System;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;

using Notifications.DataAccessLayer;
[assembly: WebActivator.PreApplicationStartMethod(
    typeof(MvcApplication2.App_Start.MySuperPackage), "PreStart")]

namespace MvcApplication2.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
                    
            var builder = new ContainerBuilder();

            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            builder.RegisterType<RavenRepository>().As<IDataRepository>();
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<Factory>().As<IFactory>();
            
            IContainer container = builder.Build();
            var resolver = new AutofacDependencyResolver(container);
            GlobalHost.DependencyResolver = resolver;
          
    	    RouteTable.Routes.MapHubs();
        }
    }
}