using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class MterialGroupPartsManager
    {
        public static IList<MterialGroupPartsEntity> GetMterialGroupParts(string bu, string part_type)
        {
            return MterialGroupPartsDAL.GetMterialGroupParts(bu, part_type);
        }

        public static IList<string> GetUnit()
        {
            return MterialGroupPartsDAL.GetUnit();
        }
    }
}