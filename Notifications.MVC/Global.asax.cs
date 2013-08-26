using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
//using Autofac.Integration.Mvc;
//using Autofac.Integration.WebApi;
using Autofac.Integration.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer;
using Notifications.Mvc.App_Start;
using System.Reflection;
using Microsoft.AspNet.SignalR;

namespace Notifications.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
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
           
            builder.RegisterType<SqlRepository>().As<IDataRepository>();
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<Factory>().As<IFactory>();
            //var container = builder.Build();
            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var container = builder.Build();
            var resolver = new AutofacDependencyResolver(container);
            GlobalHost.DependencyResolver = resolver;


        }
    }

}