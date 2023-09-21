using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class H_Product_InfoManager
    {
        public static IList<H_Product_InfoEntity> GetH_Product_InfoEntities(string rsi_no, string part_type)
        {
            return H_Product_InfoEntityDAL.GetH_Product_InfoEntities(rsi_no, part_type);
        }

        public static void Update_Product_Info(string rsi_no, string bl_nit, string power)
        {
            H_Product_InfoEntityDAL.Update_Product_Info(rsi_no, bl_nit, power);
        }

        public static DataTable GetFCSTPage()
        {
            return H_Product_InfoEntityDAL.GetFCSTPage();
        }
        public static DataTable GetProdInfo(string rsi_no)
        {
            return H_Product_InfoEntityDAL.GetProdInfo(rsi_no);
        }
    }
}