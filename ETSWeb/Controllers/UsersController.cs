using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebMatrix.WebData;
using ETSWeb.Filters;
using ETSWeb.Models;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Security;

namespace ETSWeb.Controllers
{
    public class UsersController : ApiController
    {
        [HttpGet]
        [ActionName("Create")]
        // GET: api/Users/Create/test1/test2/thing@stong.com/1233/asdf
        public string CreateUser(string userName, string password, string email, string nameKey, string random)
        {
            bool existingUser = false;
            using (UsersContext ctx = new UsersContext())
            {
                existingUser = ctx.UserProfiles.Count(x => x.Email == email || x.UserName == userName || x.name_key == nameKey) > 0;
            }
            
            bool coachExists = ETSData.Helpers.CoachExists(ConfigurationManager.ConnectionStrings["ETSConnection"].ConnectionString,nameKey);
            
            if (!existingUser && coachExists)
            {
                InitializeWebSecurityDB();

                WebSecurity.CreateUserAndAccount(userName, password,
                    new { Email = email, name_key = nameKey });

                return "true";
            }
            else
            {
                return "false";
            }
        }


        [HttpGet]
        [ActionName("Role")]
        // GET: api/Users/Role/1233/Supervisor
        public string UserRole(string nameKey, string RoleName)
        {
            UserProfile user = null;
            using (UsersContext ctx = new UsersContext())
            {
                user = ctx.UserProfiles.First(x => x.name_key == nameKey);
            }

            if (user == null || user.name_key != nameKey)
            {
                return "false";
            }

            InitializeWebSecurityDB();

            if (!Roles.RoleExists(RoleName))
            {
                return "false";
            }

            if (Roles.IsUserInRole(user.UserName, RoleName))
            {
                return "true";
            }

            Roles.AddUserToRole(user.UserName, RoleName);

            return "true";
        }

        private void InitializeWebSecurityDB()
        {


            if (!WebSecurity.Initialized)
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("ETSConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }

        }
    }
}
