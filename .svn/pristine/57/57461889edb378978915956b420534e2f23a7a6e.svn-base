using Oracle.ManagedDataAccess.Client;
using RSI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RSI.Models.Entity
{
    #region DTO
    public class BPS_Item_ConfigEntity
    {
        public decimal ORGANIZATION_ID { get; set; }
        public string ORGANIZATION_CODE { get; set; }
        public string ITEM_CATEGORY { get; set; }
        public decimal ITEM_ID { get; set; }
        public string ITEM_NO { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public string UOM { get; set; }
        public decimal VENDOR_ID { get; set; }
        public decimal VENDOR_SITE_ID { get; set; }
        public string VENDOR_NAME { get; set; }
        public string VENDOR_SITE_CODE { get; set; }
        public decimal LEAD_TIME { get; set; }
        public decimal SHIP_DAYS { get; set; }
        public string SHIP_AREA { get; set; }
        public string PACKING_TYPE { get; set; }
        public decimal PACKING_QTY { get; set; }
        public decimal MOQ { get; set; }
        public decimal SAFETY_STOCK { get; set; }
        public decimal SAFETY_STOCK_TO_VENDOR { get; set; }
        public decimal USAGE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string LAST_UPDATED_BY { get; set; }
        public DateTime LAST_UPDATED_DATE { get; set; }
        public string VENDOR_CODE { get; set; }
        public string ACTIVE { get; set; }
    }
    #endregion

    #region DAL
    public static class BPS_Item_ConfigEntityDAL
    {
        public static IList<BPS_Item_ConfigEntity> GetBPS_Item_Configs()
        {
            IList<BPS_Item_ConfigEntity> list = new List<BPS_Item_ConfigEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select organization_id, organization_code, item_category, item_id, item_no, item_description, 
                                               uom, vendor_id, vendor_site_id, vendor_name, vendor_site_code, lead_time, ship_days, 
                                               ship_area, packing_type, packing_qty, moq, safety_stock, safety_stock_to_vendor, usage,
                                               created_by, created_date, last_updated_by, last_updated_date, vendor_code, active 
                                               from c_bps_item_config");

                //var param = new List<OracleParameter>();
                //param.Add(new OracleParameter("p_id", OracleDbType.Decimal, pID, ParameterDirection.Input));

                list = SqlExcute.GetOraObjList<BPS_Item_ConfigEntity>(sqlText, "GetBPS_Item_Configs", "獲取Item_Configs資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return list;
            }
            return list;
        }
    }
    #endregion
}