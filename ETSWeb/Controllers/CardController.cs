using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ETSWeb.Models;
using System.Configuration;
using System.Web.Routing;
using WebMatrix.WebData;

namespace ETSWeb.Controllers
{
    public class CardController : Controller
    {
        #region OLD
        //
        // GET: /Card/
        //public ActionResult Index()
        //{
        //    // Get the last day of the current week
        //    CardModel model = SetupModel();
        //    model.EditLine = new TimeLineModel();

        //    return View(model);
        //}


        //[HttpPost]
        //public ActionResult Index(CardModel model)
        //{
        //    using (TimeLineDB db = new TimeLineDB())
        //    {
        //        model.EditLine.UserID = User.Identity.Name;                
        //        db.TimeLines.Add(model.EditLine);
        //        db.SaveChanges();
        //    }

        //    model = SetupModel();
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Edit(CardModel model)
        //{
        //    using (TimeLineDB db = new TimeLineDB())
        //    {
        //        model.EditLine.UserID = User.Identity.Name;
        //        TimeLineModel temp = db.TimeLines.FirstOrDefault(x => x.Id == model.EditLine.Id);

        //        temp.ActivityCode = model.EditLine.ActivityCode;
        //        temp.ClientID = model.EditLine.ClientID;
        //        temp.LineDate = model.EditLine.LineDate;
        //        temp.Pieces = model.EditLine.Pieces;
        //        temp.StartTime = model.EditLine.StartTime;
        //        temp.EndTime = model.EditLine.EndTime;
        //        temp.UserID = model.EditLine.UserID;

        //        db.Entry(temp).State = System.Data.EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    model = SetupModel();

        //    return View("Index", model);
        //}
        #endregion OLD

        int _EndDay = ((ETSData.Agency)System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.AGENCY]).WeekEndDay;


        //
        [HttpPost]
        [Authorize]
        public ActionResult Edit(LineModel line)
        {
            return ClientEntry(line,"ClientEntry");
        }

        //
        // GET: /Card/Edit/5
        [Authorize]
        public ActionResult Edit(int id, string clientID, DateTime week)
        {

            LineModel model = SetupLineModel(clientID, week.Date);
            using (TimeLineDB db = new TimeLineDB())
            {
                model.EditLine = db.TimeLines.FirstOrDefault(x => x.Id == id);
            }

            GetWeekHourSummary(model, clientID);
            
            return View("ClientEntry", model);
        }
        
        //
        // GET: /Card/Delete/5
        [Authorize]
        public ActionResult Delete(int id, string clientID, DateTime week)
        {
            using (TimeLineDB db = new TimeLineDB())
            {
                db.TimeLines.Remove(db.TimeLines.FirstOrDefault(x => x.Id == id));
                db.SaveChanges();
            }

            //LineModel model = SetupLineModel(clientID, week.Date);
            //GetWeekHourSummary(model, clientID);

            return RedirectToAction("ClientEntry", new RouteValueDictionary { { "clientID", clientID }, { "day", week.ToShortDateString() } });

            //return View("ClientEntry", model);
        }

        [Authorize]
        public ActionResult RemoveClient(string clientID, DateTime day)
        {
            string userID = User.Identity.Name;
            DateTime endDay = day.AddDays(7);
            using (TimeLineDB db = new TimeLineDB())
            {
                foreach (TimeLineModel line in
                        db.TimeLines.Where(x => (x.LineDate >= day && x.LineDate <= endDay && x.UserID == userID
                                                 && x.ClientID == clientID)))
                {
                    db.TimeLines.Remove(line);
                }

                db.SaveChanges();
            }

            return WeekEntry(day);
        }

        [Authorize]
        public ActionResult DuplicateClientWeek(string clientID, DateTime day)
        {
            Duplicate(day,clientID);
            return WeekEntry(day);
        }

        [Authorize]
        public ActionResult Weeks()
        {
            ViewBag.AllowDuplicates = ConfigurationManager.AppSettings["AllowDuplicate"] == "1";
            WeeksModel model = new WeeksModel();

            //Get the previous 5 weeks worth
            for (int i = 0; i < 5; i++ )
            {
                Tuple<DateTime, DateTime> dates = Utilities.GetWeekStartEndDates(_EndDay, DateTime.Today.AddDays(i*-7));

                using (TimeLineDB db = new TimeLineDB())
                {
                    List<TimeLineModel> lines = db.TimeLines.Where(x => x.UserID == User.Identity.Name)
                        .Where(x => x.LineDate >= dates.Item1)
                        .Where(x => x.LineDate <= dates.Item2)
                        .ToList();

                    WeekModel week = new WeekModel
                    {
                        WeekStart = dates.Item1,
                        WeekEnd = dates.Item2,
                        TotalHours = lines.Sum(x => (x.Hours)),
                        TotalClients = lines.Select(x => x.ClientID).Distinct().Count()
                    };

                    model.Weeks.Add(week);
                }
            }

            return View("Weeks", model);
        }

