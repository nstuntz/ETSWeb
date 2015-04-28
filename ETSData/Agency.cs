using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ETSData
{
    public class Agency
    {
        private const string AGENCY_WEEK_ENDDAY = "EndWeekDay";
        private const string TABLE_NAME = "ccsys";

        public int WeekEndDay { get; set; }
        public string Name { get; set; }

        public static Agency Load(string connectionString, int agencyID)
        {
            Agency newAgency = new Agency();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "select " + AGENCY_WEEK_ENDDAY + ",agency_name" +
                                " from " + TABLE_NAME +
                                " where agency = " + agencyID;

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            newAgency.WeekEndDay = reader.GetInt32(reader.GetOrdinal(AGENCY_WEEK_ENDDAY));
                            newAgency.Name = reader.GetString(reader.GetOrdinal("agency_name"));
                        }
                    }
                }
            }
            return newAgency;
        }
    }
}
