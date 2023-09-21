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
    public class DASHBOARD_LAYER2_VEntity
    {
        public decimal RSI_NO { get; set; }
        public decimal FORM_NO { get; set; }
        public string PART_TYPE { get; set; }
        public string MTL_GROUP { get; set; }
        public string MTL_PARTS { get; set; }
        public string ENGLISH_NAME { get; set; }
        public string APP_ASSIGNER { get; set; }
        public string REVIEWER { get; set; }
        public DateTime BEGIN_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public string APP_STATUS { get; set; }
        public string PHASE_ID { get; set; }
        public string FORM_STATUS { get; set; }
    }
    #endregion

    #region DAL
    public static class DASHBOARD_LAYER2_VEntityDAL
    {
        public static IList<DASHBOARD_LAYER2_VEntity> GetDashBoardLayer2(string rsi_no, string app_status, string form_stauts, string part_type)
        {
            IList<DASHBOARD_LAYER2_VEntity> dashboardLayer2 = new List<DASHBOARD_LAYER2_VEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM GPO.RSI_DASHBOARD_LAYER2_V
                                                WHERE RSI_NO = :rsi_no   -- RSI_NO 需從dashboard傳過來
                                                AND FORM_STATUS = :form_status     -- FROM_STATUS 需從dashboard傳過來
                                                AND PART_TYPE = :part_type       -- PART_TYPE 需從dashboard傳過來
                                                --AND APP_STATUS = :app_status        -- APP_STATUS 需從dashboard傳過來
                                                ORDER BY MTL_GROUP, MTL_PARTS");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_status", OracleDbType.NVarchar2, form_stauts, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("app_status", OracleDbType.NVarchar2, app_status, ParameterDirection.Input));

                dashboardLayer2 = SqlExcute.GetOraObjList<DASHBOARD_LAYER2_VEntity>(sqlText, param.ToArray(), "GetDashBoardLayer2", "獲取DashBoardLayer2資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dashboardLayer2;
            }
            return dashboardLayer2;
        }
    }
    #endregion
}