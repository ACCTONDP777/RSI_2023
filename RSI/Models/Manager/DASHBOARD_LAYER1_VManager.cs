using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class DASHBOARD_LAYER1_VManager
    {
        public static IList<DASHBOARD_LAYER1_VEntity> GetDashBoardLayer1(string rsi_no, string phase_id, string form_stauts, string part_type)
        {
            return DASHBOARD_LAYER1_VEntityDAL.GetDashBoardLayer1(rsi_no, phase_id, form_stauts, part_type);
        }

        public static IList<DASHBOARD_LAYER1_VEntity> GetDashBoardLater1Sourcer(string rsi_no, string phase_id, string form_stauts, string part_type)
        {
            return DASHBOARD_LAYER1_VEntityDAL.GetDashBoardLater1Sourcer(rsi_no, phase_id, form_stauts, part_type);
        }

        public static bool GetViewIconShow(List<DASHBOARD_LAYER1_VEntity> model, string phase_id, string rsi_no)
        {
            return DASHBOARD_LAYER1_VEntityDAL.GetViewIconShow(model, phase_id, rsi_no);
        }

        public static DataTable GetViewDataTable(string rsi_no, string phase_id)
        {
            return DASHBOARD_LAYER1_VEntityDAL.GetViewDataTable(rsi_no, phase_id);
        }
    }
}