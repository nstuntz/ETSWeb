using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ETSData;

namespace ETSWeb
{
    public static class Utilities
    {
        public static Tuple<DateTime,DateTime> GetWeekStartEndDates(int endDay, DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Today;
            }
            
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;

            if (((int)date.Value.DayOfWeek) == endDay)
            {
                end = date.Value.AddDays(1).AddMilliseconds(-1);
                start = end.AddDays(-6);
            }
            else if ((int)date.Value.DayOfWeek > endDay)
            {
                start = date.Value.AddDays(-1 * ((int)date.Value.DayOfWeek - endDay));
                end = start.AddDays(7).AddMilliseconds(-1);
            }
            else
            {
                end = date.Value.AddDays(endDay - (int)date.Value.DayOfWeek + 1).AddMilliseconds(-1);
                start = end.AddDays(-6);
            }

            return new Tuple<DateTime, DateTime>(start, end);
        }
    
        public static ETSData.User GetCurrentUser()
        {
            return ((ETSData.User)System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.USER]);
        }
    }

}