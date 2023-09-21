using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class C_MTL_PARTSManager
    {
        public static IList<C_MTL_PARTSEntity> GetC_MTL_PARTSEntities(string rsi_no, string group_id)
        {
            return C_MTL_PARTSEntityDAL.GetC_MTL_PARTSEntities(rsi_no, group_id);
        }
    }
}