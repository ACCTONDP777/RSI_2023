using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Configuration;

namespace RSI.Models.Utility
{
    public class SqlDBAdapter
    {
        public static DataTable GetDataTable(string constr, string sql)
        {
            using (SqlConnection myconn = new SqlConnection(constr))
            {
                try
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter myda = new SqlDataAdapter(sql, myconn);
                    myda.Fill(dt);

                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static bool ExecuteNonQuery(string constr, string sql)
        {
            using (SqlConnection myconn = new SqlConnection(constr))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, myconn))
                    {
                        int i = command.ExecuteNonQuery();
                        bool result = i > 0 ? true : false;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}

