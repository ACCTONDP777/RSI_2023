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
    public class RSIWebServiceEntity
    {
        public string BU { get; set; }
        public string PROJECT_NAME { get; set; }
        public string RFQ_NO { get; set; }
        public string RFQ_VER { get; set; }
        public string APPLY_TYPE { get; set; }
        public string LCM { get; set; }
        public string MODEL { get; set; }
        public string REF_PRODUCT_RFQ { get; set; }
        public string APPLY_REASON { get; set; }
        public string RFQ_TYPE { get; set; }
        public string BL_NIT { get; set; }
        public string POWER { get; set; }
        public string PM { get; set; }
        public string PL_RD { get; set; }
        public string ACD_RD { get; set; }
        public string EE_RD { get; set; }
        public string OM_RD { get; set; }
        public string PACKING_RD { get; set; }
        public string TP_ACD_RD { get; set; }
        public string TP_EE_RD { get; set; }
        public string TP_OM_RD { get; set; }
        public string REQUEST_DATE { get; set; }
        public string REF_RFQ_NO { get; set; }
        public string REF_RFQ_VER { get; set; }
        public string EvaluatorACD { get; set; }
        public string EvaluatorOM { get; set; }
        public string EvaluatorEE { get; set; }
        public string EvaluatorPacking { get; set; }
        public string EvaluatorTPACDRD { get; set; }
        public string EvaluatorTPOMRD { get; set; }
        public string EvaluatorTPEERD { get; set; }
        public string RdEvaluationFinish { get; set; }
        public string BIDDING_PROJECT { get; set; }
        public string SOURCING_POOL_NAME { get; set; }
        public string ACTION { get; set; }
        public string PROCESS_TYPE { get; set; }
        public string PRODUCT_COMBINATION_NAME { get; set; }
        public string PRODUCT_TYPE_NAME { get; set; }
        public string PROCESS_CLASS { get; set; }
        public string ARRAY_FAB { get; set; }
        public string JI_FAB { get; set; }
        public string MODULE_FAB { get; set; }
        public string SIZE { get; set; }
        public string SURFACE_TREATMENT { get; set; }
        public string SOP_YEAR { get; set; }
        public string SOP_QUARTERLY { get; set; }
        public string SOP_MONTH { get; set; }
        public string RESOLUTION { get; set; }
        public string PM_LCD_TYPE { get; set; }
        public string PM_BRIGHTNESS { get; set; }
        public string FCST_QTY { get; set; }
        public string SAMPLE_REQUEST { get; set; }
        public string SAMPLE_REQUEST_DATE { get; set; }
        public string SAMPLE_REQUEST_QTY { get; set; }
        public string SOURCE_FROM { get; set; }
    }

    public class RSICurrentHandler
    {
        public string BU { get; set; }
        public string PROJECT_NAME { get; set; }
        public string RSI_STATUS { get; set; }

    }

    public static class RSIWebServiceDAL
    {
        public static void InsertH_Product_Info(RSIWebServiceEntity model)
        {
            string rsi_no = GetRSI_NO();
            string tp = String.Empty;
            string rsi_status = "RFQ";
            string ref_product = String.Empty;
            string ref_product_mfg = String.Empty;
            if (!String.IsNullOrEmpty(model.REF_PRODUCT_RFQ))
            {
                var arr_split = model.REF_PRODUCT_RFQ.Split('-');
                if (arr_split.Length == 1)
                {
                    ref_product = model.REF_PRODUCT_RFQ.Split('-')[0].ToString();
                }
                else
                {
                    ref_product = model.REF_PRODUCT_RFQ.Split('-')[0].ToString();
                    ref_product_mfg = model.REF_PRODUCT_RFQ.Split('-')[1].ToString();
                }
            }
            string ref_rsi_no = String.Empty;
            string source_from = "ADP";
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"INSERT INTO RSI_H_PRODUCT_INFO
                                  (RSI_NO, BU, PROJECT_NAME, RFQ_NO, RFQ_VER, APPLY_TYPE, LCM, 
                                   TP, MODEL, REF_PRODUCT_RFQ, APPLY_REASON, RFQ_TYPE, BL_NIT, POWER, PM, PL_RD,
                                   ACD_RD, EE_RD, OM_RD, PACKING_RD, TP_ACD_RD, TP_EE_RD, TP_OM_RD,
                                   REQUEST_DATE, RSI_STATUS, REF_PRODUCT, REF_PRODUCT_MFG, REF_RSI_NO, CREATED_DATE,
                                   REF_RFQ_NO, REF_RFQ_VER, EVAL_ACD, EVAL_OM, EVAL_EE, EVAL_PACKING, EVAL_TPACD, EVAL_TPOM,
                                   EVAL_TPEE, EVAL_STATUS, BIDDING_PROJECT, SOURCING_POOL_NAME, PROCESS_TYPE, PRODUCT_COMBINATION_NAME, 
                                   PRODUCT_TYPE_NAME, PROCESS_CLASS, ARRAY_FAB, JI_FAB, MODULE_FAB, SIZE_INCH, SURFACE_TREATMENT, 
                                   SOP_YEAR, SOP_QUARTERLY, SOP_MONTH, RESOLUTION, PM_LCD_TYPE, PM_BRIGHTNESS, FCST_QTY, SAMPLE_REQUEST, SAMPLE_REQUEST_DATE, SAMPLE_REQUEST_QTY, SOURCE_FROM) 
                                VALUES 
                                  (:rsi_no, :bu, :project_name, :rfq_no, :rfq_ver, :apply_type, :lcm, :tp, :model, :ref_product_rfq,
                                   :apply_reason, :rfq_type, :bl_nit, :power, :pm, :pl_rd, :acd_rd, :ee_rd, :om_rd, :packing_rd,
                                   :tp_acd_rd, :tp_ee_rd, :tp_om_rd, TO_DATE(:request_date,  'yyyy/mm/dd'), 
                                   :rsi_status, :ref_product, :ref_product_mfg, :ref_rsi_no, SYSDATE,
                                   :ref_rfq_no, :ref_rfq_ver, :eval_acd, :eval_om, :eval_ee, :eval_packing, :eval_tpacd, :eval_tpom,
                                   :eval_tpee, :eval_status , :bidding_project, :sourcing_pool_name, :process_type, :product_combination_name, 
                                   :product_type_name, :process_class, :array_fab, :ji_fab, :module_fab, :size_inch, :surface_treatment, 
                                   :sop_year, :sop_quarterly, :sop_month, :resolution, :pm_lcd_type, :pm_brightness, :fcst_qty, :sample_request, :sample_request_date, :sample_request_qty, :source_from) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("bu", OracleDbType.Varchar2, model.BU.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("project_name", OracleDbType.Varchar2, model.PROJECT_NAME.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("apply_type", OracleDbType.Varchar2, model.APPLY_TYPE.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("lcm", OracleDbType.Varchar2, String.IsNullOrEmpty(model.LCM) ? DBNull.Value : (Object)model.LCM.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("tp", OracleDbType.Varchar2, String.IsNullOrEmpty(tp) ? DBNull.Value : (Object)tp.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("model", OracleDbType.Varchar2, model.MODEL.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("ref_product_rfq", OracleDbType.Varchar2, model.REF_PRODUCT_RFQ.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("apply_reason", OracleDbType.Varchar2, String.IsNullOrEmpty(model.APPLY_REASON) ? DBNull.Value : (Object)model.APPLY_REASON.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_type", OracleDbType.Varchar2, model.RFQ_TYPE.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("bl_nit", OracleDbType.Varchar2, DBNull.Value, ParameterDirection.Input));
                param.Add(new OracleParameter("power", OracleDbType.Varchar2, DBNull.Value, ParameterDirection.Input));
                param.Add(new OracleParameter("pm", OracleDbType.Varchar2, model.PM.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("pl_rd", OracleDbType.Varchar2, model.PL_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("acd_rd", OracleDbType.Varchar2, model.ACD_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("ee_rd", OracleDbType.Varchar2, model.EE_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("om_rd", OracleDbType.Varchar2, model.OM_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("packing_rd", OracleDbType.Varchar2, model.PACKING_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("tp_acd_rd", OracleDbType.Varchar2, model.TP_ACD_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("tp_ee_rd", OracleDbType.Varchar2, model.TP_EE_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("tp_om_rd", OracleDbType.Varchar2, model.TP_OM_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("request_date", OracleDbType.Varchar2, DateTime.Now.ToString("yyyy/MM/dd"), ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_status", OracleDbType.Varchar2, rsi_status.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("ref_product", OracleDbType.Varchar2, ref_product.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("ref_product_mfg", OracleDbType.Varchar2, ref_product_mfg.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("ref_rsi_no", OracleDbType.Varchar2, ref_rsi_no.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("ref_rfq_no", OracleDbType.Varchar2, String.IsNullOrEmpty(model.REF_RFQ_NO) ? DBNull.Value : (Object)model.REF_RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("ref_rfq_ver", OracleDbType.Varchar2, String.IsNullOrEmpty(model.REF_RFQ_VER) ? DBNull.Value : (Object)model.REF_RFQ_VER.Trim(), ParameterDirection.Input));
                //20180927 OWEN ADD MS需要RFQ技術評估，需要分批次傳至RSI
                param.Add(new OracleParameter("eval_acd", OracleDbType.Varchar2, String.IsNullOrEmpty(model.EvaluatorACD) ? DBNull.Value : (Object)model.EvaluatorACD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_om", OracleDbType.Varchar2, String.IsNullOrEmpty(model.EvaluatorOM) ? DBNull.Value : (Object)model.EvaluatorOM.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_ee", OracleDbType.Varchar2, String.IsNullOrEmpty(model.EvaluatorEE) ? DBNull.Value : (Object)model.EvaluatorEE.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_packing", OracleDbType.Varchar2, String.IsNullOrEmpty(model.EvaluatorPacking) ? DBNull.Value : (Object)model.EvaluatorPacking.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_tpacd", OracleDbType.Varchar2, String.IsNullOrEmpty(model.EvaluatorTPACDRD) ? DBNull.Value : (Object)model.EvaluatorTPACDRD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_tpom", OracleDbType.Varchar2, String.IsNullOrEmpty(model.EvaluatorTPOMRD) ? DBNull.Value : (Object)model.EvaluatorTPOMRD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_tpee", OracleDbType.Varchar2, String.IsNullOrEmpty(model.EvaluatorTPEERD) ? DBNull.Value : (Object)model.EvaluatorTPEERD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_status", OracleDbType.Varchar2, String.IsNullOrEmpty(model.RdEvaluationFinish) ? DBNull.Value : (Object)model.RdEvaluationFinish.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("bidding_project", OracleDbType.Varchar2, String.IsNullOrEmpty(model.BIDDING_PROJECT) ? DBNull.Value : (Object)model.BIDDING_PROJECT.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("sourcing_pool_name", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SOURCING_POOL_NAME) ? DBNull.Value : (Object)model.SOURCING_POOL_NAME.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("process_type", OracleDbType.Varchar2, String.IsNullOrEmpty(model.PROCESS_TYPE) ? DBNull.Value : (Object)model.PROCESS_TYPE.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("product_combination_name", OracleDbType.Varchar2, String.IsNullOrEmpty(model.PRODUCT_COMBINATION_NAME) ? DBNull.Value : (Object)model.PRODUCT_COMBINATION_NAME.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("product_type_name", OracleDbType.Varchar2, String.IsNullOrEmpty(model.PRODUCT_TYPE_NAME) ? DBNull.Value : (Object)model.PRODUCT_TYPE_NAME.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("process_class", OracleDbType.Varchar2, String.IsNullOrEmpty(model.PROCESS_CLASS) ? DBNull.Value : (Object)model.PROCESS_CLASS.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("array_fab", OracleDbType.Varchar2, String.IsNullOrEmpty(model.ARRAY_FAB) ? DBNull.Value : (Object)model.ARRAY_FAB.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("ji_fab", OracleDbType.Varchar2, String.IsNullOrEmpty(model.JI_FAB) ? DBNull.Value : (Object)model.JI_FAB.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("module_fab", OracleDbType.Varchar2, String.IsNullOrEmpty(model.MODULE_FAB) ? DBNull.Value : (Object)model.MODULE_FAB.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("size_inch", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SIZE) ? DBNull.Value : (Object)model.SIZE.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("surface_treatment", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SURFACE_TREATMENT) ? DBNull.Value : (Object)model.SURFACE_TREATMENT.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("sop_year", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SOP_YEAR) ? DBNull.Value : (Object)model.SOP_YEAR.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("sop_quarterly", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SOP_QUARTERLY) ? DBNull.Value : (Object)model.SOP_QUARTERLY.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("sop_month", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SOP_MONTH) ? DBNull.Value : (Object)model.SOP_MONTH.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("resolution", OracleDbType.Varchar2, String.IsNullOrEmpty(model.RESOLUTION) ? DBNull.Value : (Object)model.RESOLUTION.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("pm_lcd_type", OracleDbType.Varchar2, String.IsNullOrEmpty(model.PM_LCD_TYPE) ? DBNull.Value : (Object)model.PM_LCD_TYPE.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("pm_brightness", OracleDbType.Varchar2, String.IsNullOrEmpty(model.PM_BRIGHTNESS) ? DBNull.Value : (Object)model.PM_BRIGHTNESS.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("fcst_qty", OracleDbType.Varchar2, String.IsNullOrEmpty(model.FCST_QTY) ? DBNull.Value : (Object)model.FCST_QTY.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("sample_request", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SAMPLE_REQUEST) ? DBNull.Value : (Object)model.SAMPLE_REQUEST.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("sample_request_date", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SAMPLE_REQUEST_DATE) ? DBNull.Value : (Object)model.SAMPLE_REQUEST_DATE.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("sample_request_qty", OracleDbType.Varchar2, String.IsNullOrEmpty(model.SAMPLE_REQUEST_QTY) ? DBNull.Value : (Object)model.SAMPLE_REQUEST_QTY.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("source_from", OracleDbType.Varchar2, source_from.Trim(), ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "InsertH_Product_Info", "InsertH_Product_Info方法");

                SqlExcute.ExecuteStoredProcedure("RSI_MAIN_PKG.main_process");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Cancel_Product_Info(RSIWebServiceEntity model, string header_flag, string approve_flag)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE GPO.RSI_H_FORM_HEADER   
                                                SET FORM_STATUS = :header_flag, END_DATE = SYSDATE   
                                                WHERE RSI_NO IN (SELECT DISTINCT RSI_NO   
                                                FROM GPO.RSI_H_PRODUCT_INFO   
                                                WHERE RFQ_NO = :rfq_no   --RFQ傳入  
                                                AND RFQ_VER = :rfq_ver )   --RFQ傳入 ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("header_flag", OracleDbType.Varchar2, header_flag, ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Cancel_Product_Info", "Update Header方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE GPO.RSI_H_FORM_APPROVE
                                                SET APP_STATUS = :approve_flag, END_DATE = SYSDATE, APP_CONTENT = 'RFQ Cancel'   
                                                WHERE APP_STATUS = 'U'
                                                AND FORM_NO IN (SELECT DISTINCT FORM_NO
                                                FROM GPO.RSI_H_FORM_HEADER
                                                WHERE RSI_NO IN (SELECT DISTINCT RSI_NO
                                                FROM GPO.RSI_H_PRODUCT_INFO
                                                WHERE RFQ_NO = :rfq_no --RFQ傳入  
                                                AND RFQ_VER = :rfq_ver))  --RFQ傳入 ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("approve_flag", OracleDbType.Varchar2, approve_flag, ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Cancel_Product_Info", "Update Approve方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE GPO.RSI_H_PRODUCT_INFO
                                                SET RSI_STATUS = :header_flag  
                                                WHERE RFQ_NO = :rfq_no 
                                                AND RFQ_VER = :rfq_ver ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("header_flag", OracleDbType.Varchar2, header_flag, ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Cancel_Product_Info", "Update Product Info方法");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetRSI_NO()
        {
            string result = String.Empty;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select max(rsi_no) rsi_no from RSI_H_PRODUCT_INFO where rsi_no like :rsi_no");
                var param = new List<OracleParameter>();
                string rsi_no = String.Format("9{0}%", DateTime.Now.ToString("yyyyMMdd"));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                var dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetRSI_NO", "GetRSI_NO");

                if (String.IsNullOrEmpty(dt.Rows[0]["RSI_NO"].ToString()))
                {
                    result = String.Format("9{0}0001", DateTime.Now.ToString("yyyyMMdd"));
                }
                else
                {
                    var rsi_no_max = dt.Rows[0]["RSI_NO"].ToString();
                    var number = Convert.ToInt32(rsi_no_max.Replace(DateTime.Now.ToString("yyyyMMdd"), "")) + 1;
                    result = String.Format("9{0}{1}", DateTime.Now.ToString("yyyyMMdd"), number.ToString("0000"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static bool Check_Product_Info_Status(RSIWebServiceEntity model)
        {
            Boolean flag = false;

            try
            {
                StringBuilder sqlText = new StringBuilder();

                sqlText.Append(@"select count(*) as count
                                   from gpo.rsi_h_product_info t 
                                  where rfq_no = :rfq_no 
                                    and rfq_ver = :rfq_ver
                                    and eval_status = 'N' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER.Trim(), ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "Check_RSI_Status", "確認RSI是否有啟動過表單");

                var results = dt.AsEnumerable().Select(p => p.Field<decimal>("count")).ToList();
                int result = Convert.ToInt32(results.FirstOrDefault());

                if (result > 0)
                    flag = true;
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return flag;
            }

            return flag;
        }
        public static bool Check_Product_Info_Exists(RSIWebServiceEntity model)
        {
            Boolean flag = false;

            try
            {
                StringBuilder sqlText = new StringBuilder();

                sqlText.Append(@"select count(*) as count
                                   from gpo.rsi_h_product_info t 
                                  where rfq_no = :rfq_no 
                                    and rfq_ver = :rfq_ver ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER.Trim(), ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "Check_RSI_Exists", "確認RSI是否存在");

                var results = dt.AsEnumerable().Select(p => p.Field<decimal>("count")).ToList();
                int result = Convert.ToInt32(results.FirstOrDefault());

                if (result > 0)
                    flag = true;
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return flag;
            }

            return flag;
        }

        public static void Update_Product_Info_Status(RSIWebServiceEntity model)
        {
            try
            {
                var param = new List<OracleParameter>();
                StringBuilder sqlText = new StringBuilder();

                sqlText.Append(@"update gpo.rsi_h_product_info ");
                sqlText.Append(@"   set ");
                if (!string.IsNullOrEmpty(model.PL_RD.Trim()))
                    sqlText.Append(@"       pl_rd = :pl_rd, ");
                param.Add(new OracleParameter("pl_rd", OracleDbType.Varchar2, model.PL_RD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.ACD_RD.Trim()))
                    sqlText.Append(@"       acd_rd = :acd_rd, eval_acd = :eval_acd, ");
                param.Add(new OracleParameter("acd_rd", OracleDbType.Varchar2, model.ACD_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_acd", OracleDbType.Varchar2, model.EvaluatorACD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.EE_RD.Trim()))
                    sqlText.Append(@"       ee_rd = :ee_rd, eval_ee = :eval_ee, ");
                param.Add(new OracleParameter("ee_rd", OracleDbType.Varchar2, model.EE_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_ee", OracleDbType.Varchar2, model.EvaluatorEE.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.OM_RD.Trim()))
                    sqlText.Append(@"       om_rd = :om_rd, eval_om = :eval_om, ");
                param.Add(new OracleParameter("om_rd", OracleDbType.Varchar2, model.OM_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_om", OracleDbType.Varchar2, model.EvaluatorOM.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.PACKING_RD.Trim()))
                    sqlText.Append(@"       packing_rd = :packing_rd, eval_packing = :eval_packing, ");
                param.Add(new OracleParameter("packing_rd", OracleDbType.Varchar2, model.PACKING_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_packing", OracleDbType.Varchar2, model.EvaluatorPacking.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.TP_ACD_RD.Trim()))
                    sqlText.Append(@"       tp_acd_rd = :tp_acd_rd, eval_tpacd = :eval_tpacd, ");
                param.Add(new OracleParameter("tp_acd_rd", OracleDbType.Varchar2, model.TP_ACD_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_tpacd", OracleDbType.Varchar2, model.EvaluatorTPACDRD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.TP_EE_RD.Trim()))
                    sqlText.Append(@"       tp_ee_rd = :tp_ee_rd, eval_tpee = :eval_tpee, ");
                param.Add(new OracleParameter("tp_ee_rd", OracleDbType.Varchar2, model.TP_EE_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_tpee", OracleDbType.Varchar2, model.EvaluatorTPEERD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.TP_OM_RD.Trim()))
                    sqlText.Append(@"       tp_om_rd = :tp_om_rd, eval_tpom = :eval_tpom, ");
                param.Add(new OracleParameter("tp_om_rd", OracleDbType.Varchar2, model.TP_OM_RD.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("eval_tpom", OracleDbType.Varchar2, model.EvaluatorTPOMRD.Trim(), ParameterDirection.Input));
                sqlText.Append(@"   rsi_status = 'ContinueRSI' ");
                sqlText.Append(@" where rsi_no = (select distinct rsi_no ");
                sqlText.Append(@"                   from gpo.rsi_h_product_info ");
                sqlText.Append(@"                  where rfq_no = :rfq_no ");
                sqlText.Append(@"                    and rfq_ver = :rfq_ver ");
                sqlText.Append(@"               ) ");

                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER.Trim(), ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Product_Info_Status", "Update_Product_Info_Status方法");

                SqlExcute.ExecuteStoredProcedure("RSI_MAIN_PKG.main_process");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_Product_Info_RD_Member(RSIWebServiceEntity model)
        {
            try
            {
                var param = new List<OracleParameter>();
                StringBuilder sqlText = new StringBuilder();

                sqlText.Append(@"update gpo.rsi_h_product_info ");
                sqlText.Append(@"   set ");
                if (!string.IsNullOrEmpty(model.PL_RD.Trim()))
                    sqlText.Append(@"       pl_rd = :pl_rd, ");
                param.Add(new OracleParameter("pl_rd", OracleDbType.Varchar2, model.PL_RD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.ACD_RD.Trim()))
                    sqlText.Append(@"       acd_rd = :acd_rd, ");
                param.Add(new OracleParameter("acd_rd", OracleDbType.Varchar2, model.ACD_RD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.EE_RD.Trim()))
                    sqlText.Append(@"       ee_rd = :ee_rd, ");
                param.Add(new OracleParameter("ee_rd", OracleDbType.Varchar2, model.EE_RD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.OM_RD.Trim()))
                    sqlText.Append(@"       om_rd = :om_rd, ");
                param.Add(new OracleParameter("om_rd", OracleDbType.Varchar2, model.OM_RD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.PACKING_RD.Trim()))
                    sqlText.Append(@"       packing_rd = :packing_rd, ");
                param.Add(new OracleParameter("packing_rd", OracleDbType.Varchar2, model.PACKING_RD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.TP_ACD_RD.Trim()))
                    sqlText.Append(@"       tp_acd_rd = :tp_acd_rd, ");
                param.Add(new OracleParameter("tp_acd_rd", OracleDbType.Varchar2, model.TP_ACD_RD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.TP_EE_RD.Trim()))
                    sqlText.Append(@"       tp_ee_rd = :tp_ee_rd, ");
                param.Add(new OracleParameter("tp_ee_rd", OracleDbType.Varchar2, model.TP_EE_RD.Trim(), ParameterDirection.Input));
                if (!string.IsNullOrEmpty(model.TP_OM_RD.Trim()))
                    sqlText.Append(@"       tp_om_rd = :tp_om_rd, ");
                param.Add(new OracleParameter("tp_om_rd", OracleDbType.Varchar2, model.TP_OM_RD.Trim(), ParameterDirection.Input));
                sqlText.Append(@"   rfq_no = :rfq_no ");
                sqlText.Append(@" where rsi_no = (select distinct rsi_no ");
                sqlText.Append(@"                   from gpo.rsi_h_product_info ");
                sqlText.Append(@"                  where rfq_no = :rfq_no ");
                sqlText.Append(@"                    and rfq_ver = :rfq_ver ");
                sqlText.Append(@"               ) ");

                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER.Trim(), ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reassign_Product_Info_RD_Member", "Reassign_Product_Info_RD_Member方法-RFQ_NO:"+model.RFQ_NO.Trim()+ ",RFQ_VER:" + model.RFQ_VER.Trim());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_Rsi_H_Form_Header_RD_Member(RSIWebServiceEntity model)
        {
            try
            {
                var param = new List<OracleParameter>();
                StringBuilder sqlText = new StringBuilder();
                StringBuilder sqlText_boss = new StringBuilder();

                //更新RD 欄位
                sqlText.Append(@"update gpo.rsi_h_form_header h ");
                sqlText.Append(@"set h.rd = ");
                sqlText.Append(@"( ");
                sqlText.Append(@"  select case when h.part_type = 'ACD' then i.acd_rd ");
                sqlText.Append(@"              when h.part_type = 'EE'  then i.ee_rd ");
                sqlText.Append(@"              when h.part_type = 'OM'  then i.om_rd ");
                sqlText.Append(@"              when h.part_type = 'PACKING' then i.packing_rd ");
                sqlText.Append(@"              when h.part_type = 'TPACD'   then i.tp_acd_rd ");
                sqlText.Append(@"              when h.part_type = 'TPEE'    then i.tp_ee_rd ");
                sqlText.Append(@"              when h.part_type = 'TPOM'    then i.tp_om_rd ");
                sqlText.Append(@"         else null end ""RD"" ");
                sqlText.Append(@"  from gpo.rsi_h_product_info i ");
                sqlText.Append(@" where (1 = 1) ");
                sqlText.Append(@"   and h.rsi_no = i.rsi_no ");
                sqlText.Append(@"), ");
                sqlText.Append(@"h.pl_rd = ");
                sqlText.Append(@"( ");
                sqlText.Append(@"  select pl_rd ");
                sqlText.Append(@"  from gpo.rsi_h_product_info i ");
                sqlText.Append(@" where(1 = 1) ");
                sqlText.Append(@"   and h.rsi_no = i.rsi_no ");
                sqlText.Append(@") ");
                sqlText.Append(@"where h.rsi_no = (select distinct rsi_no ");
                sqlText.Append(@"                    from gpo.rsi_h_product_info ");
                sqlText.Append(@"                   where rfq_no = :rfq_no ");
                sqlText.Append(@"                     and rfq_ver = :rfq_ver ");
                sqlText.Append(@"                  ) ");

                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER.Trim(), ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reassign_RD_Member", "Reassign_RD_Member方法");


                //更新RD Boss欄位
                sqlText_boss.Append(@"update gpo.rsi_h_form_header fh ");
                sqlText_boss.Append(@"   set fh.rd_boss = (select e.boss_no from gfin.emp_data_all e where e.emp_no = fh.rd), ");
                sqlText_boss.Append(@"       fh.rd_manager = (select e.boss_no from gfin.emp_data_all e where e.emp_no = (select e.boss_no from gfin.emp_data_all e where e.emp_no = fh.rd)) ");
                sqlText_boss.Append(@" where fh.form_no in (select form_no ");
                sqlText_boss.Append(@"                       from gpo.rsi_h_form_header ");
                sqlText_boss.Append(@"                      where rsi_no = (select distinct rsi_no ");
                sqlText_boss.Append(@"                                        from gpo.rsi_h_product_info ");
                sqlText_boss.Append(@"                                       where rfq_no = :rfq_no ");
                sqlText_boss.Append(@"                                         and rfq_ver = :rfq_ver )) ");
                sqlText_boss.Append(@"   and fh.form_status = 'UA' ");

                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER.Trim(), ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText_boss, param.ToArray(), "Update_Rsi_H_Form_Header_RD_Member", "Update_Rsi_H_Form_Header_RD_Member方法");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_Rsi_H_Form_Approve_RD_Member(RSIWebServiceEntity model)
        {
            try
            {
                var param = new List<OracleParameter>();
                StringBuilder sqlText = new StringBuilder();

                //更新目前正在簽核的表單
                sqlText.Append(@"update gpo.rsi_h_form_approve fa ");
                sqlText.Append(@"   set fa.app_assigner = ");
                sqlText.Append(@"       (select case ");
                sqlText.Append(@"                 when (fa.phase_id = '10') then ");
                sqlText.Append(@"                  fh.rd ");
                sqlText.Append(@"                 when (fa.phase_id = '20') then ");
                sqlText.Append(@"                  fh.rd_boss ");
                sqlText.Append(@"                 when (fa.phase_id = '25') then ");
                sqlText.Append(@"                  fh.rd_manager ");
                sqlText.Append(@"                 when (fa.phase_id = '30' and fh.bu not in ('TV','PD')) then ");
                sqlText.Append(@"                  fh.pm ");
                sqlText.Append(@"                 when (fa.phase_id = '30' and fh.bu in ('TV','PD')) then ");
                sqlText.Append(@"                  fh.pl_rd ");
                //sqlText.Append(@"                 when (fa.phase_id = '30' and m.bg = 'MS') then ");
                //sqlText.Append(@"                  fh.pm ");
                //sqlText.Append(@"                 when (fa.phase_id = '30' and m.bg = 'VS') then ");
                //sqlText.Append(@"                  fh.pl_rd ");
                sqlText.Append(@"                 else ");
                sqlText.Append(@"                  null ");
                sqlText.Append(@"               end ");
                sqlText.Append(@"          from gpo.rsi_h_form_header fh ");
                sqlText.Append(@"          join gpo.c_pms_bgbu_mapping m ");
                sqlText.Append(@"            on fh.bu = m.bu ");
                sqlText.Append(@"           and m.active = 'Y' ");
                sqlText.Append(@"         where fh.form_no = fa.form_no) ");
                sqlText.Append(@" where fa.form_no in (select form_no ");
                sqlText.Append(@"                        from gpo.rsi_h_form_header ");
                sqlText.Append(@"                       where rsi_no = (select distinct rsi_no ");
                sqlText.Append(@"                                         from gpo.rsi_h_product_info ");
                sqlText.Append(@"                                        where rfq_no = :rfq_no ");
                sqlText.Append(@"                                          and rfq_ver = :rfq_ver )) ");
                sqlText.Append(@"   and fa.app_status = 'U' ");

                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, model.RFQ_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("rfq_ver", OracleDbType.Varchar2, model.RFQ_VER.Trim(), ParameterDirection.Input));


                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Rsi_H_Form_Approve_RD_Member", "Update_Rsi_H_Form_Approve_RD_Member方法");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<RSICurrentHandler> GetCurrentHandler(string bu, string project_name)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT bu,project_name,listagg(part_type || ':' || phase || '-' || assigner, '/') within group(ORDER BY part_type) AS rsi_status
                                FROM (SELECT DISTINCT a.bu,a.Project_Name,a.PART_TYPE,DECODE(a.part_type_v,NULL,'Pending on ' || t.phase || ' for other RSI',t.phase) AS phase,
                                                    e.eng_name as assigner
                                        FROM (SELECT H.bu,P.Project_Name,H.RSI_NO,H.PART_TYPE,V.PART_TYPE AS part_type_v,A.FORM_NO,
                                                    decode(A.PHASE_ID, '40', '45', A.Phase_Id) AS phase_id,
                                                    CASE
                                                        WHEN A.PHASE_ID IN ('40', '45') THEN
                                                        H.Product_Sourcer
                                                        ELSE
                                                        A.APP_ASSIGNER
                                                    END AS APP_ASSIGNER,
                                                    A.APP_STATUS
                                                FROM RSI_H_PRODUCT_INFO P
                                                JOIN RSI_H_FORM_HEADER H
                                                ON P.Rsi_No = H.RSI_NO
                                                JOIN RSI_H_FORM_APPROVE A
                                                ON H.FORM_NO = A.FORM_NO
                                                LEFT JOIN RSI_TO_DO_LIST_V v
                                                ON v.RSI_NO = p.rsi_no
                                                AND v.phase_id = a.phase_id
                                                WHERE P.RSI_STATUS = 'RSI'
                                                AND (A.FORM_NO, A.APP_SERIAL) IN
                                                    (SELECT FORM_NO, MAX(APP_SERIAL) AS APP_SERIAL
                                                        FROM GPO.RSI_H_FORM_APPROVE
                                                        WHERE APP_STATUS = 'U'
                                                        GROUP BY FORM_NO)
                                                AND A.App_Status = 'U') a
                                        JOIN gfin.emp_data_all e
                                        ON e.emp_no = a.app_assigner
                                        JOIN (select DISTINCT bu, phase_id, phase from rsi_c_flow_def) t
                                        ON t.bu = a.bu
                                        AND t.phase_id = a.phase_id)
                               where 1=1 ");

                if (!string.IsNullOrEmpty(bu))
                    sqlText.Append(@" and bu = :bu ");

                if (!string.IsNullOrEmpty(project_name))
                    sqlText.Append(@" and project_name = :project_name ");

                sqlText.Append(@"    group by bu, project_name ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.Varchar2, string.IsNullOrEmpty(bu) ? DBNull.Value : (object)bu.ToUpper(), ParameterDirection.Input));
                param.Add(new OracleParameter("project_name", OracleDbType.Varchar2, string.IsNullOrEmpty(project_name) ? DBNull.Value : (object)project_name.Trim(), ParameterDirection.Input));
                var result = SqlExcute.GetOraObjList<RSICurrentHandler>(sqlText, param.ToArray(), "RSIWebServiceDAL", "getCurrentHandler方法");
                return result.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FileListEntity> GetProductSourcerDocumnetByRFQNO(string rfq_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select f.* from RSI_H_FILELIST f 
                                 join rsi_h_product_info i
                                 on f.biz_id = to_char(i.rsi_no)
                                 and i.project_name= :rfq_no ");



                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rfq_no", OracleDbType.Varchar2, rfq_no, ParameterDirection.Input));
                var result = SqlExcute.GetOraObjList<FileListEntity>(sqlText, param.ToArray(), "RSIWebServiceDAL", "GetProductSourcerDocumnetByRFQNO");
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool InsertLog(string para1, string para2, string para3, string para4)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"INSERT INTO RSI_H_SQL_LOG(PKG_NAME, PRC_NAME, STEP_NAME, SQL_LOG, LM_TIME, LM_USER) 
                                    VALUES (:para1, :para2, :para3, :para4, sysdate, 'SYSTEM') ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("para1", OracleDbType.Varchar2, para1, ParameterDirection.Input));
                param.Add(new OracleParameter("para2", OracleDbType.Varchar2, para2, ParameterDirection.Input));
                param.Add(new OracleParameter("para3", OracleDbType.Varchar2, para3, ParameterDirection.Input));
                param.Add(new OracleParameter("para4", OracleDbType.Varchar2, para4, ParameterDirection.Input));
                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "InsertLog", String.Empty);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}