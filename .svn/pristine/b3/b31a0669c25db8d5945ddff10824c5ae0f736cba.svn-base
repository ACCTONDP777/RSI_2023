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
    public class C_MTL_PARTSEntity
    {
        public string PART_TYPE { get; set; }
        public string RSI_NO { get; set; }
        public string GROUP_ID { get; set; }
        public string GROUP_DESC { get; set; }
        public string MTL_GROUP { get; set; }
        public decimal MTL_GROUP_TOTAL { get; set; }
    }
    #endregion

    public static class C_MTL_PARTSEntityDAL
    {
        public static IList<C_MTL_PARTSEntity> GetC_MTL_PARTSEntities(string rsi_no, string group_id)
        {
            IList<C_MTL_PARTSEntity> c_MTL_PARTSEntities = new List<C_MTL_PARTSEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();

                sqlText.Append(@" SELECT DISTINCT D.MTL_GROUP, SUM(NVL(D.USAGE * NVL(D.PRICE_PM, 0), 0)) AS MTL_GROUP_TOTAL 
                                  FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                  WHERE RSI_NO = :rsi_no
                                  AND (GROUP_ID IS NULL OR GROUP_ID = :group_id)
                                  AND NVL(MODIFY_TYPE,'-') <> 'D'
                                  AND NVL(D.ISCALCULATE, '-') = 'Y'
                                  AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                             OR d.parent_sn = 0 OR d.parent_sn IS NULL) 
                                  GROUP BY D.MTL_GROUP
                                  ORDER BY D.MTL_GROUP");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                c_MTL_PARTSEntities = SqlExcute.GetOraObjList<C_MTL_PARTSEntity>(sqlText, param.ToArray(), "GetUNI_SPEC_STATUS", "取得UNI_SPEC_STATUS資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return c_MTL_PARTSEntities;
            }
            return c_MTL_PARTSEntities;
        }
    }
}