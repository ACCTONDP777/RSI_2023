using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class DASHBOARD_LIST_VManager
    {
        public static IList<DASHBOARD_LIST_VEntity> GetDashBoardUA(string owner, string contractStatus, string project_name)
        {
            return DASHBOARD_LIST_VEntityDAL.GetDashBoardUA(owner, contractStatus, project_name);
        }

        public static IList<DASHBOARD_LIST_VEntity> GetDashBoardAP()
        {
            return DASHBOARD_LIST_VEntityDAL.GetDashBoardAP();
        }

        public static IList<ContractStatus> GetContractStatus()
        {
            return DASHBOARD_LIST_VEntityDAL.GetContractStatus();
        }

        public static IList<DetailApprove> GetDetailApprove(string rsi_no, string part_type)
        {
            return DASHBOARD_LIST_VEntityDAL.GetDetailApprove(rsi_no, part_type);
        }

        public static string GetDashBoardRSINO(string project_name, string rfq_no, string rfq_ver)
        {
            return DASHBOARD_LIST_VEntityDAL.GetDashBoardRSINO(project_name, rfq_no, rfq_ver);
        }
    }
}