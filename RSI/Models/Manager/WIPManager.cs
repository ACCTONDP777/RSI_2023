using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RSI.Models.Entity;


namespace RSI.Models.Manager
{
    public static class WIPManager
    {
        public static IList<WIPEntity> GetWIPInfo(string pID)
        {
            var list = WIPEntityDAL.GetWIPInfo(pID);
            return list;
        }
    }
}