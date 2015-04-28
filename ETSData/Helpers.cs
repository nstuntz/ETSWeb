using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETSData
{
    public static class Helpers
    {
        public static bool CheckIfPiecesNeeded(string connectionString, string activityCode)
        {
            string payclass = "";
           
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "select pay_class " +
                                " from ccjob " +
                                " where job_num = '" + activityCode + "'";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            payclass = reader.GetString(reader.GetOrdinal("pay_class"));                           
                        }
                    }
                }
            }
            return payclass == "PC" || payclass == "OP";
        }

        public static double GetHoursThisWeekForClient(string connectionString, string clientID, DateTime lineDate)
        {
            object temp = null; 
           
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "select Sum(hours) as TotHours " +
                                " from timeline " +
                                " where clientid = '" + clientID + "' AND " +
                                " lineDate = '" + lineDate.ToShortDateString() + "'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            temp = reader[reader.GetOrdinal("TotHours")];  
                        }
                    }
                }
            }
            if (temp == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(temp);
            }
        }        

        public static bool CoachExists(string connectionString, string nameKey)
        {
            bool retVal = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "select sort_name, name_key from nam_ee WHERE name_key ='" + nameKey + "'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = true;
                        }
                    }
                }
            }
            return retVal;
        }
    }
}
