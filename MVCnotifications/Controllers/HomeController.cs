using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCnotifications.Controllers
{
    public class HomeController : Controller
    {
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
