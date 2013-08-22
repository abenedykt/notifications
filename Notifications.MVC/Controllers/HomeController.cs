using System.Web.Mvc;
using Notifications.Base;

namespace Notifications.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApplication _application;

        public HomeController(IApplication application)
        {
            _application = application;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Example()
        {
            _application.BrodcastNotification("hello",new []{1,2,3,4,5}, 2);
            return View("Index");
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
