﻿using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.Mvc.App_Start;
using Notifications.DataAccessLayer;


namespace Notifications.Mvc
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AutofacConfiguration();

            RouteTable.Routes.MapHubs();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private static void AutofacConfiguration()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterControllers(typeof(MvcApplication).Assembly);
            //builder.RegisterApiControllers(typeof(MvcApplication).Assembly);
            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            builder.RegisterType<MongoRepository>().As<IDataRepository>();
            builder.RegisterType<ChatApplication>().As<IChatApplication>();
            builder.RegisterType<Factory>().As<IFactory>();
            //var container = builder.Build();
            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            IContainer container = builder.Build();
            var resolver = new AutofacDependencyResolver(container);
            GlobalHost.DependencyResolver = resolver;
        }
    }
}