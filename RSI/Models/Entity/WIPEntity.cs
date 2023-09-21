using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using RSI.Models.Utility;
using RSI.Models.Entity;
using Oracle.ManagedDataAccess.Client;

namespace RSI.Models.Entity
{
    #region DTO
    public class WIPEntity
    {
        public string WO_NAME { get; set; }
        public DateTime DATE_RELEASED { get; set; }
        public string ASSEMBLY { get; set; }
        public string BOM_VERSION { get; set; }
        public decimal START_QUANTITY { get; set; }
        public decimal QUANTITY_COMPLETED { get; set; }
        public decimal QUANTITY_SHIPPED { get; set; }

        #region 演示属性名无“_",DataTable栏位定义包含“_", 为ToList方法提供下划线处理功能
        //public string WONAME { get; set; }
        //public DateTime DATERELEASED { get; set; }
        //public string ASSEMBLY { get; set; }
        //public string BOMVERSION { get; set; }
        //public decimal STARTQUANTITY { get; set; }
        //public decimal QUANTITYCOMPLETED { get; set; }
        //public decimal QUANTITYSHIPPED { get; set; }
        #endregion
    }

    #endregion

    #region DAL
    public static class WIPEntityDAL
    {
        public static IList<WIPEntity> GetWIPInfo(string pID)
        {
            IList<WIPEntity> list = new List<WIPEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select wo.wip_entity_name wo_name,wo.date_released,wo.assembly,wo.bom_version,wo.start_quantity,wo.quantity_completed,wo.quantity_shipped
                                 from gmfg.wip_gib_version wo
                                 where wo.p_id=:p_id
                                 and wo.wip_entity_name like 'S0%'
                                 and wo.flag='Y'"
                              );

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("p_id", OracleDbType.Decimal, pID, ParameterDirection.Input));

                list = SqlExcute.GetOraObjList<WIPEntity>(sqlText, param.ToArray(), "GetWIPInfo", "获取ERP工单数据");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
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