        [Authorize]
        public ActionResult WeekEntry(DateTime weekStart)
        {
            OneWeekModel model = new OneWeekModel();
            ViewBag.AllowDuplicates = ConfigurationManager.AppSettings["AllowDuplicate"] == "1";

            //Add the start/End days
            Tuple<DateTime, DateTime> dates = Utilities.GetWeekStartEndDates(_EndDay, weekStart);
            model.WeekStart = dates.Item1;
            model.WeekEnd = dates.Item2;

            using (TimeLineDB db = new TimeLineDB())
            {
                List<TimeLineModel> lines = db.TimeLines.Where(x => x.UserID == User.Identity.Name)
                    .Where(x => x.LineDate >= dates.Item1)
                    .Where(x => x.LineDate <= dates.Item2)
                    .ToList();

                model.IsApproved = lines.Count(x => x.DateApproved == null) == 0 && lines.Count > 0;
                model.IsSubmitted = lines.Count(x => x.DateSubmitted == null) == 0 && lines.Count > 0;

                IEnumerable<string> clients = lines.Select(x => x.ClientID).Distinct();

                foreach (string clientID in clients)
                {
                    ClientWeek week = new ClientWeek();
                    var temp = db.Clients.Where(x => x.Name_Key == clientID).FirstOrDefault();
                    week.Client = temp;
                    week.Hours = new Dictionary<int, decimal>();

                    for (int i = 0; i < 7; i++)
                    {
                        decimal dayHours = lines.Where(x => x.ClientID == clientID).Where(x => x.LineDate == weekStart.AddDays(i)).Sum(x => (x.Hours));
                        week.Hours.Add(i, dayHours);
                    }
                    
                    model.Clients.Add(week);
                }
            }

            model.Clients = model.Clients.OrderBy(x => x.Client.sort_name).ToList();

            IEnumerable<SelectListItem> items =
                from client in GetClients()
                where !(model.Clients.Any(x => x.Client.Name_Key == client.Value))
                select new SelectListItem
                {
                    Text = client.Text,
                    Value = client.Value
                };

            model.NewClients = items;

            return View("WeekEntry", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult WeekEntry(OneWeekModel week)
        {
            ViewBag.AllowDuplicates = ConfigurationManager.AppSettings["AllowDuplicate"] == "1";
            //Redirect to the Get action for the client entry for the new client
            return RedirectToAction("ClientEntry", new { clientID = week.NewClientNameKey, day = week.WeekStart });
        }


        [HttpPost]
        [Authorize]
        public ActionResult ClientEntry(LineModel model, string viewName = null)
        {

            if (System.Configuration.ConfigurationManager.AppSettings["UseHours"] != "1")
            {
                if (model.EditLine.EndTime <= model.EditLine.StartTime)
                {
                    ModelState.AddModelError("EditLine.EndTime", "End Time must be after start time");
                }
            }
            //Check Pieces
            if (ETSData.Helpers.CheckIfPiecesNeeded(ConfigurationManager.ConnectionStrings["ETSConnection"].ConnectionString,model.EditLine.ActivityCode) && model.EditLine.Pieces <= 0)
            {
                ModelState.AddModelError("EditLine.Pieces", "This Activity Code requires Pieces");
            }


            //Check Date
            if (!(model.EditLine.LineDate.Date >= model.WeekStart.Date && model.EditLine.LineDate.Date <= model.WeekStart.Date.AddDays(7)))
            {
                ModelState.AddModelError("EditLine.LineDate", "This Date must be within the week");
            }

            //Check total hours for this client
            double oldHours = ETSData.Helpers.GetHoursThisWeekForClient
                (ConfigurationManager.ConnectionStrings["ETSConnection"].ConnectionString,
                model.Client.Name_Key, model.EditLine.LineDate);
            double newHours = (model.EditLine.EndTime - model.EditLine.StartTime).TotalHours;

            if (oldHours + newHours > int.Parse(ConfigurationManager.AppSettings["HoursError"]))
            {
                ModelState.AddModelError("EditLine.EndTime", "Total hours can not exceed " + ConfigurationManager.AppSettings["HoursError"] + " hours for the day.");
            }

            if (ModelState.IsValid)
            {
                //Save the edited line
                using (TimeLineDB db = new TimeLineDB())
                {
                    System.Data.EntityState state = System.Data.EntityState.Modified;

                    model.EditLine.UserID = User.Identity.Name;
                    TimeLineModel temp = db.TimeLines.FirstOrDefault(x => x.Id == model.EditLine.Id);

                    if (temp == null)
                    {
                        temp = new TimeLineModel();
                        state = System.Data.EntityState.Added;
                    }

                    temp.ActivityCode = model.EditLine.ActivityCode;
                    temp.ClientID = model.Client.Name_Key;
                    temp.LineDate = model.EditLine.LineDate;
                    temp.Pieces = model.EditLine.Pieces;
                    temp.StartTime = model.EditLine.StartTime;
                    temp.EndTime = model.EditLine.EndTime;

                    if (System.Configuration.ConfigurationManager.AppSettings["UseHours"] == "1")
                    {
                        temp.Hours = model.EditLine.Hours;
                    }
                    else
                    {
                        temp.Hours = (decimal)(model.EditLine.EndTime - model.EditLine.StartTime).TotalHours;
                    }
                    temp.UserID = model.EditLine.UserID;
                    temp.Comments = model.EditLine.Comments;

                    db.Entry(temp).State = state;
                    db.SaveChanges();
                }
            }

            model = SetupLineModel(model.Client.Name_Key, model.WeekStart.Date);

            GetWeekHourSummary(model, model.Client.Name_Key);
            return viewName == null ? View(model) : View(viewName, model);
        }

        [Authorize]
        public ActionResult ClientEntry(string clientID, DateTime day)
        {
            // Get the last day of the current week
            LineModel model = SetupLineModel(clientID, day.Date);

            GetWeekHourSummary(model, clientID);

            return View(model);
        }


        // GET: /Card/PreviousWeek
        [Authorize]
        public ActionResult PreviousWeek(DateTime weekStart)
        {
            // Get the last day of the current week
            CardModel model = SetupModel(weekStart.AddDays(-7));

            return View("Index", model);
        }

        // GET: /Card/NextWeek
        [Authorize]
        public ActionResult NextWeek(DateTime weekStart)
        {
            // Get the last day of the current week
            CardModel model = SetupModel(weekStart.AddDays(7));

            return View("Index", model);
        }

        [Authorize]
        public ActionResult WeekDuplicate(DateTime weekStart)
        {
            Duplicate(weekStart);

            return Weeks();
        }



        [Authorize]
        public ActionResult SubmitTime(DateTime weekStart)
        {
            Tuple<DateTime, DateTime> dates = Utilities.GetWeekStartEndDates(_EndDay, weekStart);

            using (TimeLineDB db = new TimeLineDB())
            {
                List<TimeLineModel> lines = db.TimeLines.Where(x => x.UserID == User.Identity.Name)
                    .Where(x => x.LineDate >= dates.Item1)
                    .Where(x => x.LineDate <= dates.Item2)
                    .ToList();

                foreach (TimeLineModel line in lines)
                {
                    line.DateSubmitted = DateTime.Now;                    
                }
                db.SaveChanges();
            }

           // LineModel model = SetupLineModel(clientID, weekStart);
            //GetWeekHourSummary(model, model.Client.Name_Key);
            //return View("ClientEntry", model);
            return RedirectToAction("WeekEntry", new { weekStart = weekStart });

        }


        private void Duplicate(DateTime weekStart, string clientID = null)
        {
            //Add the start/End days
            Tuple<DateTime, DateTime> dates = Utilities.GetWeekStartEndDates(_EndDay, weekStart);
            DateTime choosenWeekStart = dates.Item1;
            DateTime choosenWeekEnd = dates.Item2;

            Tuple<DateTime, DateTime> currentDate = Utilities.GetWeekStartEndDates(_EndDay);
            DateTime thisWeekStart = currentDate.Item1;

            int weeksDifference = (thisWeekStart - weekStart).Days;

            
            string userID = User.Identity.Name;
            using (TimeLineDB db = new TimeLineDB())
            {
                foreach (TimeLineModel line in db.TimeLines.Where(x => (x.LineDate >= choosenWeekStart && x.LineDate <= choosenWeekEnd && x.UserID == userID)))
                {
                    if (clientID == null || clientID != line.ClientID)
                    {
                        continue;                        
                    }

                    TimeLineModel temp = new TimeLineModel();

                    temp.ActivityCode = line.ActivityCode;
                    temp.ClientID = line.ClientID;

                    temp.LineDate = line.LineDate.AddDays(weeksDifference);

                    temp.Pieces = line.Pieces;
                    temp.StartTime = line.StartTime;
                    temp.EndTime = line.EndTime;
                    temp.Hours = line.Hours;
                    temp.UserID = line.UserID;
                    temp.Comments = line.Comments;

                    db.Entry(temp).State = System.Data.EntityState.Added;
                }

                db.SaveChanges();
            }
        }

        private CardModel SetupModel( DateTime? day = null)
        {
            CardModel model = new CardModel();
            model.Clients = GetClients();
            model.ActivityCodes = GetActivities();

            //Add the start/End days
            Tuple<DateTime, DateTime> dates = Utilities.GetWeekStartEndDates(_EndDay, day);
            model.WeekStart = dates.Item1;
            model.WeekEnd = dates.Item2;

            using (TimeLineDB db = new TimeLineDB())
            {
                model.Lines = db.TimeLines.Where(x => x.UserID == User.Identity.Name)
                    .Where(x => x.LineDate >= dates.Item1)
                    .Where(x => x.LineDate <= dates.Item2)
                    .ToList();
            }

            model.EditLine = new TimeLineModel();
            if (ModelState.IsValid)
            {
                ModelState.Clear();
            }
            return model;
        }

        private LineModel SetupLineModel(string clientID, DateTime? day = null)
        {
            LineModel model = new LineModel();
            model.ActivityCodes = GetActivities();

            //Add the start/End days
            Tuple<DateTime, DateTime> dates = Utilities.GetWeekStartEndDates(_EndDay, day);
            model.WeekStart = dates.Item1;
            model.WeekEnd = dates.Item2;

            using (TimeLineDB db = new TimeLineDB())
            {
                model.Lines = db.TimeLines.Where(x => x.UserID == User.Identity.Name)
                    .Where(x => x.LineDate >= dates.Item1)
                    .Where(x => x.LineDate <= dates.Item2)
                    .Where(x => x.ClientID == clientID)
                    .OrderBy(x => x.LineDate)
                    .ThenBy(x => x.StartTime)
                    .ToList();

                model.Client = db.Clients.FirstOrDefault(x => x.Name_Key == clientID);
            }

            model.EditLine = new TimeLineModel(model.WeekStart.AddDays((int)DateTime.Today.DayOfWeek));
            model.EditLine.ClientID = clientID;
            if (ModelState.IsValid)
            {
                ModelState.Clear();
            }
            return model;
        }

        
        private IEnumerable<SelectListItem> GetClients()
        {

            ETSData.User user = ((ETSData.User)System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.USER]);
            
            using (TimeLineDB db = new TimeLineDB())
            {
                if (ConfigurationManager.AppSettings["AllowAllClients"] == "1")
                {
                    IEnumerable<SelectListItem> items =
                     from client in db.Clients
                     orderby client.sort_name
                     select new SelectListItem
                     {
                         Text = client.sort_name,
                         Value = client.Name_Key
                     };
                    return items.ToList();
                }
                else
                {
                    IEnumerable<SelectListItem> items =
                     from client in db.Clients
                     join rel in db.ConsumerCoachs on client.Name_Key equals rel.ConsumerKey
                     //where !(client.first_name == null || client.first_name.Trim() == string.Empty)
                     where rel.CoachKey == user.NameKey
                     orderby client.sort_name
                     select new SelectListItem
                     {
                         Text = client.sort_name,
                         Value = client.Name_Key
                     };
                    return items.ToList();
                }
            }
        }

        
        private IEnumerable<SelectListItem> GetActivities()
        {
            using (TimeLineDB db = new TimeLineDB())
            {
                IEnumerable<SelectListItem> items =
                 from activity in db.Activities
                 //where activity.name_key == "1000"
                 orderby activity.job_num
                 select new SelectListItem
                 {
                     Text = activity.job_num + " - " + activity.job_desc.Substring(0,20),
                     Value = activity.job_num
                 };
                return items.ToList();
            }
        }

        private void GetWeekHourSummary(LineModel model, string clientID)
        {
            model.Hours = new Dictionary<int, decimal>();

            for (int i = 0; i < 7; i++)
            {
                if (model.Lines == null) continue;
                decimal dayHours =
                    model.Lines.Where(x => x.ClientID == clientID)
                        .Where(x => x.LineDate.Date == model.WeekStart.AddDays(i).Date)
                        .Sum(x => (x.Hours));
                model.Hours.Add(i, dayHours);
            }
        }



        // GET: /Card/TimeLineView
        [Authorize]
        public ActionResult TimeLineView()
        {
            // Get the last day of the current week
            LineModel model = new LineModel();

            using (TimeLineDB db = new TimeLineDB())
            {
                model.EditLine = db.TimeLines.Where(x => x.UserID == User.Identity.Name).FirstOrDefault();
            }

            return View(model);
        }
    }
}
