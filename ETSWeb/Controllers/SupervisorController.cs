using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETSWeb.Models;

namespace ETSWeb.Controllers
{
    public class SupervisorController : Controller
    {
        private TimeLineDB db = new TimeLineDB();
        //
        // GET: /Supervisor/
        [Authorize(Roles="Supervisor")]
        public ActionResult Index()
        {
            string userNameKey = Utilities.GetCurrentUser().NameKey;
            List<string> supervisedCoaches = db.SupervisorCoachs.Where(x => x.SupervisorKey == userNameKey).Select(x => x.CoachKey).ToList();
            IEnumerable<EmployeeModel> coaches = db.Employees.Where(x => supervisedCoaches.Any(c => c == x.name_key)).ToList();
            return View(coaches);
        }
	}
}