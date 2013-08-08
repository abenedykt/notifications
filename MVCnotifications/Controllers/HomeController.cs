using System.Web.Mvc;
using BusinessLogic;

namespace MVCnotifications.Controllers
{
    public class HomeController : Controller
    {
        private Factory _factory;

        public HomeController()
        {
            _factory = new Factory();
        }

        public ActionResult Index()
        {
            return View();
        }

          public ActionResult Default()
        {
            return PartialView();
        }

        public ActionResult Page1()
        {
            return PartialView();
        }

        public ActionResult Page2()
        {
            return PartialView();
        }
    }
    
}
