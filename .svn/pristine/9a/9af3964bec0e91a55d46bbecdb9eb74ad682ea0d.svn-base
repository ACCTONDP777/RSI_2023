using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Common;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;


namespace RSI.Models.Utility
{
    public class OraDBAdapter
    {
        /// <summary>
        /// 组合sql字串查询数据
        /// </summary>
        /// <param name="constr"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string constr, string sql)
        {
            using (OracleConnection myconn = new OracleConnection(constr))
            {
                try
                {
                    DataTable dt = new DataTable();
                    OracleDataAdapter myda = new OracleDataAdapter(sql, myconn);
                    myda.Fill(dt);

                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 透过参数Parameter传变量查询数据
        /// </summary>
        /// <param name="constr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string constr, string sql, OracleParameter[] param)
        {
            using (OracleConnection myconn = new OracleConnection(constr))
            {
                try
                {
                    using (OracleCommand command = new OracleCommand(sql, myconn))
                    {
                        command.BindByName = true;
                        if (param != null && param.Length > 0)
                            command.Parameters.AddRange(param);
                       DataTable dt = new DataTable();
                        OracleDataAdapter myda = new OracleDataAdapter(command);
                        myda.Fill(dt);

                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static bool ExecuteNonQuery(string constr, string sql)
        {
            using (OracleConnection myconn = new OracleConnection(constr))
            {
                try
                {
                    using (OracleCommand command = new OracleCommand(sql, myconn))
                    {
                        myconn.Open();
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

        public static bool ExecuteNonQuery(string constr, string sql, OracleParameter[] param)
        {
            using (OracleConnection myconn = new OracleConnection(constr))
            {
                try
                {
                    using (OracleCommand command = new OracleCommand(sql, myconn))
                    {
                        command.BindByName = true;
                        if (param != null && param.Length > 0)
                            command.Parameters.AddRange(param);

                        myconn.Open();
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

        public static bool ExecuteStoredProcedure(string constr, string storedProcedureName)
        {
            using (OracleConnection myconn = new OracleConnection(constr))
            {
                try
                {
                    using (OracleCommand command = new OracleCommand())
                    {
                        command.Connection = myconn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = storedProcedureName;
                        myconn.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static bool ExecuteStoredProcedure(string constr, string storedProcedureName, OracleParameter[] param)
        {
            using (OracleConnection myconn = new OracleConnection(constr))
            {
                try
                {
                    using (OracleCommand command = new OracleCommand())
                    {
                        command.Connection = myconn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = storedProcedureName;
                        command.BindByName = true;
                        if (param != null && param.Length > 0)
                            command.Parameters.AddRange(param);

                        myconn.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static DataTable ExecuteStoredProcedureGetDataTable(string constr, string storedProcedureName, OracleParameter[] param)
        {
            using (OracleConnection myconn = new OracleConnection(constr))
            {
                try
                {
                    using (OracleCommand command = new OracleCommand())
                    {
                        command.Connection = myconn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = storedProcedureName;
                        command.BindByName = true;
                        if (param != null && param.Length > 0)
                            command.Parameters.AddRange(param);

                        myconn.Open();
                        command.ExecuteNonQuery();
                        OracleDataAdapter da = new OracleDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
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