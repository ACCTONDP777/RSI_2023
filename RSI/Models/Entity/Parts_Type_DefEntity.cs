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
    public class Parts_Type_DefEntity
    {
        public string MTL_GROUP { get; set; }

        public string MTL_PARTS { get; set; }
    }

    public static class Parts_Type_DefEntityDAL {
        public static IList<Parts_Type_DefEntity> GetOriginalPart(string rsi_no, string part_type)
        {
            IList<Parts_Type_DefEntity> result = new List<Parts_Type_DefEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT P.MTL_GROUP, P.MTL_PARTS FROM GPO.RSI_C_PARTS_TYPE_DEF P
                                               LEFT JOIN (SELECT * FROM GPO.RSI_H_PRODUCT_DETAIL WHERE RSI_NO = :rsi_no AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D') AND MTL_TYPE = 'Normal') D
                                               ON P.MTL_GROUP = D.MTL_GROUP
                                               AND P.MTL_PARTS = D.MTL_PARTS
                                               WHERE P.ACTIVE = 'Y'
                                               AND P.PART_TYPE = :part_type   --Base page來
                                               ORDER BY P.MTL_GROUP, P.MTL_PARTS ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));

                result = SqlExcute.GetOraObjList<Parts_Type_DefEntity>(sqlText, param.ToArray(), "GetOriginalPart", "獲取OriginalPart資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return result;
            }
            return result;
        }
    }
}