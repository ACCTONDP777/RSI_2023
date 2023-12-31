﻿using Oracle.ManagedDataAccess.Client;
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
    public class DASHBOARD_LIST_VEntity
    {
        public decimal RSI_NO { get; set; }
        public string BU { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public DateTime REQUEST_DATE{get;set;}
        public string PROJECT_NAME { get; set; }
        public string REQUEST_NAME { get; set; }
        public decimal HELDDAYS { get; set; }
        public decimal TOTALDAYS { get; set; }
        public string PHASE_ID { get; set; }
        public string PART_TYPE { get; set; }
        public string PM_REQUEST { get; set; }
        public string RD { get; set; }
        public string RD_BOSS { get; set; }
        public string RD_MANAGER { get; set; }
        public string PM_JUDGE { get; set; }
        public string SOURCER { get; set; }

        public string SOURCER_PHASE_ID { get; set; }
        public string SOURCER_BOSS { get; set; }
        public string SOURCER_MANAGER { get; set; }
        public string AUO { get; set; }
        public string PM_CONFIRM { get; set; }
        public string PMCONFIRM_PHASE_ID { get; set; }
        public string PM_CANCEL { get; set; }
        public string FORM_STATUS { get; set; }
    }

    public class DashBoardVM
    {
        public IList<DASHBOARD_LIST_VEntity> UA { get; set; }
        public IList<DASHBOARD_LIST_VEntity> AP { get; set; }
    }

    public class ContractStatus
    {
        public string ATTRIBUTE3 { get; set; }
        public string ATTRIBUTE2 { get; set; }
    }

    public class DetailApprove
    {
        public decimal RSI_NO { get; set; }
        public decimal FORM_NO { get; set; }
        public string PART_TYPE { get; set; }
        public string PHASE_ID { get; set; }
        public string PHASE { get; set; }
        public decimal APP_SERIAL { get; set; }
        public string APP_ASSIGNER { get; set; }
        public string APP_ASSIGNER_NAME { get; set; }       
        public string APP_ACTOR { get; set; }
        public string APP_ACTOR_NAME { get; set;}
        public string STATUS { get; set; }
        public DateTime? BEGIN_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public string APP_CONTENT { get; set; }
        public decimal? DIF_DAY { get; set; }
    }
    #endregion

    #region DAL
    public static class DASHBOARD_LIST_VEntityDAL
    {
        public static IList<DASHBOARD_LIST_VEntity> GetDashBoardUA(string owner, string contractStatus, string project_name)
        {
            IList<DASHBOARD_LIST_VEntity> list = new List<DASHBOARD_LIST_VEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                if (owner.Equals("1"))
                {
                    sqlText.Append(@"SELECT * FROM GPO.RSI_DASHBOARD_LIST_V A
                                                   JOIN (
                                                        SELECT DISTINCT B.RSI_NO FROM GPO.RSI_H_FORM_HEADER B
                                                        WHERE FORM_NO IN
                                                        (SELECT A.FORM_NO    --自己簽核過表單
                                                         FROM GPO.RSI_H_FORM_APPROVE A
                                                         LEFT JOIN GPO.RSI_H_DETAIL_APP_LOG L ON A.FORM_NO = L.FORM_NO
                                                         WHERE (A.APP_ASSIGNER = :emp_no OR A.APP_ACTOR = :emp_no OR L.APP_ACTOR = :emp_no)
                                                         UNION            --下轄人員簽核過表單
                                                         SELECT DISTINCT B.FORM_NO 
                                                         FROM au_dw.auo_person_reportline A
                                                         INNER JOIN GPO.RSI_H_FORM_APPROVE B ON A.EMP_NO = NVL(B.APP_ACTOR, B.APP_ASSIGNER)
                                                         LEFT JOIN GPO.RSI_H_DETAIL_APP_LOG L ON B.FORM_NO = L.FORM_NO AND A.EMP_NO = L.APP_ACTOR
                                                         WHERE (TRIM(A.BOSS_EMP_NO) = :emp_no)
                                                         AND A.ACTIVE = 'Y')                
                                                   ) B ON A.RSI_NO = B.RSI_NO 
                                                   WHERE DECODE(:contractStatus, 'ALL', 1, INSTR(:contractStatus, A.FORM_STATUS)) > 0 
                                                   AND ((:rsi_no is null or A.RSI_NO LIKE :rsi_no) 
                                                   OR (:bu is null or UPPER(A.BU) LIKE :bu) 
                                                   OR (:project_name is null or UPPER(A.PROJECT_NAME) LIKE :project_name) 
                                                   OR (:request_name is null or UPPER(A.REQUEST_NAME) LIKE :request_name)) 
                                                ORDER BY A.RSI_NO DESC, DECODE(A.PART_TYPE, '', 1 ,'ACD', 2, 'EE', 3, 'OM', 4, 'PACKING', 5, 6)   ");

                    var param = new List<OracleParameter>();
                    param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                    param.Add(new OracleParameter("contractStatus", OracleDbType.NVarchar2, contractStatus, ParameterDirection.Input));
                    param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, String.IsNullOrEmpty(project_name) ? DBNull.Value : (Object)String.Format("%{0}%", project_name.ToUpper()), ParameterDirection.Input));
                    param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, String.IsNullOrEmpty(project_name) ? DBNull.Value : (Object)String.Format("%{0}%", project_name.ToUpper()), ParameterDirection.Input));
                    param.Add(new OracleParameter("project_name", OracleDbType.NVarchar2, String.IsNullOrEmpty(project_name) ? DBNull.Value : (Object)String.Format("%{0}%", project_name.ToUpper()), ParameterDirection.Input));
                    param.Add(new OracleParameter("request_name", OracleDbType.NVarchar2, String.IsNullOrEmpty(project_name) ? DBNull.Value : (Object)String.Format("%{0}%", project_name.ToUpper()), ParameterDirection.Input));
                    list = SqlExcute.GetOraObjList<DASHBOARD_LIST_VEntity>(sqlText, param.ToArray(), "GetDashBoardUA", "獲取DashBoard UA資訊1");
                }

                if (owner.Equals("2"))
                {
                    sqlText.Append(@"SELECT * FROM GPO.RSI_DASHBOARD_LIST_V 
                                                   WHERE DECODE(:contractStatus, 'ALL', 1, INSTR(:contractStatus, FORM_STATUS)) > 0  
                                                   AND ((:rsi_no is null or RSI_NO LIKE :rsi_no) 
                                                   OR (:bu is null or UPPER(BU) LIKE :bu) 
                                                   OR (:project_name is null or UPPER(PROJECT_NAME) LIKE :project_name) 
                                                   OR (:request_name is null or UPPER(REQUEST_NAME) LIKE :request_name)) 
                                                ORDER BY RSI_NO DESC, DECODE(PART_TYPE, '', 1 ,'ACD', 2, 'EE', 3, 'OM', 4, 'PACKING', 5, 6)  ");

                    var param = new List<OracleParameter>();
                    param.Add(new OracleParameter("contractStatus", OracleDbType.NVarchar2, contractStatus, ParameterDirection.Input));
                    param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, String.IsNullOrEmpty(project_name) ? DBNull.Value : (Object)String.Format("%{0}%", project_name.ToUpper()), ParameterDirection.Input));
                    param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, String.IsNullOrEmpty(project_name) ? DBNull.Value : (Object)String.Format("%{0}%", project_name.ToUpper()), ParameterDirection.Input));
                    param.Add(new OracleParameter("project_name", OracleDbType.NVarchar2, String.IsNullOrEmpty(project_name) ? DBNull.Value : (Object)String.Format("%{0}%", project_name.ToUpper()), ParameterDirection.Input));
                    param.Add(new OracleParameter("request_name", OracleDbType.NVarchar2, String.IsNullOrEmpty(project_name) ? DBNull.Value : (Object)String.Format("%{0}%", project_name.ToUpper()), ParameterDirection.Input));
                    list = SqlExcute.GetOraObjList<DASHBOARD_LIST_VEntity>(sqlText, param.ToArray(), "GetDashBoardUA", "獲取DashBoard UA資訊2");
                }
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return list;
            }
            return list;
        }

        public static IList<DASHBOARD_LIST_VEntity> GetDashBoardAP()
        {
            IList<DASHBOARD_LIST_VEntity> list = new List<DASHBOARD_LIST_VEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM GPO.RSI_DASHBOARD_LIST_V WHERE FORM_STATUS = 'AP' ");

                list = SqlExcute.GetOraObjList<DASHBOARD_LIST_VEntity>(sqlText, "GetDashBoardUA", "獲取DashBoard UA資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return list;
            }
            return list;
        }

        public static IList<ContractStatus> GetContractStatus()
        {
            IList<ContractStatus> list = new List<ContractStatus>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT ATTRIBUTE3, ATTRIBUTE2 
                                   FROM GPO.RSI_C_PARAMETER 
                                  WHERE FUNCTION = 'FORM_STATUS' 
                                  ORDER BY DECODE(ATTRIBUTE2, 'UA', 0, 'AP', 1, 'CL', 2, 'RC', 3, 4) ");

                list = SqlExcute.GetOraObjList<ContractStatus>(sqlText, "GetContractStatus", "獲取Contract Status資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return list;
            }
            return list;
        }

        public static IList<DetailApprove> GetDetailApprove(string rsi_no, string part_type)
        {
            try
            {
                IList<DetailApprove> list = new List<DetailApprove>();
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"  SELECT A.RSI_NO, A.FORM_NO, A.PART_TYPE, A.PHASE_ID, A.PHASE, A.APP_SERIAL, 
                                          A.APP_ASSIGNER, A.APP_ASSIGNER_NAME, A.APP_ACTOR, A.APP_ACTOR_NAME,
                                          A.STATUS, A.BEGIN_DATE, A.END_DATE,
                                          A.APP_CONTENT, A.DIF_DAY
                                          FROM RSI_APPROVAL_LIST_V A
                                          WHERE A.RSI_NO = :rsi_no
                                          AND A.PART_TYPE = :part_type ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                list = SqlExcute.GetOraObjList<DetailApprove>(sqlText, param.ToArray(), "GetDetailApprove", String.Empty);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetDashBoardRSINO(string project_name, string rfq_no, string rfq_ver)
        {
            try
            {
                string sql = @"select rsi_no from RSI_H_PRODUCT_INFO where 1=1 ";
                var param = new List<OracleParameter>();
                if (!String.IsNullOrEmpty(project_name))
                {
                    sql += "and project_name = :project_name ";
                    param.Add(new OracleParameter("project_name", OracleDbType.NVarchar2, project_name, ParameterDirection.Input));
                }

                if (!String.IsNullOrEmpty(rfq_no) && !String.IsNullOrEmpty(rfq_ver))
                {
                    sql += "and (rfq_no = :rfq_no and rfq_ver = :rfq_ver) ";
                    param.Add(new OracleParameter("rfq_no", OracleDbType.NVarchar2, rfq_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("rfq_ver", OracleDbType.NVarchar2, rfq_ver, ParameterDirection.Input));
                }
                sql += " order by rsi_no desc ";

                StringBuilder sqlText = new StringBuilder(sql);
                DataTable result = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetDashBoardRSINO", string.Empty);
                if (result.Rows.Count > 0)
                    return result.Rows[0][0].ToString();
                else
                return String.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion
}