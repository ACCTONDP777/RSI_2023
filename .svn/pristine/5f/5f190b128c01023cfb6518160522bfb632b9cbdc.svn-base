using AUO.Common.Authentication;
using Oracle.ManagedDataAccess.Client;
using RSI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace RSI.Models.Entity
{
    public class EmployeeModel
    {
        public string EMP_NO { get; set;}
        public string EMP_NAME { get; set; }
    }

    public static class Employee
    {
        public static string EmpNO
        {
            get
            {
                FormsAuthenticationTicket ticket = (HttpContext.Current.User.Identity as FormsIdentity).Ticket;
                FormsAuthTicketData ticketData = new FormsAuthTicketData(ticket);
                return ticketData.EmployeeId;
            }
        }

        public static string EmpName
        {
            get
            {
                FormsAuthenticationTicket ticket = (HttpContext.Current.User.Identity as FormsIdentity).Ticket;
                FormsAuthTicketData ticketData = new FormsAuthTicketData(ticket);
                return ticketData.RealName;
            }
        }
        public static string GetDepartmentName(string emp_id)
        {
            string department_name = String.Empty;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" SELECT * 
                                                FROM GFIN.EMP_DATA_ALL E, GFIN.ORG_DATA_ALL O
                                                WHERE E.ORG_ID = O.ORG_ID
                                                AND E.EMP_NO = :emp_no ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_id, ParameterDirection.Input));

                var dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetDepartmentName", "獲取Department Name資訊");
                department_name = dt.Rows[0]["ORG_CNAME"].ToString();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return department_name;
            }
            return department_name;
        }

        public static string GetOtherEmpName(string emp_no)
        {
            string emp_name = String.Empty;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" SELECT * 
                                                FROM GFIN.EMP_DATA_ALL E, GFIN.ORG_DATA_ALL O
                                                WHERE E.ORG_ID = O.ORG_ID
                                                AND E.EMP_NO = :emp_no ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));

                var dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetOtherEmpName", "獲取EMP_NAME資訊");
                emp_name = dt.Rows[0]["EMP_NAME"].ToString();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return emp_name;
            }
            return emp_name;
        }

        public static string GetSiteName(string phase_id, string bu)
        {
            bu = Validate.DecryptValue(bu);
            string siteName = String.Empty;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT PHASE_ID, PHASE FROM GPO.RSI_C_FLOW_DEF WHERE PHASE_ID = :phase_id and BU = :bu ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));

                var dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetSiteName", "獲取Site Name資訊");
                siteName = dt.Rows[0]["PHASE"].ToString();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return siteName;
            }
            return siteName;
        }

        public static string GetLogidID(string emp_no)
        {
            string LogidID = String.Empty;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" SELECT * 
                                                FROM GFIN.EMP_DATA_ALL E, GFIN.ORG_DATA_ALL O
                                                WHERE E.ORG_ID = O.ORG_ID
                                                AND E.EMP_NO = :emp_no ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));

                var dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetLogidID", "獲取登入帳號資訊");
                LogidID = dt.Rows[0]["LOGONID"].ToString();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return LogidID;
            }
            return LogidID;
        }

    public static IList<EmployeeModel> GetEmpoyee(string search, string phase_id, string part_type, string bu)
        {
            IList<EmployeeModel> employees = new List<EmployeeModel>();
            try
            {
                //if (phase_id == "40")
                //{
                    StringBuilder sqlText = new StringBuilder();
                    //可查詢工號/姓名/英文名子/分機
                    sqlText.Append(@"SELECT DISTINCT E.EMP_NO, E.ENG_NAME AS EMP_NAME
                                               FROM GFIN.EMP_DATA_ALL E
                                               WHERE (1 = 1)
                                               AND E.ACTIVE='Y'
                                               AND ((E.EMP_NO LIKE '%{0}%') OR (UPPER(E.ENG_NAME) LIKE UPPER('%{0}%')) OR
                                                         (REPLACE(E.EXT_NO, '-', '') LIKE '%{0}%'))
                                               AND ORG_ID IN
                                                        (SELECT DISTINCT ORG_ID
                                                         FROM GFIN.ORG_DATA_ALL
                                                         WHERE PARENT_ORG_ID IN
                                                        (SELECT DISTINCT PARENT_ORG_ID
                                                         FROM GFIN.ORG_DATA_ALL
                                                         WHERE ORG_ID IN
                                                        (SELECT DISTINCT E.ORG_ID
                                                         FROM GPO.RSI_H_AUTHORITY H, GFIN.EMP_DATA_ALL E
                                                         WHERE H.EMP_NO = E.EMP_NO
                                                         AND H.ACTIVE = 'Y'
                                                         AND E.ACTIVE = 'Y'
                                                         AND H.TYPE = 'FORM'
                                                         AND SYSDATE >= H.START_DATE
                                                         AND SYSDATE <= H.END_DATE)))
                                               and e.eng_name is not null ");

                    //var param = new List<OracleParameter>();
                    //param.Add(new OracleParameter("search", OracleDbType.NVarchar2, String.IsNullOrEmpty(search) ? DBNull.Value : (Object)search, ParameterDirection.Input));
                    sqlText.Replace("{0}", search);
                    employees = SqlExcute.GetOraObjList<EmployeeModel>(sqlText, "GetEmpoyee", "獲取Empoyee資訊");
                //}
                //else
                //{
                //    StringBuilder sqlText = new StringBuilder();
                //    sqlText.Append(@"SELECT DISTINCT E.EMP_NO, E.ENG_NAME AS EMP_NAME
                //                                    FROM GFIN.EMP_DATA_ALL E, GPO.RSI_USER_MAC M
                //                                    WHERE (1 = 1)
                //                                    AND E.EMP_NO = M.EMP_NO
                //                                    AND M.ACTIVE = 'Y'
                //                                    AND ((E.EMP_NO LIKE '%{0}%') OR (UPPER(E.ENG_NAME) LIKE UPPER('%{0}%')) OR
                //                                        (REPLACE(E.EXT_NO, '-', '') LIKE '%{0}%'))
                //                                    AND EXISTS ( 
                //                                                SELECT DISTINCT E.ORG_ID
                //                                                  FROM GPO.RSI_H_PS_AUTHORITY H, GFIN.EMP_DATA_ALL E, GPO.RSI_USER_MAC M
                //                                                 WHERE H.EMP_NO = E.EMP_NO
                //                                                   AND H.EMP_NO = M.EMP_NO
                //                                                   AND H.ACTIVE = 'Y'
                //                                                   AND E.ACTIVE = 'Y'
                //                                                   AND M.ACTIVE = 'Y'
                //                                                   AND SYSDATE >= H.START_DATE
                //                                                   AND SYSDATE <= H.END_DATE
                //                                                   AND H.EMP_NO = :emp_no  --:emp_no
                //                                                   AND H.PART_TYPE = :part_type    --:part_type
                //                                                   AND H.BU = :bu              --:bu
                //                                                ) ");
                //    sqlText.Replace("{0}", search);
                //    var param = new List<OracleParameter>();
                //    param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, EmpNO, ParameterDirection.Input));
                //    param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                //    param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));
                //    employees = SqlExcute.GetOraObjList<EmployeeModel>(sqlText, param.ToArray(), "GetEmpoyee", "獲取Empoyee資訊");
                //}
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return employees;
            }
            return employees;
        }

        public static DataTable Auto_Identify(string rsi_no, string emp_no, string phase_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select count(*) as count
                                                from RSI_TO_DO_LIST_V a
                                                where a.APP_ASSIGNER = :emp_no
                                                and a.rsi_no = :rsi_no
                                                and a.phase_id = :phase_id ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));

                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "Auto_Identify", "");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return new DataTable();
            }
        }

        public static DataTable CheckMacAddress(string emp_no, string part_type, string bu, string macaddress)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT COUNT(*) AS COUNT
                                                FROM GPO.RSI_H_PS_AUTHORITY H, GPO.RSI_USER_MAC M
                                                WHERE H.EMP_NO = M.EMP_NO
                                                AND H.ACTIVE = 'Y'
                                                AND M.ACTIVE = 'Y'
                                                AND SYSDATE >= H.START_DATE
                                                AND SYSDATE <= H.END_DATE
                                                AND H.EMP_NO = :emp_no   --:emp_no
                                                AND H.PART_TYPE = :part_type     --:part_type
                                                AND H.BU = :bu            --:bu 
                                                AND M.MAC = :macaddress ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("macaddress", OracleDbType.NVarchar2, macaddress, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "CheckMacAddress", String.Empty);
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
    }
}