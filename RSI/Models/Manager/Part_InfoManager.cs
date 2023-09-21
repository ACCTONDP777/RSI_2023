using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class Part_InfoManager
    {
        public static IList<Part_InfoEntity> GetPart_InfoEntities(string part_level, string english_name, string item_no, string item_desc, string maker_pn, string vendor, string remark)
        {
            return Part_InfoEntityDAL.GetPart_InfoEntities(part_level, english_name, item_no, item_desc, maker_pn, vendor, remark);
        }

        public static DataTable GetPrice_Group(string rsi_no, string part_type, string price_type)
        {
            var model = H_Product_InfoEntityDAL.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return Part_InfoEntityDAL.GetPrice_Group(model.BU, part_type, price_type);
        }

        public static DataTable GetPageNote(string phase_id)
        {
            return Part_InfoEntityDAL.GetPageNote(phase_id);
        }

        public static DataTable GetPartLevelEnglish()
        {
            return Part_InfoEntityDAL.GetPartLevelEnglish();
        }
    }
}