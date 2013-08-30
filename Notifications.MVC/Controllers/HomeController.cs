using System.Web.Mvc;

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