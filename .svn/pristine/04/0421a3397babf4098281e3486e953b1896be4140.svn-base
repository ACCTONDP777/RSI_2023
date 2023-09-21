using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.XSSF.Util;
using NPOI.OpenXmlFormats.Spreadsheet;
using RSI.Models.Entity;
using RSI.Models.Manager;
using RSI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RSI.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class RDController : Controller
    {
        [Auto_Identify]
        public ActionResult RDReview()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        [HttpPost]
        public ActionResult RDReview_NormalParts(string rsi_no, string part_type)
        {
            return Json(H_Product_DetailManager.GetRDReviewNormalParts(rsi_no, part_type));
        }

        [HttpPost]
        public ActionResult RDReview_SpecialParts(string rsi_no, string group_id)
        {
            var model = H_Product_DetailManager.GetRDReviewSpecialParts(rsi_no);
            if (model.Any() && !String.IsNullOrEmpty(group_id))
                model = model.Where(p => p.GROUP_ID == group_id).ToList();
            return Json(model);
        }

        [HttpPost]
        public ActionResult SaveForLayer1(H_Product_DetailEntity h_Product_Details, string rsi_no)
        {
            try
            {
                if (h_Product_Details != null)
                {
                    H_Product_DetailManager.Update_Product_Detail_For_Layer1(h_Product_Details);
                }
                return Json(true);

            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult SubmitForLayer1(string rsi_no, string part_type, string bu, string projectname, string phase_id)
        {
            try
            {
                H_Product_DetailManager.Update_Product_Detail_For_Layer1_Submit(rsi_no, part_type);
                H_Product_DetailManager.InsertLogForReturnSubmit(rsi_no, part_type, Employee.EmpNO);

                var dt_SigningNotice = RSI_ConfigEntityDAL.GetSigningNotice(bu, "RESEND", phase_id);
                if (dt_SigningNotice.Rows.Count != 0)
                {
                    var sendMailCode = ConfigurationManager.AppSettings["SendMailCode"].ToString();
                    var attr2 = dt_SigningNotice.Rows[0]["ATTRIBUTE2"].ToString(); //是否啟用(Y/N)
                    var attr4 = dt_SigningNotice.Rows[0]["ATTRIBUTE4"].ToString(); //mailcc是否啟用(Y/N)
                    var attr5 = dt_SigningNotice.Rows[0]["ATTRIBUTE5"].ToString(); //mailcc的boss_level
                    var dt_mailto = RSI_ConfigEntityDAL.GetMailToApprover(rsi_no, part_type);
                    var dt_mailcc = RSI_ConfigEntityDAL.GetMailCCApprover(rsi_no, part_type);
                    var mail = string.Empty;
                    var ccmail = string.Empty;

                    if (attr2.Equals("Y"))
                    {
                        using (IDS.MailSoapClient ids = new IDS.MailSoapClient())
                        {
                            var title = "【RSI】RD重新送出通知: {0}";
                            title = string.Format(title, string.Concat(bu, "-", projectname, "-", part_type));
                            #region mail Body Content
                            var body = string.Empty;
                            body += @"<html>
                                        <body>
                                            <p>Dears,</p>
                                            <p>
                                                RSI表單已由RD修正後重新送出,需要您簽核. <br/>
                                                表單資訊:<br/>
                                                BU:{1}<br/>
                                                BU RFQ No:{2}<br/>
                                                PART TYPE:{3}<br/>
                                                請進入RSI系統查詢相關訊息。謝謝！<br/>
                                                系統超連結:<a href='{0}/RSI'>RSI</a>
                                                RSI表單需要您簽核.<br/>
                                            </p>
                                            <p>簽核資訊</p>
                                            {tableContent}
                                        </body>
                                       </html>";
                            body = body.Replace("{0}", "http://" + Request.Url.Host).Replace("{1}", bu).Replace("{2}", projectname).Replace("{3}", part_type);
                            #endregion
                            #region table Content 簽核資訊
                            var tableData = DASHBOARD_LIST_VManager.GetDetailApprove(rsi_no, part_type);
                            var tableContent = @"<table style='width:100%;'>
                                        <thead>
                                            <tr style='background-color: #3c8dbc !important; color: #fff !important;'>
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
                            body = body.Replace("{tableContent}", tableContent);
                            #endregion

                            var tempMailTo = dt_mailto.AsEnumerable().Select(p => p.Field<string>("MAILTO")).ToList();
                            var tempMailCC = attr4.Equals("Y") ? dt_mailcc.AsEnumerable().Select(p => p.Field<string>("MAILCC")).ToList() : new List<string>();
                            mail = string.Join(";", tempMailTo);
                            ccmail = attr4.Equals("Y") ? string.Join(";", tempMailCC) : "";
                            ids.ManualSend_07(sendMailCode, mail, ccmail, title, body);
                        }
                    }
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult DeleteSpecialPart(string group_id)
        {
            try
            {
                H_Product_DetailManager.UpdateForDeleteSpecialPart(group_id);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [Auto_Identify]
        public ActionResult RDBossReview()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        [HttpPost]
        public ActionResult RDBossReview_NormalParts(string rsi_no, string part_type)
        {
            return Json(H_Product_DetailManager.GetRDReviewNormalParts(rsi_no, part_type));
        }

        [HttpPost]
        public ActionResult RDBossReview_SpecialParts(string rsi_no, string group_id)
        {
            var model = H_Product_DetailManager.GetRDReviewSpecialParts(rsi_no);
            if (model.Any() && !String.IsNullOrEmpty(group_id))
                model = model.Where(p => p.GROUP_ID == group_id).ToList();
            return Json(model);
        }

        public ActionResult FileManagement(string rsi_no, string sn)
        {
            string biz_id = String.Format("{0}_{1}", rsi_no, sn);
            if (string.IsNullOrEmpty(sn))
                biz_id = rsi_no;
            var filelist = FileManagementManager.GetFilelist(biz_id);
            var result = filelist.Select(p => new
            {
                SYS_ID = p.SYS_ID,
                BIZ_ID = p.BIZ_ID,
                FILE_ID = p.FILE_ID,
                FILE_NAME = p.FILE_NAME,
                FILE_SIZE = p.FILE_SIZE,
                REMARK = p.REMARK,
                CREATED_BY = Employee.GetOtherEmpName(p.CREATED_BY),
                CREATED_DATE = p.CREATED_DATE.ToString("yyyy/MM/dd HH:mm")
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
            //return View(filelist);
        }

        [HttpPost]
        public ActionResult FileManagement(FileManagementEntity model)
        {
            try
            {
                using (FLM.FLMServiceClient client = new FLM.FLMServiceClient())
                {
                    client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["FLMUser"].ToString();
                    //WCF用戶名，新系統申請時分配(測試區固定：flm_public)
                    client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["FLMPWD"].ToString();
                    //WCF密碼，新系統申請時分配(測試區固定：p@ssw0rd)
                    string sysID = ConfigurationManager.AppSettings["FLMSYS"].ToString();   //系統ID[必選]，新系統申請時固定分配
                    BinaryReader reader = new BinaryReader(model.file.InputStream);
                    byte[] fileStream = reader.ReadBytes(model.file.ContentLength);
                    //文件流[必選]，從上傳文件的地址中獲取
                    string fileName = Path.GetFileName(model.file.FileName);       //文件名[必選]，上傳文件的全名(帶擴展名)
                    string bizID = String.Format("{0}_{1}", model.rsi_no, model.sn);       //業務ID[必選]，由AP系統生成，用於管理一組文件，可根據其獲取文件List
                    if (string.IsNullOrEmpty(model.sn))
                        bizID = model.rsi_no;
                    string securityLevel = "M";       //機密等級[可選]，不填時會取默認配置
                    string markStyle = "A";            //浮水印樣式[可選]，不填時會取默認配置
                    bool preconvertPdf = false;     //是否預轉檔PDF[可選]，不填時默認不進行預轉檔(定時轉檔任務)
                    string empNo = Employee.EmpNO;  //操作者工號[必選]
                    string remark = model.remark;            //文件備註[可選]
                                                             //呼叫WCF，返回FileID
                    string fileID = client.UploadFile(sysID, fileStream, fileName, bizID,
                                          securityLevel, markStyle, preconvertPdf, empNo, remark);

                    FileManagementManager.SetFilelist(sysID, bizID, fileID, fileName, (model.file.ContentLength / 1024).ToString(), model.remark, Employee.EmpNO);
                    H_Product_DetailManager.UpdateDetailByFileManagement(model.rsi_no, model.sn, Employee.EmpNO);
                }
            }
            catch (Exception ex)
            {
                //return View("~/Views/Demo/CustomError.cshtml");
                return Json(false);
            }
            return Json(true);
            //return RedirectToAction("FileManagement", new { rsi_no = model.rsi_no, sn = model.sn, phase_id = model.phase_id });
        }

        [HttpPost]
        public ActionResult DeleteFile(string file_id)
        {
            using (FLM.FLMServiceClient client = new FLM.FLMServiceClient())
            {
                client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["FLMUser"].ToString();
                //WCF用戶名，新系統申請時分配(測試區固定：flm_public)
                client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["FLMPWD"].ToString();
                //WCF密碼，新系統申請時分配(測試區固定：p@ssw0rd)
                bool result = client.DeleteFile(file_id, Employee.EmpNO);

                FileManagementManager.UpdateFileActive(file_id);
            }
            return Json(true);
            //return RedirectToAction("FileManagement", new { rsi_no = rsi_no, sn = sn, phase_id = phase_id });
        }

        public ActionResult PartNoSearch()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PartNoSearch(string part_level, string english_name, string item_no, string item_desc, string maker_pn, string vendor, string remark)
        {
            try
            {
                //var part_Infos = Part_InfoManager.GetPart_InfoEntities(part_level, english_name, item_no, item_desc, maker_pn, vendor, remark);
                RFQWebService.WS_PartBOM service = new RFQWebService.WS_PartBOM();
                var RFQresult = service.getPartNoForRSI(part_level, item_no, english_name, vendor, item_desc, maker_pn, remark);
                DataTable dt = XMLHelper.GetDataSetFromXML(RFQresult);
                IList<Part_InfoEntity> part_Infos = new List<Part_InfoEntity>();

                foreach (DataRow row in dt.Rows)
                {
                    Part_InfoEntity part_info = new Part_InfoEntity();
                    part_info.ITEM_NO = row["part_no"].ToString();
                    part_info.ENGLIST_NAME = row["english_name"].ToString();
                    part_info.ITEM_DESC = row["part_description"].ToString();
                    part_info.VENDOR = row["makerSource"].ToString();
                    part_info.Maker_PN = row["makerPN"].ToString();
                    part_info.RELEASE_DATE = Convert.ToDateTime(row["RELEASE_DATE"].ToString());
                    part_Infos.Add(part_info);
                }

                var result = part_Infos.Select(p => new
                {
                    ITEM_NO = p.ITEM_NO,
                    ENGLIST_NAME = p.ENGLIST_NAME,
                    ITEM_DESC = p.ITEM_DESC,
                    VENDOR = p.VENDOR,
                    PART_LEVEL = p.PART_LEVEL,
                    Maker_PN = p.Maker_PN,
                    RELEASE_DATE = p.RELEASE_DATE.ToString("yyyy/MM/dd HH:mm")
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                IList<Part_InfoEntity> part_Infos = new List<Part_InfoEntity>();
                return Json(part_Infos);
            }
        }

        [HttpPost]
        public ActionResult getEOLandUNI_SPEC_STATUS(string item_no)
        {
            Dictionary<string, string> EOLandUniSpecStauts = new Dictionary<string, string>();
            EOLandUniSpecStauts.Add("EOL", H_Product_DetailManager.GetEOL(item_no).FirstOrDefault());
            EOLandUniSpecStauts.Add("UNI_SPEC_STATUS", H_Product_DetailManager.GetUNI_SPEC_STATUS(item_no).FirstOrDefault());
            return Json(EOLandUniSpecStauts);
        }

        [HttpPost]
        public ActionResult GetSpecialNameRepeat(string rsi_no, string group_name)
        {
            var result = H_Product_DetailManager.GetSpecialNameRepeat(rsi_no, group_name);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetOriginalPart(string rsi_no, string part_type)
        {
            var result = Parts_Type_DefManager.GetOriginalPart(rsi_no, part_type);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetMterialGroupParts(string bu, string part_type)
        {
            return Json(MterialGroupPartsManager.GetMterialGroupParts(bu, part_type));
        }

        [HttpPost]
        public ActionResult GetUnit()
        {
            return Json(MterialGroupPartsManager.GetUnit());
        }

        [HttpPost]
        public ActionResult RDReview_Save(H_Product_DetailEntity model, List<string> resultSN, string parent_no, string group_id)
        {
            bool result = false;
            model.UNI_SPEC_STATUS = H_Product_DetailManager.GetUniSpecStatus(model.PART_NO);
            model.EOL_STATUS = H_Product_DetailManager.GetEolStatus(model.PART_NO);

            if (model.MTL_TYPE.Equals("Special"))
            {
                model.GROUP_NAME = H_Product_DetailManager.GetGroupName(model.RSI_NO.ToString(), model.GROUP_ID);
                model.GROUP_DESC = H_Product_DetailManager.GetGroupDesc(model.RSI_NO.ToString(), model.GROUP_ID);
            }

            if (model.MODIFY_TYPE.Equals("A"))
            {
                model.SN = H_Product_DetailManager.GetSNByProductDetail();
                result = H_Product_DetailManager.Create_Product_Detail(model);

                if (model.MTL_TYPE.Equals("Normal"))
                    H_Product_DetailManager.UpdateParentPartNo(model.RSI_NO.ToString(), resultSN, parent_no);

                if (!string.IsNullOrEmpty(model.PART_NO_MFG))
                {
                    string partnumber_top = model.PART_NO_MFG.Split('-')[0];
                    string partnumber_top_mfg = model.PART_NO_MFG.Split('-')[1];

                    H_Product_DetailManager.Gen_ref_bom_data(partnumber_top, partnumber_top_mfg, model.PART_NO, Employee.EmpNO);
                    var other_dt = H_Product_DetailManager.GetNewOtherTableData(model.RSI_NO.ToString(), Employee.EmpNO);
                    foreach (var item in other_dt)
                    {
                        item.SN = H_Product_DetailManager.GetSNByProductDetail();
                        result = H_Product_DetailManager.Create_Product_Detail(item);
                    }

                    H_Product_DetailManager.UpdateParentSNForChange(model.RSI_NO.ToString());
                }
            }

            if (model.MODIFY_TYPE.Equals("U"))
            {
                result = H_Product_DetailManager.Delete_Product_Detail(model, group_id);
                result = H_Product_DetailManager.Create_Product_Detail(model);

                //string old_sn = model.SN.ToString();
                //string new_sn = H_Product_DetailManager.GetProductDetailRSINOSN(model.RSI_NO.ToString(), model.PART_NO);
                //if(!String.IsNullOrEmpty(new_sn))
                //    H_Product_DetailManager.UpdateParentSNbyBOM(model.RSI_NO.ToString(), old_sn, new_sn);
                if (model.MTL_TYPE.Equals("Normal"))
                    H_Product_DetailManager.UpdateParentPartNo(model.RSI_NO.ToString(), resultSN, parent_no);
            }

            //if (model.MODIFY_TYPE.Equals("NEW"))
            //{
            //    if (model.SN == 0)
            //    {
            //        model.SN = H_Product_DetailManager.GetSNByProductDetail();
            //    }
            //    else
            //    {
            //        result = H_Product_DetailManager.Delete_Product_Detail(model, group_id);
            //    }

            //    result = H_Product_DetailManager.Create_Product_Detail(model);

            //    if (model.MTL_TYPE.Equals("Normal"))
            //        H_Product_DetailManager.UpdateParentPartNo(model.RSI_NO.ToString(), resultSN, parent_no);
            //}

            if (model.MODIFY_TYPE.Equals("D"))
            {
                if (model.MTL_TYPE.Equals("Normal"))
                {
                    var levelModel = H_Product_DetailManager.GetRDReviewNormalpartsBySN(model.RSI_NO.ToString(), model.SN.ToString());
                    foreach (var item in levelModel)
                        result = H_Product_DetailManager.Delete_Product_Detail(item, group_id);

                    H_Product_DetailManager.UpdateParentPartNo(model.RSI_NO.ToString(), resultSN, parent_no);
                }

                if (model.MTL_TYPE.Equals("Special"))
                {
                    result = H_Product_DetailManager.Delete_Product_Detail(model, group_id);
                    H_Product_DetailManager.RDReviewNormalModifyTypeSToEmptyWithSpecialDelete(model.RSI_NO.ToString(), model.PART_NO, model.PARTNUMBER_PARENT);
                }
            }


            return Json(result);
        }

        [HttpPost]
        public ActionResult RDReview_CheckBLUCELL(string rsi_no, string part_type, string bl_nit, string power)
        {
            H_Product_InfoManager.Update_Product_Info(rsi_no, bl_nit, power);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            if (part_type.Equals("OM"))
            {
                if (!String.IsNullOrEmpty(h_Product_Info.REF_PRODUCT))
                {
                    var ref_product = h_Product_Info.REF_PRODUCT.Split('.')[0];
                    if (!ref_product.Contains("91") && String.IsNullOrEmpty(h_Product_Info.BL_NIT))
                        return Json(false);
                }
            }
            if (part_type.Equals("ACD") && String.IsNullOrEmpty(h_Product_Info.POWER))
                return Json(false);
            return Json(true);
        }

        [HttpPost]
        public ActionResult RDReview_CheckUpdate(string rsi_no)
        {
            return Json(H_Form_ApproveManager.RDCheckUpdate(rsi_no, Employee.EmpNO));
        }

        public ActionResult RDReview_Change()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RDReview_Change(string rsi_no, string partno, string partlevel, string plant)
        {
            if (H_Product_DetailManager.RDReviewChangeCallPackage(partno, partlevel, plant))
                return Json(H_Product_DetailManager.GetRDReviewChangeOtherTableData(rsi_no, partno));
            return Json(new List<H_Product_DetailEntity>());
        }

        [HttpPost]
        public ActionResult RDReview_ChangeData(IList<H_Product_DetailEntity> changeData, IList<H_Product_DetailEntity> otherData)
        {
            var result = false;
            foreach (var item in changeData)
            {
                result = H_Product_DetailManager.Delete_Product_Detail(item, string.Empty);
            }

            foreach (var item in otherData)
            {
                item.SN = H_Product_DetailManager.GetSNByProductDetail();
                result = H_Product_DetailManager.Create_Product_Detail(item);
            }

            H_Product_DetailManager.UpdateParentSNForChange(otherData.FirstOrDefault().RSI_NO.ToString());

            return Json(result);
        }

        [HttpPost]
        public ActionResult RDReview_AutoComplatePartNo(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Json(string.Empty);

            var dt = ModelInfoManager.GetPartNo(query.ToUpper());
            var result = dt.AsEnumerable().Select(p => new
            {
                part_no = p.Field<string>("PART_NO"),
                part_desc = p.Field<string>("PART_DESCRIPTION"),
                english_name = p.Field<string>("ENGLISH_NAME")
            }).ToList();
            return Json(result);
        }

        [HttpPost]
        public ActionResult RDReviewChange_AutoComplateQueryConditions(string query)
        {
            var dt = ModelInfoManager.GetQueryConditions(query);
            var result = dt.AsEnumerable().Select(p => new
            {
                rsi_no = p.Field<Decimal>("RSI_NO").ToString(),
                query_result = p.Field<string>("QUERYRESULT").Trim()
            }).ToList();
            return Json(result);
        }

        [HttpPost]
        public ActionResult RDReviewChange_AutoComplatePartNo(string query)
        {
            var result = ModelInfoManager.GetModelInfo(query.ToUpper());
            return Json(result.Select(p => p.PART_NO).Distinct().ToList());
        }

        [HttpPost]
        public ActionResult RDReview_AutoComplatePlant(string query)
        {
            var result = ModelInfoManager.GetModelInfo(query);
            return Json(result.Select(p => p.PART_MFG).ToList());
        }

        public ActionResult RDReview_SpecialTransaction(string rsi_no)
        {
            return View();
        }

        [HttpPost]
        public ActionResult RDReview_SpecialTransaction(IList<H_Product_DetailEntity> specialData, string rsi_no, string group_id)
        {
            try
            {
                var group_name = H_Product_DetailManager.GetGroupName(rsi_no, group_id);
                var group_desc = H_Product_DetailManager.GetGroupDesc(rsi_no, group_id);

                H_Product_DetailManager.RDReviewSpecialModefyTypeD(rsi_no, group_id);
                if (String.IsNullOrEmpty(group_id))
                {
                    DateTime GetNowDateTimeDetail = new DateTime(0001, 01, 01, 01, 01, 01, 01);
                    GetNowDateTimeDetail = DateTime.Now;
                    group_id = GetNowDateTimeDetail.ToString("yyyyMMddhhmmssffffff");
                }

                foreach (var item in specialData)
                {
                    item.MTL_TYPE = "Special";
                    item.GROUP_ID = group_id;
                    item.GROUP_NAME = group_name;
                    item.GROUP_DESC = group_desc;
                    item.MODIFY_TYPE = "A";
                    item.SN = H_Product_DetailManager.GetSNByProductDetail();
                    H_Product_DetailManager.Create_Product_Detail(item);
                }

                H_Product_DetailManager.RDReviewNormalModifyTypeSToEmpty(rsi_no);
                H_Product_DetailManager.RDReviewNormalModifyTypeEmptyToS(rsi_no, group_id);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult RDReview_GetMaterialGroupMaterialParts(string rsi_no, string part_level, string english_name, string parent_part_level, string sn)
        {
            var result = H_Product_DetailManager.GetMaterialGroupMaterialParts(rsi_no, part_level, english_name, parent_part_level, sn);
            var resultObject = result.AsEnumerable().Select(p => new
            {
                MTL_GROUP = p.Field<string>("MTL_GROUP"),
                MTL_PARTS = p.Field<string>("MTL_PARTS"),
                PART_TYPE = p.Field<string>("PART_TYPE"),
                PARTS_GROUP = p.Field<string>("PARTS_GROUP"),
                SPEC_DEF = p.Field<string>("SPEC_DEF")
            }).ToList();
            return Json(resultObject);
        }

        [HttpPost]
        public ActionResult RDReview_SetSpecialName(string rsi_no, string group_id, string group_name)
        {
            return Json(H_Product_DetailManager.UpdateSpecialPartsName(rsi_no, group_id, group_name));
        }

        [HttpPost]
        public ActionResult RDReview_SetSpecialDesc(string rsi_no, string group_id, string group_desc)
        {
            return Json(H_Product_DetailManager.UpdateSpecialPartsDesc(rsi_no, group_id, group_desc));
        }

        [HttpPost]
        public ActionResult RDReview_CreateSpecialParts(string rsi_no, IList<H_Product_DetailEntity> sendData, string group_name)
        {
            DateTime GetNowDateTimeDetail = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            GetNowDateTimeDetail = DateTime.Now;
            var group_id = GetNowDateTimeDetail.ToString("yyyyMMddhhmmssffffff");

            foreach (var item in sendData)
            {
                item.MTL_TYPE = "Special";
                item.GROUP_ID = group_id;
                item.GROUP_NAME = String.Format("{0}-複製", group_name);
                item.GROUP_DESC = String.Empty;
                item.MODIFY_TYPE = "A";
                //item.SN = H_Product_DetailManager.GetSNByProductDetail();
                H_Product_DetailManager.Create_Product_Detail(item);
            }
            return Json(group_id);
        }

        [HttpPost]
        public ActionResult RDReview_DeleteSpecialParts(string rsi_no, string group_id)
        {
            H_Product_DetailManager.RDReviewSpecialModefyTypeD(rsi_no, group_id);
            if (!H_Product_DetailManager.GetGroupID(rsi_no).Any())
                H_Product_DetailManager.RDReviewNormalModifyTypeSToEmpty(rsi_no);
            return Json(group_id);
        }

        [HttpPost]
        public ActionResult PartNoSearch_PartLevel(string query)
        {
            var dt = Part_InfoManager.GetPartLevelEnglish();
            var result = dt.AsEnumerable().Select(p => p.Field<string>("PART_LEVEL"))
                .Distinct().OrderBy(p => p).ToList();
            if (!String.IsNullOrEmpty(query))
                result = result.Where(p => p.Contains(query)).ToList();
            return Json(result);
        }

        [HttpPost]
        public ActionResult PartNoSeach_EnglishName(string part_level)
        {
            var dt = Part_InfoManager.GetPartLevelEnglish();
            var result = dt.AsEnumerable().Where(p => p.Field<string>("PART_LEVEL") == part_level)
                .Select(p => p.Field<string>("ENGLISH_NAME"))
                .Distinct().OrderBy(p => p).ToList();
            return Json(result);
        }

        [HttpPost]
        public ActionResult PartNoSeach_MaterialParts(string part_level)
        {
            var dt = Part_InfoManager.GetPartLevelEnglish();
            var result = dt.AsEnumerable().Where(p => p.Field<string>("PART_LEVEL") == part_level)
                .Select(p => p.Field<string>("MTL_PARTS"))
                .Distinct().FirstOrDefault();
            return Json(result);
        }

        [HttpPost]
        public ActionResult PartNoSearch_Material(string part_level, string english_name)
        {
            var dt = Part_InfoManager.GetPartLevelEnglish();
            var result = dt.AsEnumerable()
                .Where(p => p.Field<string>("PART_LEVEL") == part_level && p.Field<string>("ENGLISH_NAME") == english_name)
                .Select(p => new
                {
                    material_group = p.Field<string>("MTL_GROUP"),
                    material_parts = p.Field<string>("MTL_PARTS")
                })
                .FirstOrDefault();
            return Json(result);
        }

        [HttpPost]
        public ActionResult ExportExcel(string rsi_no, string projectname, string bu, string part_type, string phase_id, string form_no, string page_name)
        {
            var bom = H_Product_DetailManager.GetRDReviewNormalParts(rsi_no, part_type);
            var groupIdList = H_Product_DetailManager.GetGroupID(rsi_no);
            Dictionary<string, IList<H_Product_DetailEntity>> specialData = new Dictionary<string, IList<H_Product_DetailEntity>>();
            foreach (var group_id in groupIdList)
            {
                var data = H_Product_DetailManager.GetProductDetailForExportExcelSpecialData(rsi_no, group_id, Employee.EmpNO);
                specialData.Add(group_id, data);
            }

            string fileName = String.Format("{0}_{1}.xlsx", projectname, DateTime.Now.ToString("yyyyMMdd"));
            var exportexcelconfig = RSI_ConfigEntityDAL.GetExportExcelConfig(bu, part_type, phase_id);
            var fileStream = this.ExchangeExcel(rsi_no, bom, specialData);

            var dt_rmscrol = RSI_ConfigEntityDAL.GetRMSConrol();
            if (dt_rmscrol.Rows.Count > 0)
            {
                using (FLM.FLMServiceClient client = new FLM.FLMServiceClient())
                {
                    client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["FLMUser"];
                    client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["FLMPWD"];
                    //client.Url = ConfigurationManager.AppSettings["FLMService"];
                    string sysId = ConfigurationManager.AppSettings["FLMSYS"];  //系統編號[必選]，FLM註冊或本系統名稱

                    //相應的權限（V：查看、P：打印、E：編輯、C：複製、S：保存）
                    var flmidentity = exportexcelconfig.Rows[0]["attribute7"] != null ? exportexcelconfig.Rows[0]["attribute7"].ToString() : String.Empty;
                    var flmvaliddays = exportexcelconfig.Rows[0]["attribute8"] != null ? exportexcelconfig.Rows[0]["attribute8"].ToString() : String.Empty;
                    var attribute9 = exportexcelconfig.Rows[0]["attribute9"].ToString();
                    var userAndRightsList = new List<string[]>();
                    userAndRightsList.Add(new string[2] { Employee.EmpNO, flmidentity });

                    if (!string.IsNullOrEmpty(attribute9))
                    {
                        var boss = RSI_ConfigEntityDAL.GetRMSUser(Employee.EmpNO, attribute9);
                        if (boss.Rows.Count > 0)
                        {
                            foreach (DataRow row in boss.Rows)
                            {
                                var boss_emp_no = row["BOSS_EMP_NO"].ToString();
                                userAndRightsList.Add(new string[2] { boss_emp_no, flmidentity });
                            }
                        }
                    }


                    int validDays = Convert.ToInt32(flmvaliddays);  //加密有效天數[必選]，從當日起計算有效天數
                    bool offlineRead = true;
                    byte[] encryptFileStream = client.EncryptFile(sysId, fileStream, fileName, userAndRightsList.ToArray(), null, validDays, offlineRead);

                    RSI_ConfigEntityDAL.ExportLog("Export", page_name, rsi_no, part_type, Validate.DecryptValue(form_no), flmidentity, String.Join(",", userAndRightsList.Select(p => p[0]).ToArray()));
                    return File(encryptFileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName); //檔案經過加密
                }
            }
            else
            {
                return File(fileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        [HttpPost]
        public ActionResult DownloadExcel(string rsi_no, string part_type, string project_name)
        {
            DataTable dt_sheet1Data = H_Product_DetailManager.GetSheet1Data(rsi_no, part_type);
            DataTable dt_sheet2Data = H_Product_DetailManager.GetSheet2Data(rsi_no, part_type);
            DataTable dt_sheet3Data = H_Product_DetailManager.GetSheet3Data();
            string path = Server.MapPath("~/File/RDExcel.xlsx");
            IWorkbook workbook = new XSSFWorkbook(path);
            string filename = String.Format("{0}_{1}_{2}", project_name, part_type, DateTime.Now.ToString("yyyyMMDDHHmmss"));
            string sheet1name = "RSI_SPEC_Upload";
            string sheet2name = String.Format("Template_{0}", part_type);
            string sheet3name = "ENGLISH_NAME List";
            string sheet4name = "Source";

            #region Sheet1 Data
            workbook.SetSheetName(0, sheet1name);
            XSSFSheet sheet = (XSSFSheet)workbook.GetSheetAt(0);

            #region Sheet1 header Data
            IRow row = sheet.CreateRow(0);
            XSSFCellStyle cellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
            IFont fontstyle = workbook.CreateFont();
            fontstyle.Color = IndexedColors.Red.Index;
            cellstyle.SetFont(fontstyle);
            cellstyle.Alignment = HorizontalAlignment.Center;
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 2, 5));
            row.CreateCell(0).SetCellValue("必填");
            row.GetCell(0).CellStyle = cellstyle;
            row.CreateCell(1).SetCellValue("必填");
            row.GetCell(1).CellStyle = cellstyle;
            row.CreateCell(2).SetCellValue("PART_NO或PART_LEVEL+ENGLISH_NAME+PART_SPEC擇一必填");
            row.GetCell(2).CellStyle = cellstyle;
            row.CreateCell(6).SetCellValue("必填");
            row.GetCell(6).CellStyle = cellstyle;
            row.CreateCell(7).SetCellValue("必填");
            row.GetCell(7).CellStyle = cellstyle;

            row = sheet.CreateRow(1);
            cellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
            fontstyle = workbook.CreateFont();
            fontstyle.Color = IndexedColors.White.Index;
            fontstyle.Boldweight = (short)FontBoldWeight.Bold;
            fontstyle.FontHeightInPoints = 10;
            fontstyle.FontName = "Verdana";
            cellstyle.SetFont(fontstyle);
            var color = new XSSFColor(new byte[] { 75, 172, 198 });
            cellstyle.FillForegroundColorColor = color;
            cellstyle.FillPattern = FillPattern.SolidForeground;
            cellstyle.IsLocked = true;

            for (int i = 0; i < dt_sheet1Data.Columns.Count; i++)
            {
                DataColumn dt_col = dt_sheet1Data.Columns[i];
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt_col.ColumnName);
                cell.CellStyle = cellstyle;
            }
            #endregion

            #region Sheet1 Detail Data
            fontstyle = workbook.CreateFont();
            fontstyle.FontHeightInPoints = 10;
            fontstyle.FontName = "Verdana";
            
            XSSFCellStyle unlockcellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
            unlockcellstyle.SetFont(fontstyle);
            color = new XSSFColor(new byte[] { 255, 255, 204 });
            unlockcellstyle.FillForegroundColorColor = color;
            unlockcellstyle.FillPattern = FillPattern.SolidForeground;
            unlockcellstyle.IsLocked = false;

            XSSFCellStyle lockcellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
            lockcellstyle.SetFont(fontstyle);
            lockcellstyle.IsLocked = true;
            //XSSFDataFormat format = (XSSFDataFormat)workbook.CreateDataFormat();

            for (int i = 0; i < dt_sheet1Data.Rows.Count; i++)
            {
                DataRow dt_row = dt_sheet1Data.Rows[i];
                string lockkey = dt_row[dt_sheet1Data.Columns.Count - 1].ToString();
                IRow detail_row = sheet.CreateRow(i + 2);
                for (int j = 0; j < dt_sheet1Data.Columns.Count; j++)
                {
                    ICell cell = detail_row.CreateCell(j);
                    cell.SetCellValue(dt_row[j].ToString());                   
                    if (dt_sheet1Data.Columns[j].ColumnName.Equals("USAGE") && !String.IsNullOrEmpty(dt_row[j].ToString()))
                        cell.SetCellValue(Convert.ToDouble(dt_row[j].ToString()));

                    if (lockkey.Equals("Y"))
                    {
                        cell.CellStyle = unlockcellstyle;
                        //設定Part_Level欄位格式
                        if (dt_sheet1Data.Columns[j].ColumnName.Equals("PARENT_LEVEL") || dt_sheet1Data.Columns[j].ColumnName.Equals("PART_LEVEL"))
                        {
                            IDataFormat dataFormat = workbook.CreateDataFormat();
                            XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                            IFont sFont = workbook.CreateFont();
                            cellStyle.DataFormat = dataFormat.GetFormat("@");
                            sFont.FontHeightInPoints = 10;
                            sFont.FontName = "Verdana";                          
                            cellStyle.SetFont(sFont);                           
                            color = new XSSFColor(new byte[] { 255, 255, 204 });
                            cellStyle.FillForegroundXSSFColor = color;
                            cellStyle.FillPattern = FillPattern.SolidForeground;
                            cellStyle.IsLocked = false;
                            detail_row.GetCell(j).CellStyle = cellStyle;
                        }
                    }            
                    else
                        cell.CellStyle = lockcellstyle;
                }
            }
            #endregion

            for (int i = 0; i < (int)row.LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
                if (i == 0 || i == 2 || i == 3)
                {
                    XSSFCellStyle stringlstyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    stringlstyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                    stringlstyle.IsLocked = false;
                    sheet.SetDefaultColumnStyle(i, stringlstyle);
                }
                
            }
            sheet.CreateFreezePane(0, 2);
            sheet.UnlockDeleteRows();
            sheet.UnlockInsertRows();
            sheet.EnableLocking();
            sheet.SetColumnHidden(row.LastCellNum - 1, true);
            #endregion

            #region Sheet2 Data
            workbook.SetSheetName(1, sheet2name);
            sheet = (XSSFSheet)workbook.GetSheetAt(1);
            #region Sheet2 header Data
            row = sheet.CreateRow(0);
            cellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
            fontstyle = workbook.CreateFont();
            fontstyle.Color = IndexedColors.White.Index;
            fontstyle.Boldweight = (short)FontBoldWeight.Bold;
            fontstyle.FontHeightInPoints = 10;
            fontstyle.FontName = "Verdana";
            cellstyle.SetFont(fontstyle);
            color = new XSSFColor(new byte[] { 75, 172, 198 });
            cellstyle.FillForegroundColorColor = color;
            cellstyle.FillPattern = FillPattern.SolidForeground;

            for (int i = 0; i < dt_sheet2Data.Columns.Count; i++)
            {
                DataColumn dt_col = dt_sheet1Data.Columns[i];
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt_col.ColumnName);
                cell.CellStyle = cellstyle;
            }
            #endregion

            #region Sheet2 Detail Data
            fontstyle = workbook.CreateFont();
            fontstyle.FontHeightInPoints = 10;
            fontstyle.FontName = "Verdana";

            for (int i = 1; i <= dt_sheet2Data.Rows.Count; i++)
            {
                DataRow dt_row = dt_sheet2Data.Rows[i - 1];
                IRow detail_row = sheet.CreateRow(i);
                for (int j = 0; j < dt_sheet2Data.Columns.Count; j++)
                {
                    ICell cell = detail_row.CreateCell(j);
                    cell.SetCellValue(dt_row[j].ToString());
                    cell.CellStyle.SetFont(fontstyle);

                    //English_Name Dropdown List
                    //if (j == 4)
                    //{
                    //    DataTable dt_english = ModelInfoManager.GetEnglishForUploadExcel(rsi_no, dt_row[0].ToString(), dt_row[3].ToString());
                    //    if (dt_english.Rows.Count == 0)
                    //        continue;

                    //    CellRangeAddressList regions = new CellRangeAddressList(i, i, j, j);
                    //    XSSFDataValidationHelper helper = new XSSFDataValidationHelper(sheet);//獲得一個數據驗證Helper  
                    //    IDataValidation validation = helper.CreateValidation(helper.CreateExplicitListConstraint(dt_english.AsEnumerable().Select(p => p["ENGLISH_NAME"].ToString()).ToArray()), regions);//建立約束
                    //    sheet.AddValidationData(validation);
                    //}
                }
            }
            #endregion

            for (int i = 0; i < (int)row.LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            sheet.CreateFreezePane(0, 1);
            sheet.ProtectSheet(string.Empty);

            #endregion

            #region Sheet3 Data
            workbook.SetSheetName(2, sheet3name);
            sheet = (XSSFSheet)workbook.GetSheetAt(2);
            #region Sheet3 header Data
            row = sheet.CreateRow(0);
            cellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
            fontstyle = workbook.CreateFont();
            fontstyle.Color = IndexedColors.White.Index;
            fontstyle.Boldweight = (short)FontBoldWeight.Bold;
            fontstyle.FontHeightInPoints = 10;
            fontstyle.FontName = "Verdana";
            cellstyle.SetFont(fontstyle);
            color = new XSSFColor(new byte[] { 75, 172, 198 });
            cellstyle.FillForegroundColorColor = color;
            cellstyle.FillPattern = FillPattern.SolidForeground;

            for (int i = 0; i < dt_sheet3Data.Columns.Count; i++)
            {
                DataColumn dt_col = dt_sheet3Data.Columns[i];
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt_col.ColumnName);
                cell.CellStyle = cellstyle;
            }
            #endregion

            #region Sheet3 Detail Data
            fontstyle = workbook.CreateFont();
            fontstyle.FontHeightInPoints = 10;
            fontstyle.FontName = "Verdana";

            for (int i = 1; i <= dt_sheet3Data.Rows.Count; i++)
            {
                DataRow dt_row = dt_sheet3Data.Rows[i - 1];
                IRow detail_row = sheet.CreateRow(i);
                for (int j = 0; j < dt_sheet3Data.Columns.Count; j++)
                {
                    ICell cell = detail_row.CreateCell(j);
                    cell.SetCellValue(dt_row[j].ToString());
                    cell.CellStyle.SetFont(fontstyle);
                }
            }
            #endregion

            for (int i = 0; i < (int)row.LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            sheet.CreateFreezePane(0, 1);
            //sheet.ProtectSheet(string.Empty);

            #endregion

            #region sheet4 data clone sheet3          
            XSSFSheet sheet_org = (XSSFSheet)workbook.GetSheetAt(2);
            sheet_org.CopySheet(sheet4name);
            workbook.SetSheetName(3, sheet4name);
            sheet = (XSSFSheet)workbook.GetSheetAt(3);
            sheet.DisableLocking();
            workbook.SetSheetHidden(3, 1);
            #endregion

            #region Create Dropdownlist & Formula management
            XSSFSheet sheet1 = (XSSFSheet)workbook.GetSheetAt(0);
            int iStart = 0, iEnd = 0, iShift = 2;
            string sName = "";

            //for (int i = 1; i < dt_sheet3Data.Rows.Count; i++)
            for (int i = 0; i < dt_sheet3Data.Rows.Count - 1; i++)
            {
                sName = "_" + dt_sheet3Data.Rows[i]["PART_LEVEL"].ToString();
                if (dt_sheet3Data.Rows[i]["PART_LEVEL"].ToString() != dt_sheet3Data.Rows[i + 1]["PART_LEVEL"].ToString())
                {
                    if (!String.IsNullOrEmpty(dt_sheet3Data.Rows[i]["PART_LEVEL"].ToString()))
                    {
                        //名稱管理員設定
                        //iEnd = i + 1;
                        iEnd = i;
                        if (iStart == 0)
                        {
                            NPOI.SS.UserModel.IName namedRange = workbook.CreateName();
                            namedRange.NameName = sName;                        
                            namedRange.RefersToFormula = sheet4name + "!$B$2:$B$" + (iEnd + iShift).ToString();
                            iStart = i + 1;
                        }
                        else
                        {
                            NPOI.SS.UserModel.IName namedRange = workbook.CreateName();
                            namedRange.NameName = sName;
                            namedRange.RefersToFormula = sheet4name + "!$B$" + (iStart + iShift).ToString() + ":$B$" + (iEnd + iShift).ToString();
                            iStart = i + 1;
                        }
                        //iStart = iEnd;
                    }
                }

                /*
                if (i == dt_sheet3Data.Rows.Count-2)
                {
                    NPOI.SS.UserModel.IName namedRange = workbook.CreateName();
                    namedRange.NameName = sName;
                    namedRange.RefersToFormula = sheet4name + "!$B$" + (iStart + iShift).ToString() + ":$B$" + (dt_sheet3Data.Rows.Count + 1).ToString();
                }
                */
            }

            if (dt_sheet3Data.Rows[dt_sheet3Data.Rows.Count-1]["PART_LEVEL"].ToString() != dt_sheet3Data.Rows[dt_sheet3Data.Rows.Count-2]["PART_LEVEL"].ToString())
            {
                NPOI.SS.UserModel.IName namedRange = workbook.CreateName();
                sName = "_" + dt_sheet3Data.Rows[dt_sheet3Data.Rows.Count - 1]["PART_LEVEL"].ToString();
                namedRange.NameName = sName;
                namedRange.RefersToFormula = sheet4name + "!$B$" + (dt_sheet3Data.Rows.Count + 1).ToString() + ":$B$" + (dt_sheet3Data.Rows.Count + 1).ToString();
            }


            CT_DataValidation valid = new CT_DataValidation();
            XSSFDataValidationHelper helper = new XSSFDataValidationHelper(sheet1);
            valid.showDropDown = true;
            int sShift = 2; //起始位移
            int fShift = 3; //公式位移
            valid.allowBlank = true;
            for (int i = 0; i <= dt_sheet1Data.Rows.Count - 1; i++)
            {
                if (dt_sheet1Data.Rows[i]["ISMODIFY"].ToString().Equals("Y"))
                {
                    //設定下拉選單
                    CellRangeAddressList ddlregions = new CellRangeAddressList(i + sShift, i + sShift, 4, 4);
                    XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)helper.CreateFormulaListConstraint("INDIRECT(\"_\"&D" + (i + fShift).ToString() + ")");
                    XSSFDataValidation dataValidation = (XSSFDataValidation)helper.CreateValidation(dvConstraint, ddlregions);
                    dataValidation.SuppressDropDownArrow = true;
                    dataValidation.ShowErrorBox = true;
                    sheet1.AddValidationData(dataValidation);
                }
            }

            //設定底下千筆欄位格式
            for (int j= dt_sheet1Data.Rows.Count; j <= (dt_sheet1Data.Rows.Count + 1000); j++)
            {
                CellRangeAddressList ddlregions = new CellRangeAddressList(j + sShift, j + sShift, 4, 4);
                XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)helper.CreateFormulaListConstraint("INDIRECT(\"_\"&D" + (j + fShift).ToString() + ")");
                XSSFDataValidation dataValidation = (XSSFDataValidation)helper.CreateValidation(dvConstraint, ddlregions);
                dataValidation.SuppressDropDownArrow = true;
                dataValidation.ShowErrorBox = true;
                sheet1.AddValidationData(dataValidation);

                IRow detail_row = sheet1.CreateRow(j + 2);
                for (int k = 0; k < dt_sheet1Data.Columns.Count; k++)
                {
                    XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    cellStyle.IsLocked = false;
                    ICell cell = detail_row.CreateCell(k);
                    IFont sFont = workbook.CreateFont();
                    sFont.FontHeightInPoints = 10;
                    sFont.FontName = "Verdana";
                    cellStyle.SetFont(sFont);
                    detail_row.GetCell(k).CellStyle = cellStyle;                 
                    if (dt_sheet1Data.Columns[k].ColumnName.Equals("PARENT_LEVEL") || dt_sheet1Data.Columns[k].ColumnName.Equals("PART_LEVEL"))
                    {
                        IDataFormat dataFormat = workbook.CreateDataFormat();                                          
                        cellStyle.DataFormat = dataFormat.GetFormat("@");                       
                        cellStyle.SetFont(sFont);
                        detail_row.GetCell(k).CellStyle = cellStyle;
                    }
                }
                
            }
            #endregion

            //workbook.RemoveSheetAt(2);

            MemoryStream memoryStream = new MemoryStream();
            workbook.Write(memoryStream);

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename + ".xlsx");
        }

        [HttpPost]
        public ActionResult UploadExcel(string rsi_no, string parttype, HttpPostedFileBase uploadfile)
        {
            string filename = Path.GetFileName(uploadfile.FileName);
            //var httpPostedFile = Request.Files["uploadfile"];
            JsonMessage jsonmessage = new JsonMessage();

            List<H_Product_DetailTree> uploaddata = new List<H_Product_DetailTree>();
            IWorkbook workbook = new XSSFWorkbook(uploadfile.InputStream);
            XSSFSheet sheet = (XSSFSheet)workbook.GetSheetAt(0);
            sheet.ProtectSheet(null);
            int row_number = sheet.LastRowNum;
            int col_number = sheet.GetRow(1).LastCellNum;
            bool dataAccuracy = true;
            #region 檢查資料欄位正確性，並將資料轉換至List Object(h_Product_DetailStructures)
            
            for (int i = 2; i <= row_number; i++)
            {
                H_Product_DetailTree item = new H_Product_DetailTree();
                string errormessage = string.Empty;
                IRow row = sheet.GetRow(i);
                string parent_level = GetCellData(row, 0);
                string part_type = GetCellData(row, 1);
                string part_no = GetCellData(row, 2);
                string part_level = GetCellData(row, 3);
                string english_name = GetCellData(row, 4);
                string part_spec = GetCellData(row, 5);
                string usage = GetCellData(row, 6);
                string part_unit = GetCellData(row, 7);
                string remark = GetCellData(row, 8);
                string lockkey = GetCellData(row, col_number - 1);
                item.parent_level = parent_level;
                item.part_type = part_type;
                item.part_no = part_no;
                item.part_level = part_level;
                item.english_name = english_name;
                item.part_spec = part_spec;
                item.usage = usage;
                item.unit = part_unit;
                item.remark = remark;
                item.ismodify = lockkey;
                //確認excel資料整行是不是空白，如果全部欄位都是空白，則不進行欄位檢查
                bool exceldatarownull = string.IsNullOrEmpty(item.parent_level) && string.IsNullOrEmpty(item.part_type) && string.IsNullOrEmpty(item.part_no) &&
                                        string.IsNullOrEmpty(item.part_level) && string.IsNullOrEmpty(item.english_name) && string.IsNullOrEmpty(item.part_spec) &&
                                        string.IsNullOrEmpty(item.usage) && string.IsNullOrEmpty(item.unit) && string.IsNullOrEmpty(item.remark) &&
                                        string.IsNullOrEmpty(item.ismodify);

                //先判斷parent_level欄位有無資料
                if (!String.IsNullOrEmpty(item.parent_level))
                {
                    //檢查是否有重複上傳的資料
                    if (CheckH_Product_DetailTree(uploaddata, item))
                        errormessage = SetErrorMessage(errormessage, "資料重複，請重新確認");
                    else
                        uploaddata.Add(item);
                }

                row.CreateCell(col_number).SetCellValue(string.Empty);
                //檢查可編輯的資料
                if ((lockkey.Equals("Y") || string.IsNullOrEmpty(lockkey)) && !exceldatarownull)
                {
                    //PARENT_LEVEL欄位檢查
                    if (!String.IsNullOrEmpty(parent_level))
                    {
                        if (!parent_level.Equals("9X"))
                        {
                            if (parent_level.Length == 2)
                            {
                                DataTable dt_parent_level = ModelInfoManager.GetPartKindKeyForUploadExcel(parent_level);
                                if (Convert.ToInt32(dt_parent_level.Rows[0][0].ToString()) == 0)
                                {
                                    errormessage = SetErrorMessage(errormessage, "PARENT_LEVEL不存在，請重新確認");
                                }
                            }
                            else
                            {
                                DataTable dt_parent_level = ModelInfoManager.GetPartNoForUploadExcel(parent_level);
                                if (Convert.ToInt32(dt_parent_level.Rows[0][0].ToString()) == 0)
                                {
                                    errormessage = SetErrorMessage(errormessage, "PARENT_LEVEL不存在，請重新確認");
                                }
                            }
                        }
                    }
                    else
                    {
                        errormessage = SetErrorMessage(errormessage, "PARENT_LEVEL不可為空白");
                    }

                    //PART_TYPE欄位檢查
                    if (!String.IsNullOrEmpty(part_type))
                    {
                        DataTable dt_parttype = ModelInfoManager.GetPartTypeForUploadExcel();
                        if (!dt_parttype.AsEnumerable().Where(p => p[0].ToString().Equals(part_type)).Any())
                        {
                            errormessage = SetErrorMessage(errormessage, "PARENT_LEVEL不存在，請重新確認");
                        }
                    }
                    else
                    {
                        errormessage = SetErrorMessage(errormessage, "PART_TYPE不可為空白");
                    }

                    //PART_NO欄位檢查
                    //有值則檢查是否存在於資料庫
                    //無值則檢查PART_LEVEL、ENGLISH_NAME、PART_SPEC欄位
                    if (!String.IsNullOrEmpty(part_no))
                    {
                        DataTable dt_partno = ModelInfoManager.GetPartNoForUploadExcel(part_no);
                        if (Convert.ToInt32(dt_partno.Rows[0][0].ToString()) == 0)
                        {
                            errormessage = SetErrorMessage(errormessage, "PART_NO不存在，請重新確認");
                        }
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(part_level) || string.IsNullOrEmpty(english_name) || string.IsNullOrEmpty(part_spec))
                        {
                            errormessage = SetErrorMessage(errormessage, "PART_NO或PART_LEVEL+ENGLISH_NAME+PART_SPEC不可為空白");
                        }
                        else
                        {
                            DataTable dt_part_level = ModelInfoManager.GetPartKindKeyForUploadExcel(part_level);
                            if (Convert.ToInt32(dt_part_level.Rows[0][0].ToString()) == 0)
                            {
                                errormessage = SetErrorMessage(errormessage, "PART_LEVEL不存在，請重新確認");
                            }

                            DataTable dt_partlevelenglish = ModelInfoManager.GetPartLevelEnglishForUploadExcel(part_level, english_name);
                            if (Convert.ToInt32(dt_partlevelenglish.Rows[0][0].ToString()) == 0)
                            {
                                errormessage = SetErrorMessage(errormessage, "PART_LEVEL、ENGLISH_NAME不存在，請重新確認");
                            }
                        }
                    }

                    //USAGE欄位檢查
                    if (!string.IsNullOrEmpty(usage))
                    {
                        decimal number = 0;
                        bool parse = Decimal.TryParse(usage, out number);
                        if (parse)
                        {
                            if (number == 0)
                                errormessage = SetErrorMessage(errormessage, "USAGE必須大於0");
                        }
                        else
                        {
                            errormessage = SetErrorMessage(errormessage, "USAGE必須為數值");
                        }
                    }
                    else
                    {
                        errormessage = SetErrorMessage(errormessage, "USAGE不可為空白");
                    }

                    //PART_UNIT欄位檢查
                    if (!String.IsNullOrEmpty(part_unit))
                    {
                        string s_part_unit = part_unit.ToUpper();
                        List<string> list_unit = MterialGroupPartsManager.GetUnit().ToList();
                        if (!list_unit.Any(p => p.Equals(s_part_unit)))
                        {
                            errormessage = SetErrorMessage(errormessage, "PART_UNIT不存在，請重新確認");
                        }
                    }
                    else
                    {
                        errormessage = SetErrorMessage(errormessage, "PART_UNIT不可為空白");
                    }

                }

                //errormessage有值表示有錯誤訊息，顯示錯誤訊息至Excel上
                if (!string.IsNullOrEmpty(errormessage))
                {
                    dataAccuracy = false;

                    ICell cell = row.CreateCell(col_number);
                    cell.SetCellValue(errormessage);

                    XSSFCellStyle cellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    IFont fontstyle = workbook.CreateFont();
                    fontstyle.Color = IndexedColors.Red.Index;
                    cellstyle.SetFont(fontstyle);
                    cellstyle.IsLocked = false;
                    cell.CellStyle = cellstyle;
                }
            }

            sheet.UnlockDeleteRows();
            sheet.UnlockInsertRows();
            sheet.EnableLocking();
            #endregion

            #region 當資料檢查發現有問題後，請使用者下載檔案重新編輯
            if (!dataAccuracy)
            {
                
                string path = string.Format("{0}{1}", Server.MapPath("~/File/"), filename);
                FileStream file = new FileStream(path, FileMode.Create);//產生檔案
                workbook.Write(file);
                file.Close();

                jsonmessage.status = false;
                jsonmessage.message = "資料錯誤，請下載檔案調整";
                jsonmessage.attr1 = filename;
                return Json(jsonmessage);
            }
            #endregion

            #region 檢查資料樹狀結構
            //step1：取得top階料號，top階料號為起始源頭。
            //step2：進行迴圈將所有料階設定至top階料號底下。
            //step3：採用遞迴將資料設定到對應的料階底下，邏輯為"父階的part_no=新增料號的parent_level"
            //step4：檢查新增的料號是否有正確的加入樹狀結構，正常狀況只會加入1次(addcount=1)，如果沒有加入(addcount=0)或加入多次(addcount>1)都是錯誤的結構設定
            H_Product_DetailTree topnode = uploaddata.FirstOrDefault(); //設定top階料號，setp1
            topnode.nodes = new List<H_Product_DetailTree>();
            List<H_Product_DetailTree> errornode = new List<H_Product_DetailTree>(); //記錄有問題的料號
            for (int i = 1; i < uploaddata.Count; i++) //迴圈方法，step2
            {
                H_Product_DetailTree node = uploaddata[i];
                int addcount = 0; //記錄料號加入樹狀結構的次數
                Tuple<H_Product_DetailTree, int> result = SetNodeToTopNode(topnode, node, addcount); //遞迴方法，step3
                topnode = result.Item1;
                if (result.Item2 != 1) //檢查料號加入的次數，必須等於1，step4
                    errornode.Add(node);
            }
            #endregion

            #region 樹狀結構有問題後，請使用者下載檔案重新編輯
            if (errornode.Count > 0)
            {
                foreach (H_Product_DetailTree node in errornode)
                {
                    int index = uploaddata.IndexOf(node);
                    sheet.GetRow(index + 2).GetCell(col_number).SetCellValue("BOM表結構有問題，請重新調整資料");


                    XSSFCellStyle cellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    IFont fontstyle = workbook.CreateFont();
                    fontstyle.Color = IndexedColors.Red.Index;
                    cellstyle.SetFont(fontstyle);
                    cellstyle.IsLocked = false;
                    sheet.GetRow(index + 2).GetCell(col_number).CellStyle = cellstyle;
                }

                string path = string.Format("{0}{1}", Server.MapPath("~/File/"), filename);
                FileStream file = new FileStream(path, FileMode.Create);//產生檔案
                workbook.Write(file);
                file.Close();

                jsonmessage.status = false;
                jsonmessage.message = "資料錯誤，請下載檔案調整";
                jsonmessage.attr1 = filename;
                return Json(jsonmessage);
            }
            #endregion

            #region 資料沒有錯誤，寫入資料庫
            
            H_Product_DetailManager.DeleteDetailTempForUpload(rsi_no, parttype);
            foreach (var item in uploaddata.Where(p => p.ismodify.Equals("Y") || string.IsNullOrEmpty(p.ismodify)).ToList())
            {
                H_Product_DetailManager.InsertDetailTempForUpload(rsi_no, item, Employee.EmpNO);
            }
            H_Product_DetailManager.Ins_rd_upload_bom(rsi_no, parttype, Employee.EmpNO);

            //H_Product_DetailManager.DeleteDetailForUpload(rsi_no, parttype, Employee.EmpNO);
            #endregion

            jsonmessage.status = true;
            jsonmessage.message = "檔案上傳成功，重新整理畫面";
            return Json(jsonmessage);
        }

        [HttpPost]
        public ActionResult DownloadErrorExcel(string file_name)
        {
            string path = string.Format("{0}{1}", Server.MapPath("~/File/"), file_name);
            if (System.IO.File.Exists(path))
            {
                IWorkbook workbook = new XSSFWorkbook(path);
                MemoryStream memoryStream = new MemoryStream();
                workbook.Write(memoryStream);
                workbook.Close();

                System.IO.File.Delete(path);
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
            }

            return JavaScript("<script>alert('檔案不存在');</script>");
        }

        [HttpPost]
        public ActionResult GetPartNoMfg(string part_no)
        {
            var part_no_mfg = H_Product_DetailManager.GetPartNoMfg(part_no);
            return Json(part_no_mfg);
        }

        //Export Execl funciton
        private byte[] ExchangeExcel(string rsi_no, IList<H_Product_DetailEntity> bom, Dictionary<string, IList<H_Product_DetailEntity>> specialData)
        {
            //IWorkbook wb = new HSSFWorkbook();
            //ISheet ws;

            ////建立Excel 2007檔案
            IWorkbook wb = new XSSFWorkbook();
            ISheet ws;

            if (bom.Count() > 0)
            {
                ws = wb.CreateSheet("Normal Parts");
                ws.CreateRow(0);//第一行為欄位名稱

                #region 設定excel第一行
                XSSFCellStyle cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                IFont fontstyle = wb.CreateFont();
                fontstyle.Color = IndexedColors.White.Index;
                cellstyle.SetFont(fontstyle);
                var color = new XSSFColor(new byte[] { 60, 141, 188 });
                cellstyle.FillForegroundColorColor = color;
                cellstyle.FillPattern = FillPattern.SolidForeground;

                ws.GetRow(0).CreateCell(0).SetCellValue("Part No");
                ws.GetRow(0).GetCell(0).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(1).SetCellValue("Level");
                ws.GetRow(0).GetCell(1).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(2).SetCellValue("Part Level");
                ws.GetRow(0).GetCell(2).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(3).SetCellValue("Material Group");
                ws.GetRow(0).GetCell(3).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(4).SetCellValue("Material Parts");
                ws.GetRow(0).GetCell(4).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(5).SetCellValue("Part Type");
                ws.GetRow(0).GetCell(5).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(6).SetCellValue("English Name");
                ws.GetRow(0).GetCell(6).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(7).SetCellValue("Part Description");
                ws.GetRow(0).GetCell(7).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(8).SetCellValue("Part Spec");
                ws.GetRow(0).GetCell(8).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(9).SetCellValue("Usage");
                ws.GetRow(0).GetCell(9).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(10).SetCellValue("Unit");
                ws.GetRow(0).GetCell(10).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(11).SetCellValue("Remark");
                ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(12).SetCellValue("Upload File");
                ws.GetRow(0).GetCell(12).CellStyle = cellstyle;
                ws.CreateFreezePane(0, 1);
                #endregion

                for (int i = 0; i < bom.Count(); i++)
                {
                    ws.CreateRow(i + 1);
                    #region 綁定資料
                    ws.GetRow(i + 1).CreateCell(0).SetCellValue(bom[i].DISPLAYPARTNO.ToString());
                    ws.GetRow(i + 1).CreateCell(1).SetCellValue(bom[i].BOM_LEVEL.ToString());
                    ws.GetRow(i + 1).CreateCell(2).SetCellValue(bom[i].PART_LEVEL);
                    ws.GetRow(i + 1).CreateCell(3).SetCellValue(bom[i].MTL_GROUP);
                    ws.GetRow(i + 1).CreateCell(4).SetCellValue(bom[i].MTL_PARTS);
                    ws.GetRow(i + 1).CreateCell(5).SetCellValue(bom[i].PART_TYPE);
                    ws.GetRow(i + 1).CreateCell(6).SetCellValue(bom[i].ENGLISH_NAME);
                    ws.GetRow(i + 1).CreateCell(7).SetCellValue(bom[i].PART_DESC);
                    ws.GetRow(i + 1).CreateCell(8).SetCellValue(bom[i].PART_SPEC);
                    ws.GetRow(i + 1).CreateCell(9).SetCellValue(bom[i].USAGE.ToString());
                    ws.GetRow(i + 1).CreateCell(10).SetCellValue(bom[i].PART_UNIT);
                    ws.GetRow(i + 1).CreateCell(11).SetCellValue(bom[i].REMARK);
                    ws.GetRow(i + 1).CreateCell(12).SetCellValue(bom[i].FILE_STATUS);
                    #endregion
                }
            }

            if (specialData.Count() > 0)
            {
                foreach (var itemData in specialData)
                {
                    ws = wb.CreateSheet(H_Product_DetailManager.GetGroupName(rsi_no, itemData.Key));
                    ws.CreateRow(0);//第一行為欄位名稱
                    #region 設定excel第一行
                    XSSFCellStyle cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                    IFont fontstyle = wb.CreateFont();
                    fontstyle.Color = IndexedColors.White.Index;
                    cellstyle.SetFont(fontstyle);
                    var color = new XSSFColor(new byte[] { 60, 141, 188 });
                    cellstyle.FillForegroundColorColor = color;
                    cellstyle.FillPattern = FillPattern.SolidForeground;

                    ws.GetRow(0).CreateCell(0).SetCellValue("Parent Part No");
                    ws.GetRow(0).GetCell(0).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(1).SetCellValue("Part No");
                    ws.GetRow(0).GetCell(1).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(2).SetCellValue("Material Group");
                    ws.GetRow(0).GetCell(2).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(3).SetCellValue("Material Parts");
                    ws.GetRow(0).GetCell(3).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(4).SetCellValue("Part Type");
                    ws.GetRow(0).GetCell(4).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(5).SetCellValue("English Name");
                    ws.GetRow(0).GetCell(5).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(6).SetCellValue("Part Description");
                    ws.GetRow(0).GetCell(6).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(7).SetCellValue("Part Spec");
                    ws.GetRow(0).GetCell(7).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(8).SetCellValue("Usage");
                    ws.GetRow(0).GetCell(8).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(9).SetCellValue("Unit");
                    ws.GetRow(0).GetCell(9).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(10).SetCellValue("Remark");
                    ws.GetRow(0).GetCell(10).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(11).SetCellValue("Upload File");
                    ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                    ws.CreateFreezePane(0, 1);
                    #endregion

                    int i = 0;
                    foreach (var item in itemData.Value)
                    {
                        ws.CreateRow(i + 1);
                        #region 綁定資料
                        ws.GetRow(i + 1).CreateCell(0).SetCellValue(item.PARTNUMBER_PARENT.ToString());
                        ws.GetRow(i + 1).CreateCell(1).SetCellValue(item.PART_NO);
                        ws.GetRow(i + 1).CreateCell(2).SetCellValue(item.MTL_GROUP);
                        ws.GetRow(i + 1).CreateCell(3).SetCellValue(item.MTL_PARTS);
                        ws.GetRow(i + 1).CreateCell(4).SetCellValue(item.PART_TYPE);
                        ws.GetRow(i + 1).CreateCell(5).SetCellValue(item.ENGLISH_NAME);
                        ws.GetRow(i + 1).CreateCell(6).SetCellValue(item.PART_DESC);
                        ws.GetRow(i + 1).CreateCell(7).SetCellValue(item.PART_SPEC);
                        ws.GetRow(i + 1).CreateCell(8).SetCellValue(item.USAGE.ToString());
                        ws.GetRow(i + 1).CreateCell(9).SetCellValue(item.REMARK);
                        ws.GetRow(i + 1).CreateCell(10).SetCellValue(item.FILE_STATUS);
                        #endregion
                        i++;
                    }
                }
            }

            MemoryStream stream = new MemoryStream();
            wb.Write(stream);
            wb = null;
            return stream.ToArray();
        }

        private string SetErrorMessage(string errormessage, string message)
        {
            if (String.IsNullOrEmpty(errormessage))
            {
                return message + " ";
            }
            else
            {
                return errormessage + message + " ";
            }
        }

        private bool CheckH_Product_DetailTree(List<H_Product_DetailTree> uploadData, H_Product_DetailTree item)
        {
            return uploadData.Where(p => p.parent_level.Equals(item.parent_level) && p.part_type.Equals(item.part_type) && p.part_no.Equals(item.part_no) &&
                                         p.part_level.Equals(item.part_level) && p.english_name.Equals(item.english_name) && p.part_spec.Equals(item.part_spec)).Any();
        }

        /// <summary>
        /// 設定新料號到樹狀結構
        /// step1：檢查父階料號是否有下階層。
        /// step2：有下階層時，進行迴圈且遞迴的方式執行SetNodeToTopNode，直到沒有下階層為止。
        /// step3：檢查父階料號的part_no是否等於新增料號的parent_level，相等時則將新增的料號加入到父階料號底下，並將次數+1。
        /// </summary>
        /// <param name="parent_node">父階料號</param>
        /// <param name="node">新增的料號</param>
        /// <param name="addcount">新增料號被加入到樹狀結構的次數</param>
        /// <returns></returns>
        private Tuple<H_Product_DetailTree, int> SetNodeToTopNode(H_Product_DetailTree parent_node, H_Product_DetailTree node, int addcount)
        {
            if (parent_node.nodes != null) //step1
            {
                //step2
                for (int i = 0; i < parent_node.nodes.Count; i++)
                {
                    Tuple<H_Product_DetailTree, int> result = SetNodeToTopNode(parent_node.nodes[i], node, addcount);
                    parent_node.nodes[i] = result.Item1;
                    addcount = result.Item2;
                }
            }

            //step3
            if (parent_node.part_no == node.parent_level)
            {
                if (parent_node.nodes == null) parent_node.nodes = new List<H_Product_DetailTree>();
                parent_node.nodes.Add(node);
                addcount++;
            }
            else if(parent_node.part_level == node.parent_level)
            {
                if (parent_node.nodes == null) parent_node.nodes = new List<H_Product_DetailTree>();
                parent_node.nodes.Add(node);
                addcount++;
            }

            return Tuple.Create(parent_node, addcount);
        }

        private string GetCellData(IRow row, int i)
        {
            if (row.GetCell(i) == null)
                return string.Empty;

            return row.GetCell(i).CellType.Equals(CellType.Numeric) ? ((Decimal)row.GetCell(i).NumericCellValue).ToString() : row.GetCell(i).StringCellValue;
        }
    }
}