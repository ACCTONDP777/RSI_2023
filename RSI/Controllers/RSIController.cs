using AUO.Common.Authentication;
using RSI.Models.Entity;
using RSI.Models.Manager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RSI.Controllers
{
    public class RSIController : Controller
    {
        // GET: RSI
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BasicPage()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        public ActionResult DashBoard()
        {
            DashBoardVM model = new DashBoardVM();

            var project_name = Request.QueryString["project_name"];
            var rfq_no = Request.QueryString["rfq_no"];
            var rfq_ver = Request.QueryString["rfq_ver"];
            if (project_name != null || rfq_no != null || rfq_ver != null) {
                project_name = project_name != null ? project_name.ToString() : string.Empty;
                rfq_no = rfq_no != null ? rfq_no.ToString() : string.Empty;
                rfq_ver = rfq_ver != null ? rfq_ver.ToString() : string.Empty;
                var rsi_no = DASHBOARD_LIST_VManager.GetDashBoardRSINO(project_name, rfq_no, rfq_ver);
                rsi_no = String.IsNullOrEmpty(rsi_no) ? " " : rsi_no;
                string owner = "2";
                string contractStatus = "ALL";
                ViewData["Owner"] = owner;
                ViewData["contractStatus"] = contractStatus;
                ViewData["projectName"] = rsi_no;
                model.UA = DASHBOARD_LIST_VManager.GetDashBoardUA(owner, contractStatus, rsi_no);
            }
            else
            {
                string owner = "1";
                string contractStatus = "UA";
                project_name = String.Empty;
                model.UA = DASHBOARD_LIST_VManager.GetDashBoardUA(owner, contractStatus, project_name);

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DashBoard(string owner,string contractStatus, string project_name)
        {
            DashBoardVM model = new DashBoardVM();
            model.UA = DASHBOARD_LIST_VManager.GetDashBoardUA(owner, contractStatus, project_name);
            ViewData["Owner"] = owner;
            ViewData["contractStatus"] = contractStatus;
            ViewData["projectName"] = project_name;


            return View(model);
        }

        [HttpPost]
        public ActionResult DashBoard_DeatilApprove(string rsi_no, string part_type)
        {
            var result = DASHBOARD_LIST_VManager.GetDetailApprove(rsi_no, part_type);
            return Json(result);
        }

        public ActionResult DetailLayer1(string rsi_no, string phase_id, string form_status, string part_type)
        {
            var model = DASHBOARD_LAYER1_VManager.GetDashBoardLayer1(rsi_no, phase_id, form_status, part_type);
            return View(model);
        }

        public ActionResult DetailLayer1SourcerView(string rsi_no, string phase_id, string form_status, string part_type)
        {
            var model = DASHBOARD_LAYER1_VManager.GetDashBoardLater1Sourcer(rsi_no, phase_id, form_status, part_type);
            return View(model);
        }

        public ActionResult DetailLayer2(string rsi_no, string app_status, string form_status, string part_type)
        {
            var model = DASHBOARD_LAYER2_VManager.GetDashBoardLayer2(rsi_no, app_status, form_status, part_type);
            return View(model);
        }

        public ActionResult Layer1Return()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        public ActionResult Layer1_RDReview()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        public ActionResult Layer1View_Sourcer()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        public ActionResult Layer1View_SourcerDocument()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        public ActionResult Layer1View_PMConfirm()
        {
            var rsi_no = Request.QueryString["rsi_no"];
            rsi_no = Validate.DecryptValue(rsi_no);
            var groups_id = H_Product_DetailManager.GetGroupID(rsi_no);
            var model = new List<C_MTL_PARTSEntity>();
            if (groups_id.Count() == 0)
            {
                var group_result = C_MTL_PARTSManager.GetC_MTL_PARTSEntities(rsi_no, "");
                group_result = group_result.Select(p => new C_MTL_PARTSEntity()
                {
                    RSI_NO = rsi_no,
                    GROUP_ID = "",
                    GROUP_DESC = "",
                    PART_TYPE = p.PART_TYPE,
                    MTL_GROUP = p.MTL_GROUP,
                    MTL_GROUP_TOTAL = p.MTL_GROUP_TOTAL
                }).ToList();
                model.AddRange(group_result);
            }
            else
            {
                foreach (var group_id in groups_id)
                {
                    var group_result = C_MTL_PARTSManager.GetC_MTL_PARTSEntities(rsi_no, group_id);
                    group_result = group_result.Select(p => new C_MTL_PARTSEntity()
                    {
                        RSI_NO = rsi_no,
                        GROUP_ID = group_id,
                        PART_TYPE = p.PART_TYPE,
                        MTL_GROUP = p.MTL_GROUP,
                        MTL_GROUP_TOTAL = p.MTL_GROUP_TOTAL
                    }).ToList();
                    model.AddRange(group_result);
                }
            }


            return View(model);
        }

        public ActionResult GetToDoListCount()
        {
            var model = To_Do_List_VManager.GetTo_Do_List_VEntities(Employee.EmpNO);
            return Content(model.Count().ToString());
        }

        public ActionResult GetEmp_EngName()
        {
            FormsAuthenticationTicket ticket = (HttpContext.User.Identity as FormsIdentity).Ticket;
            var eng_name = ticket.Name.Split('\\')[1].ToString();
            return Content(eng_name);
        }

        public ActionResult GetDepartmentId()
        {
            FormsAuthenticationTicket ticket = (HttpContext.User.Identity as FormsIdentity).Ticket;
            FormsAuthTicketData ticketData = new FormsAuthTicketData(ticket);
            var result = string.Format("{0} {1}", ticketData.DepartmentId, Employee.GetDepartmentName(Employee.EmpNO));
            return Content(result);
        }

        public ActionResult GetFile(string file_id, string rsi_no, string sn)
        {
            string biz_id = String.Format("{0}_{1}", rsi_no, sn);
            if (string.IsNullOrEmpty(sn))
                biz_id = rsi_no;
            var filelist = FileManagementManager.GetFilelist(biz_id);

            var file_name = filelist.Where(p => p.FILE_ID == file_id).Select(p => p.FILE_NAME).FirstOrDefault();
            var emp_no = filelist.Where(p => p.FILE_ID == file_id).Select(p => p.CREATED_BY).FirstOrDefault();

            byte[] fileStream = null;

            using (FLM.FLMServiceClient client = new FLM.FLMServiceClient())
            {
                client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["FLMUser"];
                //WCF用戶名，新系統申請時分配(測試區固定：flm_public)
                client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["FLMPWD"];
                //WCF密碼，新系統申請時分配(測試區固定：p@ssw0rd)
                string fileID = file_id;
                string fileName = file_name;
                string empNo = emp_no;
                bool getPdf = false;
                //PDF文件設定
                fileStream = client.GetFile(fileID, ref fileName, empNo, getPdf, null);
            }

            return File(fileStream, "application/unknown", file_name);
        }

        public ActionResult SearchEmp() {
            return View();
        }

        public ActionResult NoMacAddress()
        {
            return View();
        }

        public ActionResult Tips()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchEmp(string search, string phase_id, string part_type, string bu)
        {
            //var employees = new List<EmployeeModel>();
            //if (!String.IsNullOrEmpty(search)) {
            //    employees = Employee.GetEmpoyee(search, phase_id, part_type, bu).ToList();
            //}
            
            //return Json(employees);

            var employees = new List<EmployeeModel>();
            employees = Employee.GetEmpoyee(search, phase_id, part_type, bu).ToList();

            return Json(employees.ToList());
        }

        //[HttpPost]
        //public ActionResult ReSetAssign(string reassign, string bu, string mtl_part, string rsi_no, string part_type, string phase_id, string projectname, string sn)
        //{
        //    try
        //    {
        //        var list_sn = sn.Split(',');
        //        foreach (var item in list_sn)
        //        {
        //            H_Form_ApproveManager.Update_IsAssigner_IsApproved_N(rsi_no, part_type, item, reassign); //處理rsi_h_product_detail
        //            H_Form_ApproveManager.ReassignForRDDefineAuth(bu, reassign, mtl_part, rsi_no, part_type, phase_id, item, Employee.EmpNO); //處理authority
        //        }

        //        //if (H_Form_ApproveManager.Check_Reassign_Status(rsi_no, part_type, Employee.EmpNO))  //判斷負責的材料是否已全數簽核
        //        H_Form_ApproveManager.ReassignApprove(bu, reassign, mtl_part, rsi_no, part_type, phase_id, Employee.EmpNO); //更新Approve表
        //        return Json(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(false);
        //    }

        //}

        [HttpPost]
        public ActionResult GetSpecialPartsGroups(string rsi_no)
        {
            return Json(H_Product_DetailManager.GetGroupID(rsi_no));
        }

        [HttpPost]
        public ActionResult GetSpecialPartsGroupName(string rsi_no, string group_id)
        {
            return Json(H_Product_DetailManager.GetGroupName(rsi_no, group_id));
        }

        [HttpPost]
        public ActionResult GetSpecialPartsGroupDesc(string rsi_no, string group_id)
        {
            return Json(H_Product_DetailManager.GetGroupDesc(rsi_no, group_id));
        }

        public ActionResult SingOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Demo");
        }

        public ActionResult DeatilApprove(string rsi_no, string part_type, string phase_id)
        {
            rsi_no = Validate.DecryptValue(rsi_no);
            part_type = Validate.DecryptValue(part_type);
            var result = DASHBOARD_LIST_VManager.GetDetailApprove(rsi_no, part_type);
            ViewData["phase_id"] = phase_id;
            return View(result);
        }

        public ActionResult RFQSoucerDoc()
        {
            string url = ConfigurationManager.AppSettings["WebAPIDecrypt"];
            var uri = string.Format("{0}{1}", url, Request.QueryString.ToString());
            //var uri = "http://tw100042109.corpnet.auo.com/RFQ_NEW/API/RFQAPI/getQuerystringDecrypt?querystring=" + querystring;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(uri).Result;
            var customerJsonString = response.Content.ReadAsStringAsync();
            var para = customerJsonString.Result.Replace("\"", string.Empty).Split('&');
            var project_name = para.Where(p => p.Contains("project_name")).FirstOrDefault();
            if (!string.IsNullOrEmpty(project_name)) project_name = project_name.Replace("project_name=", string.Empty);
            var result = RSIWebServiceManager.GetProductSourcerDocumnetByRFQNO(project_name);
            return View(result);
        }
    }
}