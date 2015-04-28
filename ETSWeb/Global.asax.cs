using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;
using System.Configuration;
using System.Web.Security;

namespace ETSWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Session_Start()
        {
            if (User.Identity.IsAuthenticated)
            {
                //Add the user's agency to the session context so we can use it later
                ETSData.User user = ETSData.User.Load(ConfigurationManager.ConnectionStrings["ETSConnection"].ConnectionString, User.Identity.Name);
                System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.USER] = user;
                System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.AGENCY] = ETSData.Agency.Load(ConfigurationManager.ConnectionStrings["ETSConnection"].ConnectionString, user.AgencyID);
            }
            else
            {
                //Default to 1
                System.Web.HttpContext.Current.Session[ETSData.Constants.HTTPSessionNames.AGENCY] = ETSData.Agency.Load(ConfigurationManager.ConnectionStrings["ETSConnection"].ConnectionString, 1);
            }
        }

        void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                //Get the reset Days from the config
                string resetDays = ConfigurationManager.AppSettings["PasswordResetDays"];
                int days = 90;
                if (!Int32.TryParse(resetDays, out days))
                {
                    days = 90;
                }
                if (days < 1)
                {
                    days = 90;
                }

                try
                {
                    // has their password expired?
                    if (WebSecurity.GetPasswordChangedDate(User.Identity.Name).AddDays(days) < DateTime.Now.Date
                        && !Request.Path.EndsWith("/Account/Manage"))
                    {
                        Response.RedirectToRoute(new RouteValueDictionary {
                                    { "Controller", "Account" },
                                    { "Action", "Manage" }});
                    }
                }
                catch
                {                    
                        Response.RedirectToRoute(new RouteValueDictionary {
                                    { "Controller", "Account" },
                                    { "Action", "LogOff" }});
                }
            }
        }

    }
}