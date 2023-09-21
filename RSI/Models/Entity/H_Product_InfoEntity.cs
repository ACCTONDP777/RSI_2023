using Oracle.ManagedDataAccess.Client;
using RSI.Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace RSI.Models.Entity
{
    #region DTO
    public class H_Product_InfoEntity
    {
        public decimal RSI_NO { get; set; }
        public string BU { get; set; }
        public string PROJECT_NAME { get; set; }
        public string RFQ_NO { get; set; }
        public string RFQ_VER { get; set; }
        public string APPLY_TYPE { get; set; }
        public string LCM { get; set; }
        public string TP { get; set; }
        public string MODEL { get; set; }
        public string REF_PRODUCT { get; set; }
        public string APPLY_REASON { get; set; }
        public string RFQ_TYPE { get; set; }
        public string BL_NIT { get; set; }
        public string POWER { get; set; }
        public string PM { get; set; }
        public string PM_NAME { get; set; }
        public string PL_RD { get; set; }
        public string PLRD_NAME { get; set; }
        public string ACD_RD { get; set; }
        public string ACDRD_NAME { get; set; }
        public string EE_RD { get; set; }
        public string EERD_NAME { get; set; }
        public string OM_RD { get; set; }
        public string OMRD_NAME { get; set; }
        public string PACKING_RD { get; set; }
        public string PACKINGRD_NAME { get; set; }
        public string TP_ACD_RD { get; set; }
        public string TP_ACDRD_NAME { get; set; }
        public string TP_EE_RD { get; set; }
        public string TP_EERD_NAME { get; set; }
        public string TP_OM_RD { get; set; }
        public string TP_OMRD_NAME { get; set; }
        public DateTime REQUEST_DATE { get; set; }
        public string PartType { get; set; }
        public string REF_PRODUCT_RFQ { get; set; }
        public string BIDDING_PROJECT { get; set; }
        public string SOURCING_POOL_NAME { get; set; }
        public string ARRAY_FAB { get; set; }
        public string JI_FAB { get; set; }
        public string MODULE_FAB { get; set; }
        public string SIZE_INCH { get; set; }
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
    }
    #endregion

    #region DAL
    public static class H_Product_InfoEntityDAL
    {
        public static IList<H_Product_InfoEntity> GetH_Product_InfoEntities(string rsi_no, string part_type)
        {
            IList<H_Product_InfoEntity> h_Product_Infos = new List<H_Product_InfoEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT 
                    RSI_NO, BU, PROJECT_NAME, RFQ_NO, RFQ_VER, APPLY_TYPE, LCM, TP, MODEL, REF_PRODUCT, APPLY_REASON, RFQ_TYPE, BL_NIT, POWER,
                    PM, (SELECT DISTINCT PM ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.PM) AS PM_NAME,
                    PL_RD, (SELECT DISTINCT PL_RD ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.PL_RD) AS PLRD_NAME,
                    ACD_RD, (SELECT DISTINCT ACD_RD ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.ACD_RD) AS ACDRD_NAME,
                    EE_RD, (SELECT DISTINCT EE_RD ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.EE_RD) AS EERD_NAME,
                    OM_RD, (SELECT DISTINCT OM_RD ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.OM_RD) AS OMRD_NAME,
                    PACKING_RD, (SELECT DISTINCT PACKING_RD ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.PACKING_RD) AS PACKINGRD_NAME,
                    TP_ACD_RD, (SELECT DISTINCT TP_ACD_RD ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.TP_ACD_RD) AS TP_ACDRD_NAME,
                    TP_EE_RD, (SELECT DISTINCT TP_EE_RD ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.TP_EE_RD) AS TP_EERD_NAME,
                    TP_OM_RD, (SELECT DISTINCT TP_OM_RD ||'-'|| EMP_NAME FROM GFIN.EMP_DATA_ALL E WHERE E.EMP_NO = P.TP_OM_RD) AS TP_OMRD_NAME,
                    REQUEST_DATE,  :part_type PartType, REF_PRODUCT_RFQ, BIDDING_PROJECT, SOURCING_POOL_NAME, 
                    ARRAY_FAB, JI_FAB, MODULE_FAB, SIZE_INCH, SURFACE_TREATMENT, SOP_YEAR, SOP_QUARTERLY, SOP_MONTH, RESOLUTION, PM_LCD_TYPE, PM_BRIGHTNESS,
                    FCST_QTY, SAMPLE_REQUEST, SAMPLE_REQUEST_DATE, SAMPLE_REQUEST_QTY
                    FROM gpo.RSI_H_PRODUCT_INFO P 
                    WHERE P.RSI_NO =:rsi_no ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                h_Product_Infos =  SqlExcute.GetOraObjList<H_Product_InfoEntity>(sqlText, param.ToArray(), "GetH_Product_InfoEntities", "取得H_Product_Info資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Infos;
            }
            return h_Product_Infos;
        }

        public static void Update_Product_Info(string rsi_no, string bl_nit, string power)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE RSI_H_PRODUCT_INFO SET 
                                                BL_NIT = :bl_nit, 
                                                POWER = :power
                                                WHERE RSI_NO = :rsi_no ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("BL_NIT", OracleDbType.Varchar2, bl_nit, ParameterDirection.Input));
                param.Add(new OracleParameter("POWER", OracleDbType.Varchar2, power, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                
               SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Product_Info", "Update_Product_Info方法");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
        }

        public static DataTable GetFCSTPage()
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select attribute1 as bu from RSI_C_PARAMETER where function='FCST_PAGE'  ");

                DataTable result = SqlExcute.GetOraDateTable(sqlText, "GetFCSTPage", "");
                return result;
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
        }

        public static DataTable GetProdInfo(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select * from RSI_H_PRODUCT_INFO where rsi_no = :rsi_no ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetProdInfo", "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion
}