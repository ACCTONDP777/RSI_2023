using Oracle.ManagedDataAccess.Client;
using RSI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace RSI.Models.Entity
{
    #region DTO
    public class To_Do_List_VEntity
    {
        public decimal RSI_NO { get; set; }
        public string FORM_NO { get; set; }
        public string PHASE_ID { get; set; }
        public string PART_TYPE { get; set; }
        public string BU { get; set; }
        public string PHASE { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public DateTime REQUEST_DATE { get; set; }
        public string PROJECT_NAME { get; set; }
        public string REQUESTNAME { get; set; }
        public decimal HELDDAYS { get; set; }
        public string RETURN_PHASE_ID { get; set; }
        public decimal APP_SERIAL { get; set; }
        public string APP_STATUS { get; set; }
        public string TAG { get; set; }
        public string REMARK { get; set; }

        public string SOURCER_APPROVE { get; set; }
    }
    #endregion



    #region DAL
    public static class To_Do_List_VEntityDAL
    {
        public static IList<To_Do_List_VEntity> GetTo_Do_List_VEntities(string emp_no)
        {
            IList<To_Do_List_VEntity> list = new List<To_Do_List_VEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM RSI_TO_DO_LIST_V 
                                                WHERE ( :emp_no is null or APP_ASSIGNER = :emp_no) 
                                                ORDER BY PHASE_ID, HELDDAYS ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, String.IsNullOrEmpty(emp_no) ? DBNull.Value : (object)emp_no, ParameterDirection.Input));

               list = SqlExcute.GetOraObjList<To_Do_List_VEntity>(sqlText, param.ToArray(), "GetTo_Do_List_VEntities", "獲取To_Do_List_V資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return list;
            }
            return list;
        }

        public static DataTable GetContactWindow()
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT ATTRIBUTE1, ATTRIBUTE2 FROM RSI_C_PARAMETER WHERE FUNCTION = 'Contact_Windows' ");

                dt = SqlExcute.GetOraDateTable(sqlText, "GetContactWindow", "獲取ContactWindow資訊");
            }
            catch (Exception)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dt;
            }
            return dt;
        }

        public static DataTable GetFAQ()
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM RSI_C_PARAMETER WHERE FUNCTION = 'FAQ' ");

                dt = SqlExcute.GetOraDateTable(sqlText, "GetFAQ", "獲取FAQ資訊");
            }
            catch (Exception)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dt;
            }
            return dt;
        }

        public static DataTable GetRSI_H_Todo_Note(string rsi_no, string form_no, string phase_id, string app_serial, string app_status)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM RSI_H_TODO_NOTE 
                                 WHERE rsi_no = :rsi_no 
                                 AND form_no = :form_no
                                 AND phase_id = :phase_id
                                 AND app_serial = :app_serial
                                 AND app_status = :app_status
                                 AND emp_no = :emp_no ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_no", OracleDbType.NVarchar2, form_no, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("app_serial", OracleDbType.NVarchar2, app_serial, ParameterDirection.Input));
                param.Add(new OracleParameter("app_status", OracleDbType.NVarchar2, app_status, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetRSI_H_Todo_Note", "獲取RSI_H_TODO_NOTE資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dt;
            }
            return dt;
        }

        public static bool InsRSI_H_Todo_NoteByTag(string rsi_no, string form_no, string phase_id, string app_serial, string app_status, string tag)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"INSERT INTO RSI_H_TODO_NOTE(rsi_no, form_no, app_serial, app_status, phase_id, emp_no, flag, lm_user, lm_time)
                                 VALUES (:rsi_no, :form_no, :app_serial, :app_status, :phase_id, :emp_no, :flag, :emp_no, sysdate) ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_no", OracleDbType.NVarchar2, form_no, ParameterDirection.Input));
                param.Add(new OracleParameter("app_serial", OracleDbType.NVarchar2, app_serial, ParameterDirection.Input));
                param.Add(new OracleParameter("app_status", OracleDbType.NVarchar2, app_status, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("flag", OracleDbType.NVarchar2, tag, ParameterDirection.Input));
                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "InsRSI_H_Todo_NoteByTag", "新增RSI_H_Todo_Note Flag資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
        }

        public static bool UdtRSI_H_Todo_NoteByTag(string rsi_no, string form_no, string phase_id, string app_serial, string app_status, string tag)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE RSI_H_TODO_NOTE
                                 SET flag = :flag,
                                     lm_user = :emp_no,
                                     lm_time = sysdate
                                 where rsi_no = :rsi_no
                                 and   form_no = :form_no
                                 and   app_serial = :app_serial
                                 and   app_status = :app_status
                                 and   phase_id = :phase_id
                                 and   emp_no = :emp_no");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("flag", OracleDbType.NVarchar2, tag, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_no", OracleDbType.NVarchar2, form_no, ParameterDirection.Input));
                param.Add(new OracleParameter("app_serial", OracleDbType.NVarchar2, app_serial, ParameterDirection.Input));
                param.Add(new OracleParameter("app_status", OracleDbType.NVarchar2, app_status, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UdtRSI_H_Todo_NoteByTag", "更新RSI_H_Todo_Note Flag資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
        }

        public static bool InsRSI_H_Todo_NoteByRemark(string rsi_no, string form_no, string phase_id, string app_serial, string app_status, string remark)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"INSERT INTO RSI_H_TODO_NOTE(rsi_no, form_no, app_serial, app_status, phase_id, emp_no, remark, lm_user, lm_time)
                                 VALUES (:rsi_no, :form_no, :app_serial, :app_status, :phase_id, :emp_no, :remark, :emp_no, sysdate)");
                var param = new List<OracleParameter>();
                
               
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_no", OracleDbType.NVarchar2, form_no, ParameterDirection.Input));
                param.Add(new OracleParameter("app_serial", OracleDbType.NVarchar2, app_serial, ParameterDirection.Input));
                param.Add(new OracleParameter("app_status", OracleDbType.NVarchar2, app_status, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("remark", OracleDbType.NVarchar2, remark, ParameterDirection.Input));
                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "InsRSI_H_Todo_NoteByRemark", "更新RSI_H_Todo_Note Remark資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
        }

        public static bool UdtRSI_H_Todo_NoteByRemark(string rsi_no, string form_no, string phase_id, string app_serial, string app_status, string remark)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE RSI_H_TODO_NOTE
                                 SET remark = :remark,
                                     lm_user = :emp_no,
                                     lm_time = sysdate
                                 where rsi_no = :rsi_no
                                 and   form_no = :form_no
                                 and   app_serial = :app_serial
                                 and   app_status = :app_status
                                 and   phase_id = :phase_id
                                 and   emp_no = :emp_no");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("remark", OracleDbType.NVarchar2, remark, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_no", OracleDbType.NVarchar2, form_no, ParameterDirection.Input));
                param.Add(new OracleParameter("app_serial", OracleDbType.NVarchar2, app_serial, ParameterDirection.Input));
                param.Add(new OracleParameter("app_status", OracleDbType.NVarchar2, app_status, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UdtRSI_H_Todo_NoteByRemark", "更新RSI_H_Todo_Note Remark資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
        }
    }
    #endregion

}