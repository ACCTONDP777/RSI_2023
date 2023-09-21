using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class RSIWebServiceManager
    {
        public static void InsertH_Product_Info(RSIWebServiceEntity model)
        {
            RSIWebServiceDAL.InsertH_Product_Info(model);
        }

        public static void Cancel_Product_Info(RSIWebServiceEntity model, string header_flag, string approve_flag)
        {
            RSIWebServiceDAL.Cancel_Product_Info(model, header_flag, approve_flag);
        }

        public static bool Check_Product_Info_Status(RSIWebServiceEntity model)
        {
            return RSIWebServiceDAL.Check_Product_Info_Status(model);
        }
        public static bool Check_Product_Info_Exists(RSIWebServiceEntity model)
        {
            return RSIWebServiceDAL.Check_Product_Info_Exists(model);
        }

        public static void Update_Product_Info_Status(RSIWebServiceEntity model)
        {
            RSIWebServiceDAL.Update_Product_Info_Status(model);
        }

        public static void Reassign_RD_Member(RSIWebServiceEntity model)
        {
            //Reassign Product Info RD Member
            RSIWebServiceDAL.Update_Product_Info_RD_Member(model);
            //Update Form Header
            RSIWebServiceDAL.Update_Rsi_H_Form_Header_RD_Member(model);
            //Update Form Approve
            RSIWebServiceDAL.Update_Rsi_H_Form_Approve_RD_Member(model);
           
        }

        public static List<RSICurrentHandler> GetCurrentHandler(string bu, string project_name)
        {
            return RSIWebServiceDAL.GetCurrentHandler(bu, project_name);
        }

        public static List<FileListEntity> GetProductSourcerDocumnetByRFQNO(string rfq_no)
        {
            return RSIWebServiceDAL.GetProductSourcerDocumnetByRFQNO(rfq_no);
        }

        public static bool InsertLog(string para1, string para2, string para3, string para4)
        {
            return RSIWebServiceDAL.InsertLog(para1, para2, para3, para4);
        }
    }
}