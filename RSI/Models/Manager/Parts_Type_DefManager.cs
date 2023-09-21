using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class Parts_Type_DefManager
    {
        public static IList<Parts_Type_DefEntity> GetOriginalPart(string rsi_no, string part_type)
        {
            return Parts_Type_DefEntityDAL.GetOriginalPart(rsi_no, part_type);
        }
    }
}