using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ETSData
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Role { get; set; }
        public int AgencyID { get; set; }
        public string NameKey { get; set; }


        public static User Load(string connectionString, string username)
        {
            User newUser = new User();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "select nam_addr.first_name, nam_addr.last_name, UserProfile.email, nam_addr.agency,UserProfile.name_key " +
                                " from nam_addr INNER JOIN UserProfile ON nam_addr.name_key = UserProfile.name_key " +
                                " where UserProfile.username = '" + username + "'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            newUser.FirstName = reader.GetString(reader.GetOrdinal("first_name"));
                            newUser.LastName = reader.GetString(reader.GetOrdinal("last_name"));
                            newUser.AgencyID = reader.GetInt32(reader.GetOrdinal("agency"));
                            newUser.NameKey = reader.GetString(reader.GetOrdinal("name_key"));
                        }
                    }
                }
            }
            return newUser;
        }


        public static Dictionary<string,string> GetAllUsers(string connectionString)
        {
            Dictionary<string, string> users = new Dictionary<string, string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "select sort_name, name_key from nam_ee WHERE name_key not in (Select name_key from UserProfile)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(reader.GetString(reader.GetOrdinal("name_key")), reader.GetString(reader.GetOrdinal("sort_name")));
                        }
                    }
                }
            }
            return users;
        }
    }
}
