using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class BPS_Item_ConfigManager
    {
        public static IList<BPS_Item_ConfigEntity> GetBPS_Item_Configs()
        {
            return BPS_Item_ConfigEntityDAL.GetBPS_Item_Configs();
        }
    }
}