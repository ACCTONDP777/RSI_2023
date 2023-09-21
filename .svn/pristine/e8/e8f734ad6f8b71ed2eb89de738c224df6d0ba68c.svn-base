using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RSI.Models.Manager;
using RSI.Models.Entity;
using System.Web.Security;
using AUO.Common.Authentication;

namespace RSI.Controllers
{
    public class DemoController : Controller
    {
        /// <summary>
        /// 展示view示例
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            RSI_ConfigEntityDAL.LoginLog(Employee.EmpNO);
            var model = To_Do_List_VManager.GetTo_Do_List_VEntities(Employee.EmpNO);
            return View(model);
        }

        public ActionResult Form()
        {
            return View();
        }

        [OutputCache(CacheProfile = "CacheProfile")]
        public ActionResult Table(string id = "13")
        {
            //做Oracle DB访问捞取数据的demo演示
            var list = WIPManager.GetWIPInfo(id);
            return View(list);
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Element()
        {
            return View();
        }

        public ActionResult Notice()
        {
            return View();
        }

        public ActionResult OracleTest()
        {
            var list = BPS_Item_ConfigManager.GetBPS_Item_Configs();
            return View(list);
        }


        #region Sample页面
        // GET: Repair
        /// <summary>
        /// 提供调用webapi示例程式，及与view做简单数据传输的示例
        /// </summary>
        /// <returns></returns>
        //public ActionResult SampleView(string id)
        //{
        //try
        //{
        //    //FormsAuthTicketData ticketData = new FormsAuthTicketData((HttpContext.User.Identity as FormsIdentity).Ticket);   //获取CAP中的用户身份信息
        //    //string tempJsonDorm = WebApiHelper.GetMethod("Repair/GetDormList?site=" + ticketData.Site + "&dorm_id=");

        //    string tempJsonDorm = WebApiHelper.GetMethod("Samples/GetDormList?site=" + "AUSZ" + "&dorm_id=");
        //    DataTable dtDorm = JsonConvert.DeserializeObject<DataTable>(tempJsonDorm);
        //    ViewBag.dtDorm = dtDorm;
        //    ViewBag.dormitory_manage_info = dtDorm.Rows.Count > 0 ? string.IsNullOrEmpty(dtDorm.Rows[0]["supervisor_contact_number"].ToString()) ? "" : dtDorm.Rows[0]["supervisor_contact_number"].ToString() : "";
        //    ViewBag.hr_info = dtDorm.Rows.Count > 0 ? string.IsNullOrEmpty(dtDorm.Rows[0]["hr_supervisor_empno"].ToString()) ? "" : dtDorm.Rows[0]["hr_supervisor_empno"].ToString() : "";
        //    return View();
        //}
        //catch (Exception ex)
        //{
        //    LogHelper.GetLogger("DemoView ").Error(ex.ToString());
        //    return View("Error");
        //}
        //}
        #endregion

        [HttpPost]
        public ActionResult ToDoListUpdateTag(string rsi_no, string form_no, string phase_id, string app_serial, string app_status, string tag)
        {
            rsi_no = Validate.DecryptValue(rsi_no);
            form_no = Validate.DecryptValue(form_no);
            return Json(To_Do_List_VManager.ToDoListUpdateTag(rsi_no, form_no, phase_id, app_serial, app_status, tag));
        }

        [HttpPost]
        public ActionResult ToDoListUpdateRemark(string rsi_no, string form_no, string phase_id, string app_serial, string app_status, string remark)
        {
            rsi_no = Validate.DecryptValue(rsi_no);
            form_no = Validate.DecryptValue(form_no);
            return Json(To_Do_List_VManager.ToDoListUpdateRemark(rsi_no, form_no, phase_id, app_serial, app_status, remark));
        }
    }
}