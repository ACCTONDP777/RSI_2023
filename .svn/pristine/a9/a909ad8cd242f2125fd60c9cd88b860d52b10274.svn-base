using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;


namespace RSI.Models.Utility
{
    public class SqlExcute
    {
        /// <summary>
        /// 获取SqlServer DB Data
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="className"></param>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        public static DataTable GetSqlDateTable(StringBuilder sqlText, string className, string classInfo)
        {
            string logInfo = "【" + className + "】:" + sqlText.ToString() + "\r\n";
            LogHelper.GetLogger("--Begin:" + classInfo + "").Info(logInfo);
            try
            {
                DataTable dtResult = SqlDBAdapter.GetDataTable(DBCS.Default.ConnectionString, sqlText.ToString());
                dtResult.TableName = className;
                LogHelper.GetLogger("--End:" + classInfo + "").Info("Completed");
                return dtResult;
            }
            catch (Exception ex)
            {
                logInfo += ex.ToString();
                LogHelper.GetLogger("" + classInfo + "").Error(logInfo);
                throw ex;
            }
        }

        /// <summary>
        /// DB数据以object list形式返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlText"></param>
        /// <param name="className"></param>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        public static IList<T> GetSqlObjList<T>(StringBuilder sqlText, string className, string classInfo) where T : class, new()
        {
            return GetSqlDateTable(sqlText, className, classInfo).ToList<T>();
        }

        /// <summary>
        /// 获取Oracle DB Data
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="className"></param>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        public static DataTable GetOraDateTable(StringBuilder sqlText, string className, string classInfo)
        {
            string logInfo = "【" + className + "】:" + sqlText.ToString() + "\r\n";
            LogHelper.GetLogger("--Begin:" + classInfo + "").Info(logInfo);
            try
            {
                DataTable dtResult = OraDBAdapter.GetDataTable(DBCS.Default.ConnectionString, sqlText.ToString());
                dtResult.TableName = className;
                LogHelper.GetLogger("--End:" + classInfo + "").Info("Completed");
                return dtResult;
            }
            catch (Exception ex)
            {
                logInfo += ex.ToString();
                LogHelper.GetLogger("" + classInfo + "").Error(logInfo);
                throw ex;
            }
        }

        public static DataTable GetOraDateTable(StringBuilder sqlText, OracleParameter[] param, string className, string classInfo)
        {
            string logInfo = "【" + className + "】:" + sqlText.ToString() + "\r\n";
            LogHelper.GetLogger("--Begin:" + classInfo + "").Info(logInfo);
            try
            {
                DataTable dtResult = OraDBAdapter.GetDataTable(DBCS.Default.ConnectionString, sqlText.ToString(), param);
                dtResult.TableName = className;
                LogHelper.GetLogger("--End:" + classInfo + "").Info("Completed");
                return dtResult;
            }
            catch (Exception ex)
            {
                logInfo += ex.ToString();
                LogHelper.GetLogger("" + classInfo + "").Error(logInfo);
                throw ex;
            }
        }

        /// <summary>
        /// DB数据以object list形式返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlText"></param>
        /// <param name="className"></param>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        public static IList<T> GetOraObjList<T>(StringBuilder sqlText, string className, string classInfo) where T : class, new()
        {
            return GetOraDateTable(sqlText, className, classInfo).ToList<T>();
        }

        public static IList<T> GetOraObjList<T>(StringBuilder sqlText, OracleParameter[] param, string className, string classInfo) where T : class, new()
        {
            return GetOraDateTable(sqlText, param, className, classInfo).ToList<T>();
        }

        /// <summary>
        /// 更动SQL Server DB Data
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="className"></param>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        public static bool ExcuteSql(StringBuilder sqlText, string className, string classInfo)
        {
            string logInfo = "【" + className + "】:" + sqlText.ToString() + "\r\n";
            LogHelper.GetLogger("--Begin:" + classInfo + "").Info(logInfo);
            try
            {
                bool result = SqlDBAdapter.ExecuteNonQuery(DBCS.Default.ConnectionString, sqlText.ToString());
                LogHelper.GetLogger("--End:" + classInfo + "").Info("Completed");
                return result;
            }
            catch (Exception ex)
            {
                logInfo += ex.ToString();
                LogHelper.GetLogger("" + classInfo + "").Error(logInfo);
                throw ex;
            }
        }

        /// <summary>
        /// 更动Oracle DB Data
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="className"></param>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        public static bool ExcuteOraSql(StringBuilder sqlText, string className, string classInfo)
        {
            string logInfo = "【" + className + "】:" + sqlText.ToString() + "\r\n";
            LogHelper.GetLogger("--Begin:" + classInfo + "").Info(logInfo);
            try
            {
                bool result = OraDBAdapter.ExecuteNonQuery(DBCS.Default.ConnectionString, sqlText.ToString());
                LogHelper.GetLogger("--End:" + classInfo + "").Info("Completed");
                return result;
            }
            catch (Exception ex)
            {
                logInfo += ex.ToString();
                LogHelper.GetLogger("" + classInfo + "").Error(logInfo);
                throw ex;
            }
        }

        /// <summary>
        /// 更动Oracle DB Data
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="param"></param>
        /// <param name="className"></param>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        public static bool ExcuteOraSql(StringBuilder sqlText, OracleParameter[] param, string className, string classInfo)
        {
            string logInfo = "【" + className + "】:" + sqlText.ToString() + "\r\n";
            LogHelper.GetLogger("--Begin:" + classInfo + "").Info(logInfo);
            try
            {
                bool result = OraDBAdapter.ExecuteNonQuery(DBCS.Default.ConnectionString, sqlText.ToString(), param);
                LogHelper.GetLogger("--End:" + classInfo + "").Info("Completed");
                return result;
            }
            catch (Exception ex)
            {
                logInfo += ex.ToString();
                LogHelper.GetLogger("" + classInfo + "").Error(logInfo);
                throw ex;
            }
        }

        public static bool ExecuteStoredProcedure(string storedProcedureName)
        {
            try
            {
                bool result = OraDBAdapter.ExecuteStoredProcedure(DBCS.Default.ConnectionString, storedProcedureName);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ExecuteStoredProcedure(string storedProcedureName, OracleParameter[] param)
        {
            try
            {
                bool result = OraDBAdapter.ExecuteStoredProcedure(DBCS.Default.ConnectionString, storedProcedureName, param);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable ExecuteStoredProcedureGetDataTable(string storedProcedureName, OracleParameter[] param)
        {
            try
            {
                DataTable result = OraDBAdapter.ExecuteStoredProcedureGetDataTable(DBCS.Default.ConnectionString, storedProcedureName, param);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
