using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETSWeb.Models;

namespace ETSWeb.Controllers
{
    public class CardController : Controller
    {
        //
        // GET: /Card/
        public ActionResult Index()
        {
            // Get the last day of the current week
            CardModel model = SetupModel();
            model.EditLine = new TimeLineModel();

            double time = model.Lines.Sum(x => (x.EndTime - x.StartTime).Minutes) / 60.0;

            return View(model);
        }


        [HttpPost]
        public ActionResult Index(CardModel model)
        {
            using (TimeLineDB db = new TimeLineDB())
            {
                model.EditLine.UserID = User.Identity.Name;                
                db.TimeLines.Add(model.EditLine);
                db.SaveChanges();
            }

            model = SetupModel();
            return View(model);
        }

        //
        // GET: /Card/Edit/5
        public ActionResult Edit(int id)
        {
            CardModel model = SetupModel();
            using (TimeLineDB db = new TimeLineDB())
            {
                model.EditLine = db.TimeLines.FirstOrDefault(x => x.Id == id);
            }
            
            return View("Index",model);
        }

        [HttpPost]
        public ActionResult Edit(CardModel model)
        {
            using (TimeLineDB db = new TimeLineDB())
            {
                model.EditLine.UserID = User.Identity.Name;
                TimeLineModel temp = db.TimeLines.FirstOrDefault(x => x.Id == model.EditLine.Id);
                
                temp.ActivityCode = model.EditLine.ActivityCode;
                temp.ClientID = model.EditLine.ClientID;
                temp.LineDate = model.EditLine.LineDate;
                temp.Pieces = model.EditLine.Pieces;
                temp.StartTime = model.EditLine.StartTime;
                temp.EndTime = model.EditLine.EndTime;
                temp.UserID = model.EditLine.UserID;
                
                db.Entry(temp).State = System.Data.EntityState.Modified;
                db.SaveChanges();
            }

            model = SetupModel();

            return View("Index", model);
        }

        //
        // GET: /Card/Delete/5
        public ActionResult Delete(int id)
        {
            using (TimeLineDB db = new TimeLineDB())
            {
                db.TimeLines.Remove(db.TimeLines.FirstOrDefault(x => x.Id == id));
                db.SaveChanges();
            }

            CardModel model = SetupModel();

            return View("Index", model);
        }



        // GET: /Card/PreviousWeek
        public ActionResult PreviousWeek(DateTime weekStart)
        {
            // Get the last day of the current week
            CardModel model = SetupModel(weekStart.AddDays(-7));

            return View("Index", model);
        }

        // GET: /Card/NextWeek
        public ActionResult NextWeek(DateTime weekStart)
        {
            // Get the last day of the current week
            CardModel model = SetupModel(weekStart.AddDays(7));

            return View("Index", model);
        }

        private CardModel SetupModel( DateTime? day = null)
        {
            CardModel model = new CardModel();
            model.Clients = GetClients();
            model.ActivityCodes = GetActivities();

            //Add the start/End days
            int endDay = ((ETSData.Agency)System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.AGENCY]).WeekEndDay;

            Tuple<DateTime, DateTime> dates = Utilities.GetWeekStartEndDates(endDay, day);
            model.WeekStart = dates.Item1;
            model.WeekEnd = dates.Item2;

            using (TimeLineDB db = new TimeLineDB())
            {
                model.Lines = db.TimeLines.Where(x => x.UserID == User.Identity.Name)
                    .Where(x => x.LineDate >= dates.Item1)
                    .Where(x => x.LineDate <= dates.Item2)
                    .ToList<TimeLineModel>();
            }

            model.EditLine = new TimeLineModel();
            ModelState.Clear();
            return model;

        }

        private IEnumerable<SelectListItem> GetClients()
        {
            using (TimeLineDB db = new TimeLineDB())
            {
                IEnumerable<SelectListItem> items =
                 from client in db.Clients
                 //where !(client.first_name == null || client.first_name.Trim() == string.Empty)
                 orderby client.sort_name
                 select new SelectListItem
                 {
                     Text = client.sort_name,
                     Value = client.Name_Key
                 };
                return items.ToList<SelectListItem>();
            }
        }


        private IEnumerable<SelectListItem> GetActivities()
        {
            using (TimeLineDB db = new TimeLineDB())
            {
                IEnumerable<SelectListItem> items =
                 from activity in db.Activities
                 where activity.name_key == "1000"
                 orderby activity.job_num ascending
                 select new SelectListItem
                 {
                     Text = "(" + activity.job_num + ") - " + activity.job_desc,
                     Value = activity.job_num
                 };
                return items.ToList<SelectListItem>();
            }
        }
    }
}
