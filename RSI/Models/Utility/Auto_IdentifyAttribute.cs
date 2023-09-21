using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RSI.Models.Utility
{
    public class Auto_IdentifyAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var rsi_no = Validate.DecryptValue(filterContext.HttpContext.Request.QueryString["rsi_no"]);
            var phase_id = filterContext.HttpContext.Request.QueryString["phase_id"];
            var emp_no = Employee.EmpNO;

            var dt = Employee.Auto_Identify(rsi_no, emp_no, phase_id);
            if(dt.Rows.Count == 0)
                filterContext.Result = new RedirectResult("/RSI");

            var dt_macrol = RSI_ConfigEntityDAL.GetMCAConrol();

            if (phase_id == "45" && dt_macrol.Rows.Count > 0)
            {
                var part_type = Validate.DecryptValue(filterContext.HttpContext.Request.QueryString["part_type"]);
                var bu = Validate.DecryptValue(filterContext.HttpContext.Request.QueryString["bu"]);
                var macaddress = filterContext.HttpContext.Request.QueryString["macaddress"];
                var dtMacAddress = Employee.CheckMacAddress(emp_no, part_type, bu, macaddress);
                if (dtMacAddress.Rows.Count > 0)
                {
                    if (dtMacAddress.Rows[0][0].ToString() == "0")
                        filterContext.Result = new RedirectResult("/RSI/RSI/NoMacAddress");
                    //寫Log紀錄
                    string logInfo = "【Rsi_no】:" + "【" + rsi_no + "】 " + "【User】:" + "【" + emp_no + "】 " + "【MAC】:" + "【" + macaddress + "】 " + "\r\n";
                    LogHelper.GetLogger("【 NoTMacAddress 】:").Info(logInfo);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}