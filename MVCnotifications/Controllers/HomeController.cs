using System.Web.Mvc;
//using BusinessLogic;

namespace MVCnotifications.Controllers
{
    public class HomeController : Controller
    {
        //private Factory _factory;

        //public HomeController()
        //{
        //    _factory = new Factory();
        //}

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
