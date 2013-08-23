using System.Web.Mvc;
using Notifications.Base;
using Notifications.Mvc.Hubs;
using Microsoft.AspNet.SignalR;

namespace Notifications.Mvc.Controllers
{
    public class HomeController : Controller
    {
      

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Logowanie()
        {
            return PartialView();
        }

        public ActionResult Powiadomienia()
        {

            return PartialView();
        }

    }
    
}
