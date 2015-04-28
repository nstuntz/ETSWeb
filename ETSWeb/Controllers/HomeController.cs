using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETSWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            if (System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.AGENCY] != null)
            {
                ViewBag.AgencyName = ((ETSData.Agency)System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.AGENCY]).Name;
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
