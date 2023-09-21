using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class To_Do_List_VManager
    {
        public static IList<To_Do_List_VEntity> GetTo_Do_List_VEntities(string emp_no)
        {
            return To_Do_List_VEntityDAL.GetTo_Do_List_VEntities(emp_no);
        }

        public static DataTable GetContactWindow()
        {
            return To_Do_List_VEntityDAL.GetContactWindow();
        }

        public static DataTable GetFAQ()
        {
            return To_Do_List_VEntityDAL.GetFAQ();
        }

        public static bool ToDoListUpdateTag(string rsi_no, string form_no, string phase_id, string app_serial, string app_status, string tag)
        {
            DataTable dt = To_Do_List_VEntityDAL.GetRSI_H_Todo_Note(rsi_no, form_no, phase_id, app_serial, app_status);
            if (dt.Rows.Count > 0)
            {
                //update
                return To_Do_List_VEntityDAL.UdtRSI_H_Todo_NoteByTag(rsi_no, form_no, phase_id, app_serial, app_status, tag);
            }
            else
            {
                //insert
                return To_Do_List_VEntityDAL.InsRSI_H_Todo_NoteByTag(rsi_no, form_no, phase_id, app_serial, app_status, tag);
            }
        }

        public static bool ToDoListUpdateRemark(string rsi_no, string form_no, string phase_id, string app_serial, string app_status, string remark)
        {
            DataTable dt = To_Do_List_VEntityDAL.GetRSI_H_Todo_Note(rsi_no, form_no, phase_id, app_serial, app_status);
            if (dt.Rows.Count > 0)
            {
                //update
                return To_Do_List_VEntityDAL.UdtRSI_H_Todo_NoteByRemark(rsi_no, form_no, phase_id, app_serial, app_status, remark);
            }
            else
            {
                //insert
                return To_Do_List_VEntityDAL.InsRSI_H_Todo_NoteByRemark(rsi_no, form_no, phase_id, app_serial, app_status, remark);
            }
        }
    }
}