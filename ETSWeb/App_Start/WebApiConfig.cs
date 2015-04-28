using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ETSWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "TimeCheckApi",
                routeTemplate: "api/{controller}/{action}/{namekey}/{lineDate}/{startTime}/{endTime}"
            );

            config.Routes.MapHttpRoute(
                name: "HoursApi",
                routeTemplate: "api/{controller}/{action}/{namekey}/{lineDate}/{hours}"
            );


            config.Routes.MapHttpRoute(
                name: "CreateUserApi",
                routeTemplate: "api/{controller}/{action}/{username}/{password}/{email}/{nameKey}/{random}"
            );

            config.Routes.MapHttpRoute(
                name: "UserRoleApi",
                routeTemplate: "api/{controller}/{action}/{nameKey}/{RoleName}"
            );
            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
        }
    }
}