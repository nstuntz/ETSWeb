using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;

namespace ETSWeb.Controllers
{
    public class ClientsController : ApiController
    {
        // GET: api/Clients
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Clients/5
        public string Get(int id)
        {
            return "This is a Test.  Only a Test.  If it were a real thing something would happen.";
        }



        // GET: api/Clients/Hours/5/11-18-2014
        [HttpGet]
        [ActionName("Hours")]
        public HttpResponseMessage Hours(string namekey, string lineDate, string starttime, string endtime)
        {
            string retVal = "false";
            DateTime testDate = DateTime.Parse(lineDate);

            //Check total hours for this client
            double oldHours = ETSData.Helpers.GetHoursThisWeekForClient(ConfigurationManager.ConnectionStrings["ETSConnection"].ConnectionString, namekey, testDate);
            double newHours = (TimeSpan.Parse(endtime.Replace("!", ":")) - TimeSpan.Parse(starttime.Replace("!", ":"))).TotalHours;

            if (oldHours + newHours > int.Parse(ConfigurationManager.AppSettings["HoursWarning"]))
            {
                retVal = "true";
            }

            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(retVal, Encoding.UTF8, "text/plain");

            return resp;
        }


        // GET: api/Clients/Hours/5/11-18-2014
        [HttpGet]
        [ActionName("Hours")]
        public HttpResponseMessage Hours(string namekey, string lineDate, double hours)
        {
            string retVal = "false";
            DateTime testDate = DateTime.Parse(lineDate);

            //Check total hours for this client
            double oldHours = ETSData.Helpers.GetHoursThisWeekForClient(ConfigurationManager.ConnectionStrings["ETSConnection"].ConnectionString, namekey, testDate);
            double newHours = hours;
            //double.TryParse(hours ,out newHours);

            if (oldHours + newHours > int.Parse(ConfigurationManager.AppSettings["HoursWarning"]))
            {
                retVal = "true";
            }

            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(retVal, Encoding.UTF8, "text/plain");

            return resp;
        }

        // POST: api/Clients
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Clients/5
        public void Put(int id, [FromBody]string value)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }

        // DELETE: api/Clients/5
        public void Delete(int id)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }
    }
}
