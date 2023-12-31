﻿using RSI.Models.Entity;
using RSI.Models.Manager;
using RSI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AUO.Common.Authentication;

namespace RSI.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Approve(string comment, string rsi_no, string part_type, string phase_id, string price_group, string form_no, string tomanager, string save_sn, string bu, string projectname)
        {
            var emp_no = Employee.EmpNO;
            var part_types = part_type.Split('/');
            if (phase_id.Equals("40"))
            {
                H_Form_ApproveManager.Update_IsApproved_Y(rsi_no, emp_no, part_type);  //將已經認領的材料調整成isapproved = 'Y'(非MOH)

                var sn = string.IsNullOrEmpty(save_sn) ? new string[0] : save_sn.Split(',');
                if (sn.Any()) //Log記錄有更新的SN資訊
                {
                    foreach (var item in sn)
                    {
                        H_Form_ApproveManager.Approve_SourcerForRecordeSN(emp_no, comment, rsi_no, part_type, phase_id, item);
                    }
                }
                else //Log記錄沒有更新SN資訊
                {
                    H_Form_ApproveManager.Approve_SourcerForRecordeNoSN(emp_no, comment, rsi_no, part_type, phase_id);
                }

                H_Form_ApproveManager.Approve_Sourcer(emp_no, comment, rsi_no, part_type, phase_id);  //By Sourcer更新Approve表

                if (H_Product_DetailManager.GetNumberForCheckAllPMPrice(rsi_no, part_type) == 0)  //確認part_type所有材料都已簽核完成(不含MOH)
                {
                    foreach (var item in part_types)
                    {
                        H_Form_ApproveManager.Update_Phaseid_byPartType(rsi_no, item); //update phase_id null
                        H_Form_ApproveManager.Update_Approve_byPass(rsi_no, item, phase_id); //更新尚未簽核的狀態為Pass
                        //H_Form_ApproveManager.Approve(emp_no, comment, rsi_no, item, phase_id);  //改為By Sourcer更新Approve表
                        if (H_Form_ApproveManager.count_Approve(rsi_no, item) == 0)   //確認平行簽核所有人都簽過才能往下一關
                        {
                            H_Form_ApproveManager.insert_Approve(emp_no, comment, rsi_no, item, phase_id, tomanager);
                            H_Form_ApproveManager.Update_Assigner(rsi_no, item, phase_id, emp_no);
                        }
                    }
                }
            }
            else
            {
                if (phase_id.Equals("10"))
                {
                    if (H_Product_DetailManager.GetGroupID(rsi_no).Any())
                    {
                        H_Product_DetailManager.DeleteMOH(rsi_no, "Normal");
                        H_Product_DetailManager.InsertMOHForSpecial(rsi_no);
                    }
                    else
                    {
                        H_Product_DetailManager.DeleteMOH(rsi_no, "Special");
                        H_Product_DetailManager.InsertMOHForNormal(rsi_no);
                    }
                    H_Product_DetailManager.UpdateChooseToSpecialStatus(rsi_no); //更新被挑選至Special Parts的材料isapprove=Y,這些資料只是為了記錄,後續無人可維護
                    H_Product_DetailManager.UpdateRDDefineOwner(rsi_no, part_type); //寫入RD DEFINE材料的Owner,以便區別自己負責的材料是否已全數簽核
                }

                if (phase_id.Equals("45"))  //將已經認領的材料調整成isapproved = 'Y'(MOH)
                    H_Form_ApproveManager.Update_IsApproved_Y_MOH(rsi_no, emp_no, part_type);

                if (phase_id.Equals("45") || phase_id.Equals("50") || phase_id.Equals("60"))
                    H_Form_ApproveManager.Insert_Sourcer_Log(emp_no, comment, rsi_no, part_type, phase_id);

                if (phase_id.Equals("10") || phase_id.Equals("45"))
                    H_Form_ApproveManager.Update_mtl_group_forSpecial(rsi_no);

                foreach (var item in part_types)
                {
                    H_Form_ApproveManager.Approve(emp_no, comment, rsi_no, item, phase_id); //更新Approve表
                    if (H_Form_ApproveManager.count_Approve(rsi_no, item) == 0)  //確認平行簽核所有人都簽過才能往下一關
                    {
                        H_Form_ApproveManager.insert_Approve(emp_no, comment, rsi_no, item, phase_id, tomanager);
                        H_Form_ApproveManager.Update_Assigner(rsi_no, item, phase_id, emp_no);
                    }
                }

                if (phase_id.Equals("60") || (phase_id.Equals("50") && tomanager.Equals("N")))
                    H_Form_ApproveManager.Update_Price_Group(price_group, rsi_no, form_no);
            }

            //Material Sourcer關卡依據權限長出Assigner,但排除Sourcer關卡簽核當下
            if (!phase_id.Equals("40") && !phase_id.Equals("45"))
            {
                var dtGetSourcerPhase = RSI_ConfigEntityDAL.GetSourcerPhase(rsi_no, part_type);
                if (dtGetSourcerPhase.Rows.Count > 0)
                {
                    H_Form_ApproveManager.Update_Sourcer_Assigner(rsi_no, part_type);
                }
            }

            //ADP透過B2B拋轉到AUO進行採購報價
            if (phase_id.Equals("30"))
            {
                H_Form_ApproveManager.Insert_b2b_status(rsi_no, part_type);
            }

            SendMail("APPROVE", phase_id, string.Empty, rsi_no, part_type, bu, projectname, true);
            return Json("success");
        }

        [HttpPost]
        public ActionResult Approve_PMConfirm(string comment, string rsi_no, string phase_id, string part_type)
        {
            H_Form_ApproveManager.Approve_PMConfirm(Employee.EmpNO, comment, rsi_no, phase_id);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            RFQWebServiceForUpdateStatus.RFQWebService service = new RFQWebServiceForUpdateStatus.RFQWebService();
            var result = service.updateRSIStatus(h_Product_Info.RFQ_NO, h_Product_Info.RFQ_VER, "close");
            return Json("Success");
        }

        [HttpPost]
        public ActionResult Approve_SourcerConfirm(string comment, string rsi_no, string part_type, string phase_id)
        {
            H_Form_ApproveManager.Approve_SourcerConfirm(Employee.EmpNO, comment, rsi_no, part_type, phase_id);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            RFQWebServiceForUpdateStatus.RFQWebService service = new RFQWebServiceForUpdateStatus.RFQWebService();
            var result = service.updateRSIStatus(h_Product_Info.RFQ_NO, h_Product_Info.RFQ_VER, "close");
            return Json("Success");
        }

        [HttpPost]
        public ActionResult Reject(string comment, string rsi_no, string part_type, string phase_id, int[] snarr, bool returnStatus, string bu, string projectname)
        {
            var emp_no = Employee.EmpNO;
            if ((phase_id.Equals("40") && !returnStatus) || (phase_id.Equals("45") && !returnStatus))  //逐筆退回
            {
                H_Form_ApproveManager.Reject_Sourcer_Log(emp_no, comment, rsi_no, part_type, phase_id, snarr); //更新detail phase_id & 寫入log
            } else if (phase_id.Equals("50") || phase_id.Equals("60"))
            {
                H_Form_ApproveManager.Update_IsApproved_N_byPartType(rsi_no, part_type); //update isapproved='N'
                H_Form_ApproveManager.Reject_Sourcer_Log(emp_no, comment, rsi_no, part_type, phase_id, snarr);
            }
            var part_types = part_type.Split('/');
            
            foreach (var item in part_types)
            {
                if ((phase_id.Equals("40")) || (phase_id.Equals("45")))
                {
                    if (returnStatus) {  //整張退回
                        H_Form_ApproveManager.Update_IsApproved_N_byPartType(rsi_no, item); //update isapproved='N'
                        H_Form_ApproveManager.Reject_Sourcer(emp_no, comment, rsi_no, item, phase_id);  //更新及寫入RSI_H_FORM_APPROVE
                        H_Form_ApproveManager.Update_Approve_byPass_forRej(rsi_no, item, phase_id); //更新尚未簽核的狀態為Pass,包含40&45
                        H_Form_ApproveManager.Update_AssignerForSourcerReturn(rsi_no, item);  //update assigner                     
                    }
                }
                else
                {
                    H_Form_ApproveManager.Reject(emp_no, comment, rsi_no, item, phase_id);
                    if (phase_id.Equals("50") || phase_id.Equals("60"))
                    {
                        H_Form_ApproveManager.Update_AssignerForSourcer(rsi_no, item);
                    }
                    H_Form_ApproveManager.Update_Assigner(rsi_no, item, phase_id, emp_no);
                }
            }

            //Material Sourcer關卡依據權限長出Assigner
            if (returnStatus) //整張退回,排除Sourcer逐筆退RD的狀況
            {
                var dtGetSourcerPhase = RSI_ConfigEntityDAL.GetSourcerPhase(rsi_no, part_type);
                if (dtGetSourcerPhase.Rows.Count > 0)
                {
                    H_Form_ApproveManager.Update_Sourcer_Assigner(rsi_no, part_type);
                }
            }

            SendMail("REJECT", phase_id, comment, rsi_no, part_type, bu, projectname, returnStatus);
            return Json("success");
        }

        [HttpPost]
        public ActionResult RejectForReturn(string comment, string rsi_no, string part_type, string phase_id, string now_phase, string bu, string projectname)
        {
            rsi_no = Validate.DecryptValue(rsi_no);
            part_type = Validate.DecryptValue(part_type);
            if (String.IsNullOrEmpty(comment))
                comment = String.Format("{0} {1}-撤回修改", Employee.EmpNO, Employee.EmpName);
            try
            {
                if (now_phase.Equals("50") || now_phase.Equals("60"))
                {
                    H_Form_ApproveManager.Return_Sourcer(rsi_no, part_type, phase_id, comment, Employee.EmpNO); //清除detail phase_id & 寫入log
                }
                H_Form_ApproveManager.Reject_Return(Employee.EmpNO, comment, rsi_no, part_type, phase_id, now_phase);
                H_Form_ApproveManager.Update_AssignerForReturn(rsi_no, part_type, phase_id, Employee.EmpNO);

                //Material Sourcer關卡依據權限長出Assigner
                var dtGetSourcerPhase = RSI_ConfigEntityDAL.GetSourcerPhase(rsi_no, part_type);
                if (dtGetSourcerPhase.Rows.Count > 0)
                {
                    H_Form_ApproveManager.Update_IsApproved_N_byPartType(rsi_no, part_type); //update isapproved='N'
                    H_Form_ApproveManager.Update_Sourcer_Assigner(rsi_no, part_type);
                }
                SendMailforWithdraw(phase_id, rsi_no, part_type, bu, projectname, Employee.EmpNO);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
            return Json(true);
        }

        private void SendMail(string attribute1, string phase_id, string rejectContent, string rsi_no, string part_type, string bu, string projectname, bool returnStatus)
        {
            var emp_no = Employee.EmpNO;
            var dt_SigningNotice = RSI_ConfigEntityDAL.GetSigningNotice(bu, attribute1, phase_id); //取出待簽核關卡的Signing Notice資訊
            if (dt_SigningNotice.Rows.Count == 0)
                return;

            var sendMailCode = ConfigurationManager.AppSettings["SendMailCode"].ToString();

            #region table Content 簽核資訊
            var tableData = DASHBOARD_LIST_VManager.GetDetailApprove(rsi_no, part_type);
            string tableContent = GetTableContent(tableData);
            #endregion


            foreach (DataRow row in dt_SigningNotice.Rows)
            {
                //var attr2 = row["ATTRIBUTE2"].ToString(); //是否啟用(Y/N)
                var attr3 = row["ATTRIBUTE3"].ToString(); //啟用的關卡
                var attr4 = row["ATTRIBUTE4"].ToString(); //mailcc是否啟用(Y/N)
                var attr5 = row["ATTRIBUTE5"].ToString(); //mailcc的boss_level
                var attr6 = row["ATTRIBUTE6"].ToString(); //簽核後是否啟用要發送通知(Y/N)
                var attr7 = row["ATTRIBUTE7"].ToString(); //通知至上幾層主管
                var dt_ccmail = RSI_ConfigEntityDAL.GetCCMail(attr3, attr4, attr5, rsi_no, part_type);
                if (phase_id.Equals("40") && !returnStatus)  //Sourcer關卡且逐筆退回,要寄發給RD
                {
                    dt_ccmail = RSI_ConfigEntityDAL.GetCCMailForPhase40(attr4, attr5, rsi_no, part_type);
                }
                var mail = string.Empty;
                var ccmail = string.Empty;

                //if (!attr2.Equals("Y"))
                //    return;

                //if (!attr3.Contains(phase_id))
                //    return;

                if (dt_ccmail.Rows.Count == 0)
                    return;

                //var phaseGroup = dt_ccmail.AsEnumerable().GroupBy(p => p.Field<string>("PHASE_ID")).ToList();

                using (IDS.MailSoapClient ids = new IDS.MailSoapClient())
                {
                    var title = attribute1.Equals("APPROVE") ? "【RSI】待簽核通知 : {0}" : "【RSI】否決通知:{0}";
                    title = string.Format(title, string.Concat("[", bu, "] ", projectname, " [", part_type, "] "));
                    #region mail Body Content
                    var body = string.Empty;
                    if (attribute1.Equals("APPROVE"))
                    {
                        body += @"<html>
                                <head>
                                    <style>
                                        td, th {
                                            border: 1px solid rgb(211, 211, 211);
                                        }
                                        .tr_style{
                                            background-color: #3c8dbc !important; 
                                            color: #fff !important; 
                                        }
                                        .table_style{
                                            width:100%;
                                            border-collapse: collapse;
                                            border: 1px solid rgb(211, 211, 211);
                                        }
                                    </style>
                                </head>
                                <body>
                                    <p>Dears,</p>
                                    <p>
                                        RSI表單需要您簽核，資訊如下:<br/><br/>
                                        BU: <font color='blue'>{1}</font><br/>
                                        BU RFQ No: <font color='blue'>{2}</font><br/>
                                        PART TYPE: <font color='blue'>{3}</font><br/><br/>
                                        請進入RSI系統查詢相關訊息。謝謝！<br/>
                                        系統連結:<a href='{0}/RSI'>RSI</a>
                                    </p>
                                    <p>簽核歷程</p>
                                    {tableContent}
                                </body>
                              </html>";
                    }
                    else
                    {
                        body += @"<html>
                                <head>
                                    <style>
                                        td, th {
                                            border: 1px solid rgb(211, 211, 211);
                                        }
                                        .tr_style{
                                            background-color: #3c8dbc !important; 
                                            color: #fff !important; 
                                        }
                                        .table_style{
                                            width:100%;
                                            border-collapse: collapse;
                                            border: 1px solid rgb(211, 211, 211);
                                        }
                                    </style>
                                </head>
                                <body>
                                    <p>Dears,</p>
                                    <p>
                                         您簽核過的RSI表單已被{empInfo}否決.否決理由:<br/>
                                         {rejectContent}<br/><br/>
                                         表單資訊:<br/>
                                         BU: <font color='blue'>{1}</font><br/>
                                         BU RFQ No: <font color='blue'>{2}</font><br/>
                                         PART TYPE: <font color='blue'>{3}</font><br/><br/>
                                         請進入RSI系統查詢相關訊息。謝謝！<br/>
                                         系統連結:<a href='{0}/RSI'>RSI</a>
                                    </p>
                                    <p>簽核歷程</p>
                                    {tableContent}
                                </body>
                              </html>";
                        FormsAuthenticationTicket ticket = (HttpContext.User.Identity as FormsIdentity).Ticket;
                        FormsAuthTicketData ticketData = new FormsAuthTicketData(ticket);
                        body = body.Replace("{rejectContent}", rejectContent).Replace("{empInfo}", ticketData.DepartmentId + "/" + Employee.EmpNO + " " + Employee.EmpName);

                    }

                    body = body.Replace("{0}", "http://" + Request.Url.Host).Replace("{1}", bu).Replace("{2}", projectname).Replace("{3}", part_type);
                    #endregion
                    //#region table Content 簽核資訊
                    //var tableData = DASHBOARD_LIST_VManager.GetDetailApprove(rsi_no, part_type);
                    //var tableContent = @"<table style='width:100%;'>
                    //                    <thead>
                    //                        <tr style='background-color: #3c8dbc !important; color: #fff !important;'>
                    //                            <th>SEQ</th>
                    //                            <th>Process Name</th>
                    //                            <th>Approver</th>
                    //                            <th>Actual Approver</th>
                    //                            <th>Status</th>
                    //                            <th>Comment</th>
                    //                            <th>Begin Date</th>
                    //                            <th>End Date</th>
                    //                            <th>Working Days</th>
                    //                        </tr>
                    //                    </thead>
                    //                    <tbody>{0}</tbody>
                    //                 </table>";
                    //var tbodyContent = string.Empty;
                    //var tbodyTemplate = @"<tr>
                    //                    <td>{0}</td>
                    //                    <td>{1}</td>
                    //                    <td>{2}</td>
                    //                    <td>{3}</td>
                    //                    <td>{4}</td>
                    //                    <td>{5}</td>
                    //                    <td>{6}</td>
                    //                    <td>{7}</td>
                    //                    <td style='text-align: right;'>{8}</td>
                    //                  </tr>";
                    //foreach (var item in tableData)
                    //{
                    //    tbodyContent += string.Format(tbodyTemplate, item.APP_SERIAL, item.PHASE,
                    //                                  item.APP_ASSIGNER_NAME ?? "", item.APP_ACTOR_NAME ?? "",
                    //                                  item.ATTRIBUTE3, item.APP_CONTENT ?? "",
                    //                                  item.BEGIN_DATE != null ? ((DateTime)item.BEGIN_DATE).ToString("yyyy/MM/dd hh:mm:ss") : "",
                    //                                  item.END_DATE != null ? ((DateTime)item.END_DATE).ToString("yyyy/MM/dd hh:mm:ss") : "",
                    //                                  item.DIF_DAY != null ? ((decimal)item.DIF_DAY).ToString() : "");
                    //}
                    //tableContent = string.Format(tableContent, tbodyContent);
                    body = body.Replace("{tableContent}", tableContent);
                    //#endregion

                    var tempMailTo = dt_ccmail.AsEnumerable().Select(p => p.Field<string>("MAILTO")).ToList();

                    var tempMailCC = attr4.Equals("Y") ? dt_ccmail.AsEnumerable().Select(p => p.Field<string>("MAILCC")).ToList() : new List<string>();
                    mail = string.Join(";", tempMailTo);
                    ccmail = attr4.Equals("Y") ? string.Join(";", tempMailCC) : "";

                    ids.ManualSend_07(sendMailCode, mail, ccmail, title, body);

                    //foreach (var phase in phaseGroup)
                    //{
                    //    var tempMailTo = phase.AsEnumerable().Select(p => p.Field<string>("MAILTO")).ToList();

                    //    var tempMailCC = attr4.Equals("Y") ? phase.AsEnumerable().Select(p => p.Field<string>("MAILCC")).ToList() : new List<string>();
                    //    mail = string.Join(";", tempMailTo);
                    //    ccmail = attr4.Equals("Y") ? string.Join(";", tempMailCC) : "";

                    //    ids.ManualSend_07(sendMailCode, mail, ccmail, title, body);
                    //}
                }
            }

            var dt_Notify = RSI_ConfigEntityDAL.GetSigningNotify(attribute1, phase_id); //取出目前關卡的Signing Notice資訊
            if (dt_Notify.Rows.Count == 0)
                return;

            var is_notify = dt_Notify.Rows[0]["ATTRIBUTE6"].ToString(); //簽核後是否啟用要發送通知(Y/N)
            var notify_level = dt_Notify.Rows[0]["ATTRIBUTE7"].ToString(); //通知至上幾層主管

            if (is_notify.Equals("Y"))
            {
                using (IDS.MailSoapClient ids = new IDS.MailSoapClient())
                {
                    var dt_notifymail = RSI_ConfigEntityDAL.GetNotifyMail(is_notify, notify_level, emp_no, bu, phase_id);
                    var notifyto = dt_notifymail.Rows[0]["notifyto"].ToString();
                    var notifycc = dt_notifymail.Rows[0]["notifycc"].ToString();
                    var phase = dt_notifymail.Rows[0]["phase"].ToString();

                    var notifytitle = "【RSI】{0}已簽核通知:{1}";
                    notifytitle = string.Format(notifytitle, phase, string.Concat("[", bu, "] ", projectname, " [", part_type, "] "));
                    var notifybody = string.Empty;
                    notifybody += @"<html>
                                <head>
                                    <style>
                                        td, th {
                                            border: 1px solid rgb(211, 211, 211);
                                        }
                                        .tr_style{
                                            background-color: #3c8dbc !important; 
                                            color: #fff !important; 
                                        }
                                        .table_style{
                                            width:100%;
                                            border-collapse: collapse;
                                            border: 1px solid rgb(211, 211, 211);
                                        }
                                    </style>
                                </head>
                                <body>
                                    <p>Dears,</p>
                                    <p>
                                        RSI表單已簽核通知，資訊如下:<br/><br/>
                                        BU: <font color='blue'>{1}</font><br/>
                                        BU RFQ No: <font color='blue'>{2}</font><br/>
                                        PART TYPE: <font color='blue'>{3}</font><br/><br/>
                                        如有需求請進入RSI系統-->DashBoard查詢相關訊息。謝謝！<br/>
                                        系統連結:<a href='{0}/RSI'>RSI</a>
                                    </p>
                                    <p>簽核歷程</p>
                                    {tableContent}
                                </body>
                              </html>";
                    notifybody = notifybody.Replace("{0}", "http://" + Request.Url.Host).Replace("{1}", bu).Replace("{2}", projectname).Replace("{3}", part_type);
                    notifybody = notifybody.Replace("{tableContent}", tableContent);

                    ids.ManualSend_07(sendMailCode, notifyto, notifycc, notifytitle, notifybody);
                }
            }
        }

        private void SendMailforWithdraw(string phase_id, string rsi_no, string part_type, string bu, string projectname, string emp_no)
        {
            var sendMailCode = ConfigurationManager.AppSettings["SendMailCode"].ToString();

            var dt_ccmail = RSI_ConfigEntityDAL.GetMailWithdraw(rsi_no, part_type, bu, phase_id);
            var mailto = dt_ccmail.Rows[0]["mailto"].ToString();
            var phase = dt_ccmail.Rows[0]["phase"].ToString();

            if (dt_ccmail.Rows.Count == 0)
                return;

            using (IDS.MailSoapClient ids = new IDS.MailSoapClient())
            {
                var title = "【RSI】{0}撤回通知 : {1}";
                title = string.Format(title, phase, string.Concat("[", bu, "] ", projectname, " [", part_type, "] "));
                #region mail Body Content
                var body = string.Empty;
                body += @"<html>
                        <head>
                            <style>
                                td, th {
                                    border: 1px solid rgb(211, 211, 211);
                                }
                                .tr_style{
                                    background-color: #3c8dbc !important; 
                                    color: #fff !important; 
                                }
                                .table_style{
                                    width:100%;
                                    border-collapse: collapse;
                                    border: 1px solid rgb(211, 211, 211);
                                }
                            </style>
                        </head>
                        <body>
                            <p>Dears,</p>
                            <p>
                                RSI表單已由{empInfo}撤回，資訊如下:<br/><br/>
                                BU: <font color='blue'>{1}</font><br/>
                                BU RFQ No: <font color='blue'>{2}</font><br/>
                                PART TYPE: <font color='blue'>{3}</font><br/><br/>
                                請進入RSI系統查詢相關訊息。謝謝！<br/>
                                系統連結:<a href='{0}/RSI'>RSI</a>
                            </p>
                            <p>簽核歷程</p>
                            {tableContent}
                        </body>
                        </html>";
                #endregion
                FormsAuthenticationTicket ticket = (HttpContext.User.Identity as FormsIdentity).Ticket;
                FormsAuthTicketData ticketData = new FormsAuthTicketData(ticket);
                body = body.Replace("{empInfo}", ticketData.DepartmentId + "/" + Employee.EmpNO + " " + Employee.EmpName);
                body = body.Replace("{0}", "http://" + Request.Url.Host).Replace("{1}", bu).Replace("{2}", projectname).Replace("{3}", part_type);

                #region table Content 簽核資訊
                var tableData = DASHBOARD_LIST_VManager.GetDetailApprove(rsi_no, part_type);
                string tableContent = GetTableContent(tableData);
                #endregion

                body = body.Replace("{tableContent}", tableContent);
                //var tempMailTo = dt_ccmail.AsEnumerable().Select(p => p.Field<string>("MAILTO")).ToList();
                //mail = string.Join(";", tempMailTo);

                ids.ManualSend_07(sendMailCode, mailto, "", title, body);               
            }
        }

        private string GetTableContent(IList<DetailApprove> tableData)
        {
            var tableContent = @"<table class='table_style' font-family: 'Arial', '微軟正黑體', sans-serif;>
                                        <thead>
                                            <tr class='tr_style'>
                                                <th>SEQ</th>
                                                <th>Process Name</th>
                                                <th>Approver</th>
                                                <th>Actual Approver</th>
                                                <th>Status</th>
                                                <th>Comment</th>
                                                <th>Begin Date</th>
                                                <th>End Date</th>
                                                <th>Working Days</th>
                                            </tr>
                                        </thead>
                                        <tbody>{0}</tbody>
                                     </table>";
            var tbodyContent = string.Empty;
            var tbodyTemplate = @"<tr>
                                        <td>{0}</td>
                                        <td>{1}</td>
                                        <td>{2}</td>
                                        <td>{3}</td>
                                        <td>{4}</td>
                                        <td>{5}</td>
                                        <td>{6}</td>
                                        <td>{7}</td>
                                        <td style='text-align: right;'>{8}</td>
                                      </tr>";
            foreach (var item in tableData)
            {
                tbodyContent += string.Format(tbodyTemplate, item.APP_SERIAL, item.PHASE,
                                              item.APP_ASSIGNER_NAME ?? "", item.APP_ACTOR_NAME ?? "",
                                              item.STATUS, item.APP_CONTENT ?? "",
                                              item.BEGIN_DATE != null ? ((DateTime)item.BEGIN_DATE).ToString("yyyy/MM/dd hh:mm:ss") : "",
                                              item.END_DATE != null ? ((DateTime)item.END_DATE).ToString("yyyy/MM/dd hh:mm:ss") : "",
                                              item.DIF_DAY != null ? ((decimal)item.DIF_DAY).ToString() : "");
            }
            tableContent = string.Format(tableContent, tbodyContent);

            return tableContent;
        }
    }
}