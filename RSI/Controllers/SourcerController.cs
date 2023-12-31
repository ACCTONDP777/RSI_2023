﻿using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using AUO.Common.Authentication;
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
using System.Web.Security;

namespace RSI.Controllers
{
    public class SourcerController : Controller
    {
        // GET: Sourcer
        [Auto_Identify]
        public ActionResult Index()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        [HttpPost]
        public ActionResult Sourcer_TableView(string rsi_no, string part_type, string bu, string phase_id)
        {

            H_Product_DetailVM h_Product_DetailVM = new H_Product_DetailVM();
            h_Product_DetailVM.normal_Detail = H_Product_DetailManager.Get_Normal_Sourcer_View(rsi_no, Employee.EmpNO, part_type).ToList();
            h_Product_DetailVM.normal_Detail = GetPriceTrend(h_Product_DetailVM.normal_Detail);
            var item_no_list = h_Product_DetailVM.normal_Detail.Where(p => p.ISMODIFY == "Y").Select(p => p.PART_NO).ToArray();
            var ws_str = String.Join(",", item_no_list);
            //MCAWebService.MCAWebService service = new MCAWebService.MCAWebService();
            var ws_result = String.Empty;
            if (String.IsNullOrEmpty(ws_str))
                ws_result = "No Data Found";
            else
                ws_result = "No Data Found";
                //ws_result = service.get_quo_info_for_RSI(ws_str);

            if (!ws_result.Equals("No Data Found") && !ws_result.Equals("Not Authorized"))
            {
                DataTable dt_normal = XMLHelper.GetDataSetFromXML(ws_result);
                var convert_dt = dt_normal.AsEnumerable().GroupBy(p => p.Field<string>("ITEM_NO"));
                foreach (var group_dt in convert_dt)
                {
                    var item_no = group_dt.Key;
                    decimal price = 0, price_HIS_H = 0, price_HIS_L = 0;
                    decimal? pricenull, price_HIS_H_null, price_HIS_L_null;
                    string tempPrice = group_dt.Where(p => p.Field<string>("TYPE") == "N").FirstOrDefault()["USD_PRICE"].ToString();
                    pricenull = Decimal.TryParse(tempPrice, out price) ? price : (decimal?)null;
                    string tempHPrice = group_dt.Where(p => p.Field<string>("TYPE") == "H").FirstOrDefault()["USD_PRICE"].ToString();
                    price_HIS_H_null = Decimal.TryParse(tempHPrice, out price_HIS_H) ? price_HIS_H : (decimal?)null;
                    string tempLPrice = group_dt.Where(p => p.Field<string>("TYPE") == "L").FirstOrDefault()["USD_PRICE"].ToString();
                    price_HIS_L_null = Decimal.TryParse(tempLPrice, out price_HIS_L) ? price_HIS_L : (decimal?)null;

                    //string currency = group_dt.Select(p => p.Field<string>("ORI_CURRENCY")).FirstOrDefault();
                    //decimal? rate = group_dt.Select(p => p.Field<decimal?>("RATE")).FirstOrDefault();
                    //string source_no = group_dt.Select(p => p.Field<string>("QUO_NO")).FirstOrDefault();
                    //string source = group_dt.Select(p => p.Field<string>("SOURCE")).FirstOrDefault();
                    string currency = group_dt.Where(p => p.Field<string>("TYPE") == "N").FirstOrDefault()["ORI_CURRENCY"].ToString();
                    decimal? rate = (decimal?)group_dt.Where(p => p.Field<string>("TYPE") == "N").FirstOrDefault()["RATE"];
                    string source_no = group_dt.Where(p => p.Field<string>("TYPE") == "N").FirstOrDefault()["QUO_NO"].ToString();
                    string source = group_dt.Where(p => p.Field<string>("TYPE") == "N").FirstOrDefault()["SOURCE"].ToString();

                    var item_model = h_Product_DetailVM.normal_Detail.Where(p => p.PART_NO == item_no).FirstOrDefault();
                    item_model.PRICE = pricenull;
                    item_model.PRICE_PM = item_model.PRICE_PM > 0 ? item_model.PRICE_PM : pricenull;
                    item_model.PRICE_HIS_H = price_HIS_H_null;
                    item_model.PRICE_HIS_L = price_HIS_L_null;
                    item_model.CURRENCY = currency;
                    item_model.RATE = (rate != null) ? (decimal)rate : 0;
                    item_model.SOURCE_NO = source_no;
                    item_model.SOURCE = source;
                }
            }

            var exportexcelconfig = RSI_ConfigEntityDAL.GetExportExcelConfig(bu, part_type, phase_id);
            if (exportexcelconfig.Rows.Count > 0)
            {
                var pariceIdentity = exportexcelconfig.Rows[0]["Attribute4"].ToString();
                if (pariceIdentity.Equals("Part_No"))
                    h_Product_DetailVM.normal_Detail = ComputeSubTotalForPartNo(h_Product_DetailVM.normal_Detail);
                if (pariceIdentity.Equals("Material_Parts"))
                    h_Product_DetailVM.normal_Detail = ComputeSubTotalForMaterialPart(h_Product_DetailVM.normal_Detail);
                if (pariceIdentity.Equals("Parts_Group"))
                    h_Product_DetailVM.normal_Detail = ComputeSubTotalForPartsGroup(h_Product_DetailVM.normal_Detail);
                if (pariceIdentity.Equals("Material_Group"))
                    h_Product_DetailVM.normal_Detail = ComputeSubTotalForMaterialGroup(h_Product_DetailVM.normal_Detail);
            }

            return Json(h_Product_DetailVM);
        }

        [HttpPost]
        public ActionResult Sourcer_GetSpecialPartsData(string rsi_no, string part_type, string group_id)
        {
            var models = H_Product_DetailManager.Get_Special_Sourcer(rsi_no, Employee.EmpNO, part_type);
            if (!String.IsNullOrEmpty(group_id))
                models = models.Where(p => p.GROUP_ID == group_id).ToList();
            models = GetPriceTrend(models);
            var item_no_list = models.Where(p => p.ISMODIFY == "Y").Select(p => p.PART_NO).ToArray();
            var ws_str = String.Join(",", item_no_list);
            var ws_result = String.Empty;
            //MCAWebService.MCAWebService service = new MCAWebService.MCAWebService();
            if (String.IsNullOrEmpty(ws_str))
                ws_result = "No Data Found";
            else
                //ws_result = service.get_quo_info_for_RSI(ws_str);
                ws_result = "No Data Found";

            if (!ws_result.Equals("No Data Found") && !ws_result.Equals("Not Authorized"))
            {
                DataTable dt_specail = XMLHelper.GetDataSetFromXML(ws_result);
                var convert_dt = dt_specail.AsEnumerable().GroupBy(p => p.Field<string>("ITEM_NO"));
                foreach (var group_dt in convert_dt)
                {
                    var item_no = group_dt.Key;
                    decimal price = 0, price_HIS_H = 0, price_HIS_L = 0;
                    string tempPrice = group_dt.Where(p => p.Field<string>("TYPE") == "N").FirstOrDefault()["USD_PRICE"].ToString();
                    price = String.IsNullOrEmpty(tempPrice) ? 0 : Decimal.Parse(tempPrice);
                    string tempHPrice = group_dt.Where(p => p.Field<string>("TYPE") == "H").FirstOrDefault()["USD_PRICE"].ToString();
                    price_HIS_H = String.IsNullOrEmpty(tempHPrice) ? 0 : Decimal.Parse(tempHPrice);
                    string tempLPrice = group_dt.Where(p => p.Field<string>("TYPE") == "L").FirstOrDefault()["USD_PRICE"].ToString();
                    price_HIS_L = String.IsNullOrEmpty(tempLPrice) ? 0 : Decimal.Parse(tempLPrice);

                    string currency = group_dt.Select(p => p.Field<string>("ORI_CURRENCY")).FirstOrDefault();
                    decimal? rate = group_dt.Select(p => p.Field<decimal?>("RATE")).FirstOrDefault();
                    string source_no = group_dt.Select(p => p.Field<string>("QUO_NO")).FirstOrDefault();
                    string source = group_dt.Select(p => p.Field<string>("SOURCE")).FirstOrDefault();
                    var item_model = models.Where(p => p.PART_NO == item_no).ToList();
                    foreach (var model in item_model)
                    {
                        model.PRICE = price;
                        model.PRICE_PM = model.PRICE_PM > 0 ? model.PRICE_PM : price;
                        model.PRICE_HIS_H = price_HIS_H;
                        model.PRICE_HIS_L = price_HIS_L;
                        model.CURRENCY = currency;
                        model.RATE = (rate != null) ? (decimal)rate : 0;
                        model.SOURCE_NO = source_no;
                        model.SOURCE = source;
                    }
                }
            }

            return Json(models);
        }

        [HttpPost]
        public ActionResult SourcerReview_GetBOMTableData(string rsi_no, string part_type, string phase_id)
        {
            return Json(H_Product_DetailManager.GetBOMTableDataForSourcer(rsi_no, part_type, phase_id, Employee.EmpNO));
        }

        [HttpPost]
        public ActionResult checkAllPMPrice(string rsi_no, string part_type)
        {
            var number = H_Product_DetailManager.GetNumberForCheckAllPMPrice(rsi_no, part_type);
            return Json(number);
        }

        [Auto_Identify]
        public ActionResult Boss()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        [HttpPost]
        public ActionResult Boss_TableView(string rsi_no, string part_type, string bu, string phase_id)
        {
            H_Product_DetailVM h_Product_DetailVM = new H_Product_DetailVM();
            h_Product_DetailVM.normal_Detail = H_Product_DetailManager.Get_Normal_Sourcer_Boss(rsi_no, Employee.EmpNO, part_type);
            h_Product_DetailVM.normal_Detail = GetPriceTrend(h_Product_DetailVM.normal_Detail);
            var exportexcelconfig = RSI_ConfigEntityDAL.GetExportExcelConfig(bu, part_type, phase_id);
            if (exportexcelconfig.Rows.Count > 0)
            {
                var pariceIdentity = exportexcelconfig.Rows[0]["Attribute4"].ToString();
                if (pariceIdentity.Equals("Part_No"))
                    h_Product_DetailVM.normal_Detail = ComputeSubTotalForPartNo(h_Product_DetailVM.normal_Detail);
                if (pariceIdentity.Equals("Material_Parts"))
                    h_Product_DetailVM.normal_Detail = ComputeSubTotalForMaterialPart(h_Product_DetailVM.normal_Detail);
                if (pariceIdentity.Equals("Parts_Group"))
                    h_Product_DetailVM.normal_Detail = ComputeSubTotalForPartsGroup(h_Product_DetailVM.normal_Detail);
                if (pariceIdentity.Equals("Material_Group"))
                    h_Product_DetailVM.normal_Detail = ComputeSubTotalForMaterialGroup(h_Product_DetailVM.normal_Detail);
            }
            return Json(h_Product_DetailVM);
        }

        [HttpPost]
        public ActionResult Boss_GetSpecialPartsData(string rsi_no, string part_type, string group_id)
        {
            var model = H_Product_DetailManager.Get_Special_Sourcer_Boss(rsi_no, Employee.EmpNO, part_type);
            model = model.Where(p => p.GROUP_ID == group_id).ToList();
            model = GetPriceTrend(model);
            return Json(model);
        }

        public ActionResult Template()
        {
            return View();
        }

        public ActionResult Template_TableView(string rsi_no, string part_type, string phase_id)
        {
            H_Product_DetailVM h_Product_DetailVM = new H_Product_DetailVM();
            h_Product_DetailVM.normal_Detail = H_Product_DetailManager.Get_Normal_Sourcer_View(rsi_no, Employee.EmpNO, part_type);

            var item_no_list = h_Product_DetailVM.normal_Detail.Where(p => p.ISMODIFY == "Y").Select(p => p.PART_NO).ToArray();
            var ws_str = String.Join(",", item_no_list);
            //MCAWebService.MCAWebService service = new MCAWebService.MCAWebService();
            var ws_result = String.Empty;
            if (String.IsNullOrEmpty(ws_str))
                ws_result = "No Data Found";
            else
                ws_result = "No Data Found";
                //ws_result = service.get_quo_info_for_RSI(ws_str);

            if (!ws_result.Equals("No Data Found") && !ws_result.Equals("Not Authorized"))
            {
                DataTable dt_normal = XMLHelper.GetDataSetFromXML(ws_result);
                var convert_dt = dt_normal.AsEnumerable().GroupBy(p => p.Field<string>("ITEM_NO"));
                foreach (var group_dt in convert_dt)
                {
                    var item_no = group_dt.Key;
                    decimal price = group_dt.Where(p => p.Field<string>("TYPE") == "N").Select(p => p.Field<decimal>("USD_PRICE")).FirstOrDefault();
                    decimal price_HIS_H = group_dt.Where(p => p.Field<string>("TYPE") == "H").Select(p => p.Field<decimal>("USD_PRICE")).FirstOrDefault();
                    decimal price_HIS_L = group_dt.Where(p => p.Field<string>("TYPE") == "L").Select(p => p.Field<decimal>("USD_PRICE")).FirstOrDefault();
                    string currency = group_dt.Select(p => p.Field<string>("ORI_CURRENCY")).FirstOrDefault();
                    decimal? rate = group_dt.Select(p => p.Field<decimal?>("RATE")).FirstOrDefault();
                    string source_no = group_dt.Select(p => p.Field<string>("QUO_NO")).FirstOrDefault();
                    string source = group_dt.Select(p => p.Field<string>("SOURCE")).FirstOrDefault();
                    var item_model = h_Product_DetailVM.normal_Detail.Where(p => p.PART_NO == item_no).FirstOrDefault();
                    item_model.PRICE = price;
                    item_model.PRICE_PM = item_model.PRICE_PM > 0 ? item_model.PRICE_PM : price;
                    item_model.PRICE_HIS_H = price_HIS_H;
                    item_model.PRICE_HIS_L = price_HIS_L;
                    item_model.CURRENCY = currency;
                    item_model.RATE = (rate != null) ? (decimal)rate : 0;
                    item_model.SOURCE_NO = source_no;
                    item_model.SOURCE = source;
                }
            }

            h_Product_DetailVM.specail_Detail = H_Product_DetailManager.Get_Special_Sourcer(rsi_no, Employee.EmpNO, part_type);
            item_no_list = h_Product_DetailVM.specail_Detail.Where(p => p.ISMODIFY == "Y").Select(p => p.PART_NO).ToArray();
            ws_str = String.Join(",", item_no_list);
            if (String.IsNullOrEmpty(ws_str))
                ws_result = "No Data Found";
            else
                ws_result = "No Data Found";
                //ws_result = service.get_quo_info_for_RSI(ws_str);

            if (!ws_result.Equals("No Data Found") && !ws_result.Equals("Not Authorized"))
            {
                DataTable dt_specail = XMLHelper.GetDataSetFromXML(ws_result);
                var convert_dt = dt_specail.AsEnumerable().GroupBy(p => p.Field<string>("ITEM_NO"));
                foreach (var group_dt in convert_dt)
                {
                    var item_no = group_dt.Key;
                    decimal price = group_dt.Where(p => p.Field<string>("TYPE") == "N").Select(p => p.Field<decimal>("USD_PRICE")).FirstOrDefault();
                    decimal price_HIS_H = group_dt.Where(p => p.Field<string>("TYPE") == "H").Select(p => p.Field<decimal>("USD_PRICE")).FirstOrDefault();
                    decimal price_HIS_L = group_dt.Where(p => p.Field<string>("TYPE") == "L").Select(p => p.Field<decimal>("USD_PRICE")).FirstOrDefault();
                    string currency = group_dt.Select(p => p.Field<string>("ORI_CURRENCY")).FirstOrDefault();
                    decimal? rate = group_dt.Select(p => p.Field<decimal?>("RATE")).FirstOrDefault();
                    string source_no = group_dt.Select(p => p.Field<string>("QUO_NO")).FirstOrDefault();
                    string source = group_dt.Select(p => p.Field<string>("SOURCE")).FirstOrDefault();
                    var item_model = h_Product_DetailVM.specail_Detail.Where(p => p.PART_NO == item_no).ToList();
                    foreach (var model in item_model)
                    {
                        model.PRICE = price;
                        model.PRICE_PM = model.PRICE_PM > 0 ? model.PRICE_PM : price;
                        model.PRICE_HIS_H = price_HIS_H;
                        model.PRICE_HIS_L = price_HIS_L;
                        model.CURRENCY = currency;
                        model.RATE = (rate != null) ? (decimal)rate : 0;
                        model.SOURCE_NO = source_no;
                        model.SOURCE = source;
                    }
                }
            }
            return View(h_Product_DetailVM);
        }

        [HttpPost]
        public ActionResult Save(List<H_Product_DetailEntity> h_Product_Details, string part_type)
        {
            var save_sn = h_Product_Details.Where(p => p.ISASSIGNER == "Y").Select(p => p.SN).ToArray();
            var rsi_no = h_Product_Details.Select(p => p.RSI_NO).FirstOrDefault();
            var header_referer = Request.Headers["Referer"].ToString();

            var normals = H_Product_DetailManager.Get_Normal_Sourcer_View(rsi_no.ToString(), Employee.EmpNO, part_type).ToList();
            if (header_referer.Contains("ProductReview"))
                normals = H_Product_DetailManager.ProductReview_GetNormalParts(rsi_no.ToString(), part_type, Employee.EmpNO).ToList();
            normals = GetPriceTrend(normals).ToList();
            var result = CheckModels(normals, h_Product_Details.Where(p => p.MTL_TYPE.Equals("Normal")).ToList());
            var specials = H_Product_DetailManager.Get_Special_Sourcer(rsi_no.ToString(), Employee.EmpNO, part_type).ToList();
            if (header_referer.Contains("ProductReview"))
                specials = H_Product_DetailManager.ProductReview_GetSpecialParts(rsi_no.ToString(), part_type, Employee.EmpNO).ToList();
            specials = GetPriceTrend(specials).ToList();
            result.AddRange(CheckModels(specials, h_Product_Details.Where(p => p.MTL_TYPE.Equals("Special")).ToList()));

            foreach (var h_Product_Detail in result)
            {
                H_Product_DetailManager.Update_Product_Detail(h_Product_Detail); //更新rsi_h_product_detail資料
                if (h_Product_Detail.SN != 0)
                {
                    if (h_Product_Detail.pricetrend != null)
                    {
                        foreach (var price in h_Product_Detail.pricetrend)
                        {
                            H_Product_DetailManager.UpdatePriceTrend(price.RSI_NO, price.SN.ToString(), price.ID.ToString(), price.PRICE.ToString()); //更新price_trend資料
                        }
                    }
                }
            }

            return Json(String.Join(",", save_sn));
        }

        public ActionResult ManagerDetail(string rsi_no, string part_type, string bu)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ManagerDetail(string rsi_no, string groupid)
        {
            var model = H_Product_DetailManager.GetSourcerManagerDetail(rsi_no, Employee.EmpNO, groupid);
            model = GetPriceTrend(model);
            return Json(model);
        }

        [HttpPost]
        public ActionResult ExportExcel(string rsi_no, string projectname, string bu, string part_type, string phase_id, string form_no, string page_name)
        {
            var exportexcelconfig = RSI_ConfigEntityDAL.GetExportExcelConfig(bu, null, phase_id);
            var showprice = exportexcelconfig.AsEnumerable().Where(p => p.Field<string>("Attribute6") == "Y").Any();
            var pariceidentity = exportexcelconfig.Rows[0]["Attribute4"].ToString();

            var bom = H_Product_DetailManager.GetBOMTableDataForSourcer(rsi_no, part_type, phase_id, Employee.EmpNO);
            var normalPartsData = H_Product_DetailManager.Get_Normal_Sourcer_View(rsi_no, Employee.EmpNO, part_type);
            normalPartsData = GetPriceTrend(normalPartsData);
            if(showprice) normalPartsData = ComputeSubTotalByConfig(normalPartsData, exportexcelconfig);

            var groupIdList = H_Product_DetailManager.GetGroupID(rsi_no);
            Dictionary<string, IList<H_Product_DetailEntity>> specialPartsData = new Dictionary<string, IList<H_Product_DetailEntity>>();
            foreach (var group_id in groupIdList)
            {
                var data = H_Product_DetailManager.GetProductDetailForExportExcelSpecialData(rsi_no, group_id, Employee.EmpNO);
                data = GetPriceTrend(data);
                specialPartsData.Add(group_id, data);
            }

            string fileName = String.Format("{0}_{1}_{2}.xlsx", projectname, Employee.GetLogidID(Employee.EmpNO), DateTime.Now.ToString("yyyyMMddhhmmss"));
            var fileStream = this.ExchangeExcel(rsi_no, bom, normalPartsData, specialPartsData, exportexcelconfig);
            //return File(fileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

            var dt_rmscrol = RSI_ConfigEntityDAL.GetRMSConrol();
            if (dt_rmscrol.Rows.Count > 0)
            {
                using (FLM.FLMServiceClient client = new FLM.FLMServiceClient())
                {
                    client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["FLMUser"];
                    client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["FLMPWD"];
                    string sysId = ConfigurationManager.AppSettings["FLMSYS"];  //系統編號[必選]，FLM註冊或本系統名稱

                    //相應的權限（V：查看、P：打印、E：編輯、C：複製、S：保存）
                    var flmidentity = exportexcelconfig.Rows[0]["attribute7"].ToString();
                    var flmvaliddays = exportexcelconfig.Rows[0]["attribute8"].ToString();
                    var attribute9 = exportexcelconfig.Rows[0]["attribute9"].ToString();
                    var userAndRightsList = new List<string[]>();
                    userAndRightsList.Add(new string[2] { Employee.EmpNO, flmidentity });

                    if (!string.IsNullOrEmpty(attribute9))
                    {
                        var boss = RSI_ConfigEntityDAL.GetRMSUser(Employee.EmpNO, attribute9);
                        if (phase_id.Equals("50"))
                        {
                            DataTable dt_SourcerEmpNo = RSI_ConfigEntityDAL.GetExportExcelForSourcerEmpNo(rsi_no, part_type);
                            var sourcer_emp_no = dt_SourcerEmpNo.Rows[0]["PRODUCT_SOURCER"].ToString();
                            boss = RSI_ConfigEntityDAL.GetRMSUser(sourcer_emp_no, attribute9);
                        }

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
                    string username = ConfigurationManager.AppSettings["FLMUser"];
                    string userpassword = ConfigurationManager.AppSettings["FLMPWD"];
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

        [Auto_Identify]
        public ActionResult ProductReview()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        [HttpPost]
        public ActionResult ProductReview_GetNormalParts(string rsi_no, string bu, string part_type, string phase_id)
        {
            var normalParts = H_Product_DetailManager.ProductReview_GetNormalParts(rsi_no, part_type, Employee.EmpNO);
            normalParts = GetPriceTrend(normalParts);
            var exportexcelconfig = RSI_ConfigEntityDAL.GetExportExcelConfig(bu, part_type, phase_id);
            if (exportexcelconfig.Rows.Count > 0)
            {
                var pariceIdentity = exportexcelconfig.Rows[0]["Attribute4"].ToString();
                if (pariceIdentity.Equals("Part_No"))
                    normalParts = ComputeSubTotalForPartNo(normalParts);
                if (pariceIdentity.Equals("Material_Parts"))
                    normalParts = ComputeSubTotalForMaterialPart(normalParts);
                if (pariceIdentity.Equals("Parts_Group"))
                    normalParts = ComputeSubTotalForPartsGroup(normalParts);
                if (pariceIdentity.Equals("Material_Group"))
                    normalParts = ComputeSubTotalForMaterialGroup(normalParts);
            }
            return Json(normalParts);
        }

        [HttpPost]
        public ActionResult ProductReview_GetSpecialParts(string rsi_no, string part_type, string group_id)
        {
            var specialParts = H_Product_DetailManager.ProductReview_GetSpecialParts(rsi_no, part_type, Employee.EmpNO);
            if (!String.IsNullOrEmpty(group_id))
                specialParts = specialParts.Where(p => p.GROUP_ID == group_id).ToList();
            specialParts = GetPriceTrend(specialParts);
            return Json(specialParts);
        }

        [HttpPost]
        public ActionResult ProductReview_ReSetAssign(string rsi_no, string reassign, string part_type, string bu, string phase_id, string projectname)
        {
            H_Form_ApproveManager.ReassignProductSourcerMember(reassign, rsi_no, part_type, phase_id, bu, Employee.EmpNO);
            SendMailForReassign(phase_id, rsi_no, part_type, bu, projectname, reassign, Employee.EmpNO);
            return Json(true);
        }

        [HttpPost]
        public ActionResult ModalEdit_Save(H_Product_DetailEntity model)
        {
            H_Product_DetailManager.ModalEdit_Save(model);
            return Json(true);
        }

        [HttpPost]
        public ActionResult DownloadExcel(string rsi_no, string part_type)
        {
            var normalPartsData = H_Product_DetailManager.Get_Normal_Sourcer_View(rsi_no, Employee.EmpNO, part_type);
            normalPartsData = GetPriceTrend(normalPartsData).Where(p => p.ISMODIFY == "Y").ToList();
            var stream = DownloadExcel(normalPartsData);
            var filename = string.Concat(rsi_no, "_", DateTime.Now.ToString("yyyyMMddhhmmss"), ".xlsm");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", filename);
        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    try
                    {
                        #region Read Excel Data, using NPOI
                        IWorkbook wb = null;
                        wb = new XSSFWorkbook(file.InputStream);
                        ISheet ws = wb.GetSheet("sheet1");

                        DataTable dt = new DataTable();
                        var columnTotal = Convert.ToInt32(ws.GetRow(0).LastCellNum);
                        dt.Columns.Add(new DataColumn("startTarget"));
                        for (int i = 1; i < columnTotal; i++)
                        {
                            dt.Columns.Add(new DataColumn(ws.GetRow(0).GetCell(i).StringCellValue));
                        }

                        var rowTotal = ws.LastRowNum;
                        for (int i = 1; i <= rowTotal; i++)
                        {
                            ICell cell = ws.GetRow(i).GetCell(0);
                            var startTaget = cell != null ? cell.StringCellValue : string.Empty;
                            if (!string.IsNullOrEmpty(startTaget))
                            {
                                DataRow row = dt.NewRow();
                                for (int j = 1; j < columnTotal; j++)
                                {
                                    cell = ws.GetRow(i).GetCell(j);
                                    cell.SetCellType(CellType.String);
                                    var value = cell != null ? cell.StringCellValue : string.Empty;
                                    row[j] = value;
                                }
                                dt.Rows.Add(row);
                            }
                        }
                        #endregion

                        #region Convert data structure, DataTable to List<H_Product_DetailEntity>
                        var model = dt.AsEnumerable().Select(p => {
                            Decimal? price_pm = null;
                            if (!string.IsNullOrEmpty(p.Field<string>("PRICE_PM")))
                                price_pm = Convert.ToDecimal(p.Field<string>("PRICE_PM"));
                            Decimal? moq = null;
                            if (!string.IsNullOrEmpty(p.Field<string>("MOQ")))
                                moq = Convert.ToDecimal(p.Field<string>("MOQ"));
                            Decimal? mockup = null;
                            if (!string.IsNullOrEmpty(p.Field<string>("MOCKUP")))
                                mockup = Convert.ToDecimal(p.Field<string>("MOCKUP"));
                            Decimal? tooling = null;
                            if (!string.IsNullOrEmpty(p.Field<string>("TOOLING")))
                                tooling = Convert.ToDecimal(p.Field<string>("TOOLING"));
                            Decimal? fpca_pcba = null;
                            if (!string.IsNullOrEmpty(p.Field<string>("FPCA_PCBA")))
                                fpca_pcba = Convert.ToDecimal(p.Field<string>("FPCA_PCBA"));
                            var result = new H_Product_DetailEntity()
                            {
                                RSI_NO = Convert.ToDecimal(p.Field<string>("RSI_NO")),
                                MTL_GROUP = p.Field<string>("MTL_GROUP"),
                                PARTS_GROUP = p.Field<string>("PARTS_GROUP"),
                                MTL_PARTS = p.Field<string>("MTL_PARTS"),
                                PART_NO = p.Field<string>("PART_NO"),
                                ENGLISH_NAME = p.Field<string>("ENGLISH_NAME"),
                                PART_DESC = p.Field<string>("PART_DESC"),
                                PART_SPEC = p.Field<string>("PART_SPEC"),
                                PART_TYPE = p.Field<string>("PART_TYPE"),
                                USAGE = Convert.ToDecimal(p.Field<string>("USAGE")),
                                ISASSIGNER = p.Field<string>("ISASSIGNER") == "" ? "N" : p.Field<string>("ISASSIGNER"),
                                //PRICE = Convert.ToDecimal(p.Field<string>("PRICE")),
                                PRICE_PM = price_pm,
                                MOQ = moq,
                                MOCKUP = mockup,
                                TOOLING = tooling,
                                FPCA_PCBA = fpca_pcba,
                                ISCALCULATE = p.Field<string>("ISCALCULATE") == "" ? "N" : p.Field<string>("ISCALCULATE"),
                                SOURCE = p.Field<string>("SOURCE"),
                                SOURCE_NO = p.Field<string>("SOURCE_NO"),
                                REMARK = p.Field<string>("REMARK"),
                                REMARK_PM = p.Field<string>("REMARK_PM"),
                                REMARK_PUR = p.Field<string>("REMARK_PUR"),
                                SN = Convert.ToDecimal(p.Field<string>("SN")),
                                MTL_TYPE = p.Field<string>("MTL_TYPE")
                            };
                            return result;
                        }).ToList();
                        DataTable pricetrend = RSI_ConfigEntityDAL.GetPriceTrend(model.FirstOrDefault().RSI_NO.ToString());
                        foreach (var item in model)
                        {
                            item.pricetrend = new List<H_Product_PriceTrend>();
                            foreach (DataRow row in pricetrend.Rows)
                            {
                                H_Product_PriceTrend trend = new H_Product_PriceTrend();
                                trend.RSI_NO = item.RSI_NO.ToString();
                                trend.SN = item.SN;
                                trend.ID = Convert.ToDecimal(row["ID"].ToString());
                                var price = dt.AsEnumerable().Where(p => p.Field<string>("SN") == item.SN.ToString())
                                    .Select(p => new { value = p.Field<string>(row["DESCRIPTION"].ToString()) }).FirstOrDefault();

                                if (string.IsNullOrEmpty(price.value))
                                    continue;

                                trend.PRICE = Convert.ToDecimal(price.value);
                                item.pricetrend.Add(trend);
                            }
                        }
                        #endregion

                        #region Update data
                        foreach (var h_Product_Detail in model)
                        {
                            H_Product_DetailManager.Update_Product_Detail(h_Product_Detail);
                            if (h_Product_Detail.SN != 0)
                            {
                                if (h_Product_Detail.pricetrend != null)
                                {
                                    foreach (var price in h_Product_Detail.pricetrend)
                                    {
                                        H_Product_DetailManager.UpdatePriceTrend(price.RSI_NO, price.SN.ToString(), price.ID.ToString(), price.PRICE.ToString());
                                    }
                                }
                            }

                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        return Json(false);
                    }
                }
            }
            else
                return Json(false);
            return Json(true);
        }

        public ActionResult MaterialSourcerReassign()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MaterialSourcerReassign(string rsi_no, string part_type)
        {
            var data = H_Product_DetailManager.Get_Sourcer_Reassign_View(rsi_no, Employee.EmpNO, part_type).ToList();
            return Json(data.Where(p => p.ISMODIFY == "Y" && (p.ISAPPROVED != "Y" || string.IsNullOrEmpty(p.ISAPPROVED))).ToList());
        }

        [HttpPost]
        public ActionResult MaterSourcerReassignBindding(string bu, string mtl_parts)
        {
            var result = RSI_ConfigEntityDAL.GetMaterialSourcerReassignBindding(bu, mtl_parts).ToList();
            return Json(result);
        }

        [HttpPost]
        public ActionResult MaterialSourcerReassignSubmit(string reassign, string bu, string mtl_part, string rsi_no, string part_type, string phase_id, string projectname, string sn)
        {
            try
            {
                var list_sn = sn.Split(',');
                foreach (var item in list_sn)
                {
                    H_Form_ApproveManager.Update_IsAssigner_IsApproved_N(rsi_no, part_type, item, reassign); //處理rsi_h_product_detail
                    if (mtl_part == "RD DEFINE")
                        H_Form_ApproveManager.ReassignForRDDefineAuth(bu, reassign, mtl_part, rsi_no, part_type, phase_id, item, Employee.EmpNO); //處理authority
                }
                H_Form_ApproveManager.ReassignApprove(bu, reassign, mtl_part, rsi_no, part_type, phase_id, Employee.EmpNO); //更新Approve表
                SendMailForReassign(phase_id, rsi_no, part_type, bu, projectname, reassign, Employee.EmpNO);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult PartNoApprovedForSubmit(string rsi_no, string part_type)
        {
            DataTable dt = H_Product_DetailManager.PartNoApprovedForSubmit(rsi_no, part_type, Employee.EmpNO);
            return Json(dt.AsEnumerable().Select(p => new { part_type = p["PART_TYPE"], mtl_type = p["MTL_TYPE"], mtl_parts = p["MTL_PARTS"], cnt = p["CNT"] }).ToList());
        }

        [Auto_Identify]
        public ActionResult ProductDocumentReview()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        public ActionResult ProductValuation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProductValuation(string rsi_no, string part_type, string phase_id, string sn)
        {
            var data = H_Product_DetailManager.ProductValuation(rsi_no, part_type, phase_id, sn, Employee.EmpNO).ToList();
            return Json(data);
        }

        [HttpPost]
        public ActionResult ProductValuationSave(List<H_Product_DetailEntity> data)
        {
            try
            {
                foreach (var model in data)
                {
                    H_Product_DetailManager.ProductValuationSave(model);
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        //Export Execl funciton
        private byte[] ExchangeExcel(string rsi_no, IList<H_Product_DetailEntity> bom, IList<H_Product_DetailEntity> normalPartsDatas, Dictionary<string, IList<H_Product_DetailEntity>> specialPartsData, DataTable exportexcelconfig)
        {
            var showPrice = exportexcelconfig.AsEnumerable().Where(p => p.Field<string>("Attribute6") == "Y").Any();
            var priceidentity = exportexcelconfig.Rows[0]["Attribute4"].ToString();
            DataTable pricetrend = RSI_ConfigEntityDAL.GetPriceTrend(rsi_no);
            //IWorkbook wb = new HSSFWorkbook();
            //ISheet ws;

            ////建立Excel 2007檔案
            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = null;

            #region BOM Sheet
            if (bom.Count() > 0)
            {
                ws = wb.CreateSheet("BOM");
                ws.CreateRow(0);//第一行為欄位名稱

                #region 設定excel第一行
                XSSFCellStyle cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                IFont fontstyle = wb.CreateFont();
                fontstyle.Color = IndexedColors.White.Index;
                fontstyle.Boldweight = (short)FontBoldWeight.Bold;
                fontstyle.FontName = "微軟正黑體";
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
                ws.GetRow(0).CreateCell(4).SetCellValue("Parts Group");
                ws.GetRow(0).GetCell(4).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(5).SetCellValue("Material Parts");
                ws.GetRow(0).GetCell(5).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(6).SetCellValue("Part Type");
                ws.GetRow(0).GetCell(6).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(7).SetCellValue("English Name");
                ws.GetRow(0).GetCell(7).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(8).SetCellValue("Part Description");
                ws.GetRow(0).GetCell(8).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(9).SetCellValue("Part Spec");
                ws.GetRow(0).GetCell(9).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(10).SetCellValue("Usage");
                ws.GetRow(0).GetCell(10).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(11).SetCellValue("Unit");
                ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(12).SetCellValue("Remark");
                ws.GetRow(0).GetCell(12).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(13).SetCellValue("Upload File");
                ws.GetRow(0).GetCell(13).CellStyle = cellstyle;
                ws.CreateFreezePane(0, 1);
                #endregion

                for (int i = 0; i < bom.Count(); i++)
                {
                    cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                    fontstyle = wb.CreateFont();
                    fontstyle.FontName = "微軟正黑體";
                    cellstyle.SetFont(fontstyle);

                    ws.CreateRow(i + 1);
                    #region 綁定資料
                    ws.GetRow(i + 1).CreateCell(0).SetCellValue(bom[i].DISPLAYPARTNO.ToString());
                    ws.GetRow(i + 1).GetCell(0).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(1).SetCellValue(bom[i].BOM_LEVEL.ToString());
                    ws.GetRow(i + 1).GetCell(1).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(2).SetCellValue(bom[i].PART_LEVEL);
                    ws.GetRow(i + 1).GetCell(2).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(3).SetCellValue(bom[i].MTL_GROUP);
                    ws.GetRow(i + 1).GetCell(3).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(4).SetCellValue(bom[i].PARTS_GROUP);
                    ws.GetRow(i + 1).GetCell(4).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(5).SetCellValue(bom[i].MTL_PARTS);
                    ws.GetRow(i + 1).GetCell(5).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(6).SetCellValue(bom[i].PART_TYPE);
                    ws.GetRow(i + 1).GetCell(6).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(7).SetCellValue(bom[i].ENGLISH_NAME);
                    ws.GetRow(i + 1).GetCell(7).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(8).SetCellValue(bom[i].PART_DESC);
                    ws.GetRow(i + 1).GetCell(8).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(9).SetCellValue(bom[i].PART_SPEC);
                    ws.GetRow(i + 1).GetCell(9).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(10).SetCellValue((double)bom[i].USAGE);
                    ws.GetRow(i + 1).CreateCell(11).SetCellValue(bom[i].PART_UNIT);
                    ws.GetRow(i + 1).GetCell(11).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(12).SetCellValue(bom[i].REMARK);
                    ws.GetRow(i + 1).GetCell(12).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(13).SetCellValue(bom[i].FILE_STATUS);
                    ws.GetRow(i + 1).GetCell(13).CellStyle = cellstyle;
                    #endregion
                }
            }

            for (var i = 0; i < ws.GetRow(0).Cells.Count; i++)
            {
                ws.AutoSizeColumn(i);
            }
            #endregion

            #region NormalParts Sheet
            if (normalPartsDatas.Count() > 0)
            {
                ws = wb.CreateSheet("Normal Parts");
                ws.CreateRow(0);//第一行為欄位名稱

                #region 設定excel第一行
                XSSFCellStyle cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                IFont fontstyle = wb.CreateFont();
                fontstyle.Color = IndexedColors.White.Index;
                fontstyle.Boldweight = (short)FontBoldWeight.Bold;
                fontstyle.FontName = "微軟正黑體";
                cellstyle.SetFont(fontstyle);
                var color = new XSSFColor(new byte[] { 60, 141, 188 });
                cellstyle.FillForegroundColorColor = color;
                cellstyle.FillPattern = FillPattern.SolidForeground;

                ws.GetRow(0).CreateCell(0).SetCellValue("Material Group");
                ws.GetRow(0).GetCell(0).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(1).SetCellValue("Parts Group");
                ws.GetRow(0).GetCell(1).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(2).SetCellValue("Material Parts");
                ws.GetRow(0).GetCell(2).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(3).SetCellValue("Parent Part No");
                ws.GetRow(0).GetCell(3).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(4).SetCellValue("Part No");
                ws.GetRow(0).GetCell(4).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(5).SetCellValue("English Name");
                ws.GetRow(0).GetCell(5).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(6).SetCellValue("Part Description");
                ws.GetRow(0).GetCell(6).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(7).SetCellValue("Part Spec");
                ws.GetRow(0).GetCell(7).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(8).SetCellValue("Part Type");
                ws.GetRow(0).GetCell(8).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(9).SetCellValue("Usage");
                ws.GetRow(0).GetCell(9).CellStyle = cellstyle;
                if (showPrice)
                {
                    ws.GetRow(0).CreateCell(10).SetCellValue("Sourcer Price");
                    ws.GetRow(0).GetCell(10).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(11).SetCellValue("PM Price");
                    ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(12).SetCellValue("MOQ");
                    ws.GetRow(0).GetCell(12).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(13).SetCellValue("MOCKUP");
                    ws.GetRow(0).GetCell(13).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(14).SetCellValue("TOOLING");
                    ws.GetRow(0).GetCell(14).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(15).SetCellValue("FPCA_PCBA");
                    ws.GetRow(0).GetCell(15).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(16).SetCellValue("Valuation");
                    ws.GetRow(0).GetCell(16).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(17).SetCellValue("Sourcer Amount");
                    ws.GetRow(0).GetCell(17).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(18).SetCellValue("PM Amount");
                    ws.GetRow(0).GetCell(18).CellStyle = cellstyle;
                    for (var i = 0; i < pricetrend.Rows.Count; i++)
                    {
                        ws.GetRow(0).CreateCell(19 + i).SetCellValue(pricetrend.Rows[i]["description"].ToString());
                        ws.GetRow(0).GetCell(19 + i).CellStyle = cellstyle;
                    }
                    ws.GetRow(0).CreateCell(19 + pricetrend.Rows.Count).SetCellValue("Price Source");
                    ws.GetRow(0).GetCell(19 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(20 + pricetrend.Rows.Count).SetCellValue("Quo No");
                    ws.GetRow(0).GetCell(20 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(21 + pricetrend.Rows.Count).SetCellValue("RD Remark");
                    ws.GetRow(0).GetCell(21 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(22 + pricetrend.Rows.Count).SetCellValue("To PM Remark");
                    ws.GetRow(0).GetCell(22 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(23 + pricetrend.Rows.Count).SetCellValue("Sorcer Remark");
                    ws.GetRow(0).GetCell(23 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(24 + pricetrend.Rows.Count).SetCellValue("Download File");
                    ws.GetRow(0).GetCell(24 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(25 + pricetrend.Rows.Count).SetCellValue("Updated date");
                    ws.GetRow(0).GetCell(25 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(26 + pricetrend.Rows.Count).SetCellValue("Updated by");
                    ws.GetRow(0).GetCell(26 + pricetrend.Rows.Count).CellStyle = cellstyle;
                }
                else
                {
                    ws.GetRow(0).CreateCell(10).SetCellValue("RD Remark");
                    ws.GetRow(0).GetCell(10).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(11).SetCellValue("To PM Remark");
                    ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(12).SetCellValue("Sorcer Remark");
                    ws.GetRow(0).GetCell(12).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(13).SetCellValue("Download File");
                    ws.GetRow(0).GetCell(13).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(14).SetCellValue("Updated date");
                    ws.GetRow(0).GetCell(14).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(15).SetCellValue("Updated by");
                    ws.GetRow(0).GetCell(15).CellStyle = cellstyle;
                }
                ws.CreateFreezePane(0, 1);
                #endregion

                for (int i = 0; i < normalPartsDatas.Count(); i++)
                {
                    cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                    fontstyle = wb.CreateFont();
                    fontstyle.FontName = "微軟正黑體";
                    cellstyle.SetFont(fontstyle);

                    ws.CreateRow(i + 1);
                    #region 綁定資料
                    ws.GetRow(i + 1).CreateCell(0).SetCellValue(normalPartsDatas[i].MTL_GROUP);
                    ws.GetRow(i + 1).GetCell(0).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(1).SetCellValue(normalPartsDatas[i].PARTS_GROUP);
                    ws.GetRow(i + 1).GetCell(1).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(2).SetCellValue(normalPartsDatas[i].MTL_PARTS);
                    ws.GetRow(i + 1).GetCell(2).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(3).SetCellValue(normalPartsDatas[i].PARTNUMBER_PARENT);
                    ws.GetRow(i + 1).GetCell(3).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(4).SetCellValue(normalPartsDatas[i].PART_NO);
                    ws.GetRow(i + 1).GetCell(4).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(5).SetCellValue(normalPartsDatas[i].ENGLISH_NAME);
                    ws.GetRow(i + 1).GetCell(5).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(6).SetCellValue(normalPartsDatas[i].PART_DESC);
                    ws.GetRow(i + 1).GetCell(6).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(7).SetCellValue(normalPartsDatas[i].PART_SPEC);
                    ws.GetRow(i + 1).GetCell(7).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(8).SetCellValue(normalPartsDatas[i].PART_TYPE);
                    ws.GetRow(i + 1).GetCell(8).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(9).SetCellValue((double)normalPartsDatas[i].USAGE);
                    ws.GetRow(i + 1).GetCell(9).CellStyle = cellstyle;
                    if (showPrice)
                    {
                        priceidentity = exportexcelconfig.AsEnumerable().Where(p => p.Field<string>("ATTRIBUTE2") == normalPartsDatas[i].PART_TYPE)
                            .Select(p => p.Field<string>("ATTRIBUTE4")).FirstOrDefault();
                        bool priceshow = IsShowPrice(priceidentity, normalPartsDatas[i]);

                        if (normalPartsDatas[i].PRICE != null)
                            ws.GetRow(i + 1).CreateCell(10).SetCellValue((double)normalPartsDatas[i].PRICE);
                        else
                            ws.GetRow(i + 1).CreateCell(10).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(10).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(10).SetCellValue(string.Empty);

                        if (normalPartsDatas[i].PRICE_PM != null)
                            ws.GetRow(i + 1).CreateCell(11).SetCellValue((double)normalPartsDatas[i].PRICE_PM);
                        else
                            ws.GetRow(i + 1).CreateCell(11).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(11).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(11).SetCellValue(string.Empty);

                        if(normalPartsDatas[i].MOQ != null)
                            ws.GetRow(i + 1).CreateCell(12).SetCellValue((double)normalPartsDatas[i].MOQ);
                        else
                            ws.GetRow(i + 1).CreateCell(12).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(12).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(12).SetCellValue(string.Empty);

                        if(normalPartsDatas[i].MOCKUP != null)
                            ws.GetRow(i + 1).CreateCell(13).SetCellValue((double)normalPartsDatas[i].MOCKUP);
                        else
                            ws.GetRow(i + 1).CreateCell(13).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(13).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(13).SetCellValue(string.Empty);

                        if(normalPartsDatas[i].TOOLING != null)
                            ws.GetRow(i + 1).CreateCell(14).SetCellValue((double)normalPartsDatas[i].TOOLING);
                        else
                            ws.GetRow(i + 1).CreateCell(14).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(14).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(14).SetCellValue(string.Empty);

                        if (normalPartsDatas[i].FPCA_PCBA != null)
                            ws.GetRow(i + 1).CreateCell(15).SetCellValue((double)normalPartsDatas[i].FPCA_PCBA);
                        else
                            ws.GetRow(i + 1).CreateCell(15).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(15).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(15).SetCellValue(string.Empty);

                        ws.GetRow(i + 1).CreateCell(16).SetCellValue(normalPartsDatas[i].ISCALCULATE);
                        ws.GetRow(i + 1).GetCell(16).CellStyle = cellstyle;

                        if(normalPartsDatas[i].SOURCERAMOUNT != null)
                            ws.GetRow(i + 1).CreateCell(17).SetCellValue((double)normalPartsDatas[i].SOURCERAMOUNT);
                        else
                            ws.GetRow(i + 1).CreateCell(17).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(17).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(17).SetCellValue(string.Empty);

                        if(normalPartsDatas[i].PMAMOUNT != null)
                            ws.GetRow(i + 1).CreateCell(18).SetCellValue((double)normalPartsDatas[i].PMAMOUNT);
                        else
                            ws.GetRow(i + 1).CreateCell(18).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(18).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(18).SetCellValue(string.Empty);

                        for (var j = 0; j < pricetrend.Rows.Count; j++)
                        {
                            decimal? temp_number = null;
                            if (normalPartsDatas[i].pricetrend != null)
                            {
                                var temp_price = normalPartsDatas[i].pricetrend.Where(p => p.ID.ToString() == pricetrend.Rows[j]["ID"].ToString()).FirstOrDefault();
                                if (temp_price != null)
                                    temp_number = temp_price.PRICE;
                            }

                            if (temp_number != null)
                                ws.GetRow(i + 1).CreateCell(19 + j).SetCellValue((double)temp_number);
                            else
                                ws.GetRow(i + 1).CreateCell(19 + j).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(19 + j).CellStyle = cellstyle;
                            if (!priceshow) ws.GetRow(i + 1).GetCell(19 + j).SetCellValue(string.Empty);
                        }
                        ws.GetRow(i + 1).CreateCell(19 + pricetrend.Rows.Count).SetCellValue(priceshow ? normalPartsDatas[i].SOURCE : String.Empty);
                        ws.GetRow(i + 1).GetCell(19 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(20 + pricetrend.Rows.Count).SetCellValue(priceshow ? normalPartsDatas[i].SOURCE_NO : String.Empty);
                        ws.GetRow(i + 1).GetCell(20 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(21 + pricetrend.Rows.Count).SetCellValue(normalPartsDatas[i].REMARK);
                        ws.GetRow(i + 1).GetCell(21 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(22 + pricetrend.Rows.Count).SetCellValue(normalPartsDatas[i].REMARK_PM);
                        ws.GetRow(i + 1).GetCell(22 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(23 + pricetrend.Rows.Count).SetCellValue(normalPartsDatas[i].REMARK_PUR);
                        ws.GetRow(i + 1).GetCell(23 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(24 + pricetrend.Rows.Count).SetCellValue(normalPartsDatas[i].FILE_STATUS);
                        ws.GetRow(i + 1).GetCell(24 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(25 + pricetrend.Rows.Count).SetCellValue(DateTime.MinValue == normalPartsDatas[i].UPDATED_DATE ? string.Empty : normalPartsDatas[i].UPDATED_DATE.ToString("yyyy/MM/dd"));
                        ws.GetRow(i + 1).GetCell(25 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(26 + pricetrend.Rows.Count).SetCellValue(normalPartsDatas[i].UPDATED_BY);
                        ws.GetRow(i + 1).GetCell(26 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    }
                    else
                    {
                        ws.GetRow(i + 1).CreateCell(10).SetCellValue(normalPartsDatas[i].REMARK);
                        ws.GetRow(i + 1).GetCell(10).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(11).SetCellValue(normalPartsDatas[i].REMARK_PM);
                        ws.GetRow(i + 1).GetCell(11).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(12).SetCellValue(normalPartsDatas[i].REMARK_PUR);
                        ws.GetRow(i + 1).GetCell(12).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(13).SetCellValue(normalPartsDatas[i].FILE_STATUS);
                        ws.GetRow(i + 1).GetCell(13).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(14).SetCellValue(DateTime.MinValue == normalPartsDatas[i].UPDATED_DATE ? string.Empty : normalPartsDatas[i].UPDATED_DATE.ToString("yyyy/MM/dd"));
                        ws.GetRow(i + 1).GetCell(14).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(15).SetCellValue(normalPartsDatas[i].UPDATED_BY);
                        ws.GetRow(i + 1).GetCell(15).CellStyle = cellstyle;
                    }
                    #endregion
                }

                for (var i = 0; i < ws.GetRow(0).Cells.Count; i++)
                {
                    ws.AutoSizeColumn(i);
                }

                //開啟篩選功能,避免RMS加密限制複製功能後無法使用篩選
                if (showPrice)
                    ws.SetAutoFilter(new CellRangeAddress(0, 1, 0, 26 + pricetrend.Rows.Count));
                else
                    ws.SetAutoFilter(new CellRangeAddress(0, 1, 0, 15));
            }
            #endregion

            #region SpecialParts Sheet
            if (specialPartsData.Count() > 0)
            {
                int x = 1;
                foreach (var itemData in specialPartsData)
                {
                    var sheetName = H_Product_DetailManager.GetGroupName(rsi_no, itemData.Key);
                    ws = wb.CreateSheet(String.IsNullOrEmpty(sheetName) ? string.Format("Portfolio{0}", x.ToString()) : sheetName);
                    x++;
                    ws.CreateRow(0);//第一行為欄位名稱
                    #region 設定excel第一行
                    XSSFCellStyle cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                    IFont fontstyle = wb.CreateFont();
                    fontstyle.Color = IndexedColors.White.Index;
                    fontstyle.Boldweight = (short)FontBoldWeight.Bold;
                    fontstyle.FontName = "微軟正黑體";
                    cellstyle.SetFont(fontstyle);
                    var color = new XSSFColor(new byte[] { 60, 141, 188 });
                    cellstyle.FillForegroundColorColor = color;
                    cellstyle.FillPattern = FillPattern.SolidForeground;

                    ws.GetRow(0).CreateCell(0).SetCellValue("Parent Part No");
                    ws.GetRow(0).GetCell(0).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(1).SetCellValue("Part No");
                    ws.GetRow(0).GetCell(1).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(2).SetCellValue("Level");
                    ws.GetRow(0).GetCell(2).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(3).SetCellValue("Part Level");
                    ws.GetRow(0).GetCell(3).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(4).SetCellValue("Material Group");
                    ws.GetRow(0).GetCell(4).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(5).SetCellValue("Material Parts");
                    ws.GetRow(0).GetCell(5).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(6).SetCellValue("Part Type");
                    ws.GetRow(0).GetCell(6).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(7).SetCellValue("English Name");
                    ws.GetRow(0).GetCell(7).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(8).SetCellValue("Part Description");
                    ws.GetRow(0).GetCell(8).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(9).SetCellValue("Part Spec");
                    ws.GetRow(0).GetCell(9).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(10).SetCellValue("Usage");
                    ws.GetRow(0).GetCell(10).CellStyle = cellstyle;
                    if (showPrice)
                    {
                        ws.GetRow(0).CreateCell(11).SetCellValue("Sourcer Price");
                        ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(12).SetCellValue("PM Price");
                        ws.GetRow(0).GetCell(12).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(13).SetCellValue("MOQ");
                        ws.GetRow(0).GetCell(13).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(14).SetCellValue("MOCKUP");
                        ws.GetRow(0).GetCell(14).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(15).SetCellValue("TOOLING");
                        ws.GetRow(0).GetCell(15).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(16).SetCellValue("FPCA_PCBA");
                        ws.GetRow(0).GetCell(16).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(17).SetCellValue("Valuation");
                        ws.GetRow(0).GetCell(17).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(18).SetCellValue("Sourcer Amount");
                        ws.GetRow(0).GetCell(18).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(19).SetCellValue("PM Amount");
                        ws.GetRow(0).GetCell(19).CellStyle = cellstyle;
                        for (var a = 0; a < pricetrend.Rows.Count; a++)
                        {
                            ws.GetRow(0).CreateCell(20 + a).SetCellValue(pricetrend.Rows[a]["description"].ToString());
                            ws.GetRow(0).GetCell(20 + a).CellStyle = cellstyle;
                        }
                        ws.GetRow(0).CreateCell(20 + pricetrend.Rows.Count).SetCellValue("Price Source");
                        ws.GetRow(0).GetCell(20 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(21 + pricetrend.Rows.Count).SetCellValue("Quo No");
                        ws.GetRow(0).GetCell(21 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(22 + pricetrend.Rows.Count).SetCellValue("RD Remark");
                        ws.GetRow(0).GetCell(22 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(23 + pricetrend.Rows.Count).SetCellValue("To PM Remark");
                        ws.GetRow(0).GetCell(23 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(24 + pricetrend.Rows.Count).SetCellValue("Sorcer Remark");
                        ws.GetRow(0).GetCell(24 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(25 + pricetrend.Rows.Count).SetCellValue("Download File");
                        ws.GetRow(0).GetCell(25 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(26 + pricetrend.Rows.Count).SetCellValue("Updated date");
                        ws.GetRow(0).GetCell(26 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(27 + pricetrend.Rows.Count).SetCellValue("Updated by");
                        ws.GetRow(0).GetCell(27 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    }
                    else
                    {
                        ws.GetRow(0).CreateCell(11).SetCellValue("RD Remark");
                        ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(12).SetCellValue("To PM Remark");
                        ws.GetRow(0).GetCell(12).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(13).SetCellValue("Sorcer Remark");
                        ws.GetRow(0).GetCell(13).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(14).SetCellValue("Download File");
                        ws.GetRow(0).GetCell(14).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(15).SetCellValue("Updated date");
                        ws.GetRow(0).GetCell(15).CellStyle = cellstyle;
                        ws.GetRow(0).CreateCell(16).SetCellValue("Updated by");
                        ws.GetRow(0).GetCell(16).CellStyle = cellstyle;
                    }
                    ws.CreateFreezePane(0, 1);
                    #endregion

                    int i = 0;
                    foreach (var item in itemData.Value)
                    {
                        cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                        fontstyle = wb.CreateFont();
                        fontstyle.FontName = "微軟正黑體";
                        cellstyle.SetFont(fontstyle);

                        if (!String.IsNullOrEmpty(item.MTL_TYPE))
                        {
                            if (item.MTL_TYPE.Equals("Special"))
                            {
                                color = new XSSFColor(new byte[] { 243, 156, 18 });
                                cellstyle.FillForegroundColorColor = color;
                                cellstyle.FillPattern = FillPattern.SolidForeground;
                            }
                        }

                        ws.CreateRow(i + 1);
                        #region 綁定資料
                        ws.GetRow(i + 1).CreateCell(0).SetCellValue(item.PARTNUMBER_PARENT);
                        ws.GetRow(i + 1).GetCell(0).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(1).SetCellValue(item.DISPLAYPARTNO ?? String.Empty);
                        ws.GetRow(i + 1).GetCell(1).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(2).SetCellValue(item.BOM_LEVEL.ToString());
                        ws.GetRow(i + 1).GetCell(2).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(3).SetCellValue(item.PART_LEVEL);
                        ws.GetRow(i + 1).GetCell(3).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(4).SetCellValue(item.MTL_GROUP);
                        ws.GetRow(i + 1).GetCell(4).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(5).SetCellValue(item.MTL_PARTS);
                        ws.GetRow(i + 1).GetCell(5).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(6).SetCellValue(item.PART_TYPE);
                        ws.GetRow(i + 1).GetCell(6).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(7).SetCellValue(item.ENGLISH_NAME);
                        ws.GetRow(i + 1).GetCell(7).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(8).SetCellValue(item.PART_DESC);
                        ws.GetRow(i + 1).GetCell(8).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(9).SetCellValue(item.PART_SPEC);
                        ws.GetRow(i + 1).GetCell(9).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(10).SetCellValue((double)item.USAGE);
                        ws.GetRow(i + 1).GetCell(10).CellStyle = cellstyle;
                        if (showPrice)
                        {
                            priceidentity = exportexcelconfig.AsEnumerable().Where(p => p.Field<string>("ATTRIBUTE2") == itemData.Value[i].PART_TYPE)
                                            .Select(p => p.Field<string>("ATTRIBUTE4")).FirstOrDefault();
                            bool priceshow = IsShowPrice(priceidentity, item);
                            if(item.PRICE != null)
                                ws.GetRow(i + 1).CreateCell(11).SetCellValue((double)item.PRICE);
                            else
                                ws.GetRow(i + 1).CreateCell(11).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(11).CellStyle = cellstyle;
                            if (!priceshow) ws.GetRow(i + 1).GetCell(11).SetCellValue(string.Empty);

                            if(item.PRICE_PM != null)
                                ws.GetRow(i + 1).CreateCell(12).SetCellValue((double)item.PRICE_PM);
                            else
                                ws.GetRow(i + 1).CreateCell(12).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(12).CellStyle = cellstyle;
                            if (!priceshow) ws.GetRow(i + 1).GetCell(12).SetCellValue(string.Empty);

                            if(item.MOQ != null)
                                ws.GetRow(i + 1).CreateCell(13).SetCellValue((double)item.MOQ);
                            else
                                ws.GetRow(i + 1).CreateCell(13).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(13).CellStyle = cellstyle;
                            if (!priceshow) ws.GetRow(i + 1).GetCell(13).SetCellValue(string.Empty);

                            if(item.MOCKUP != null)
                                ws.GetRow(i + 1).CreateCell(14).SetCellValue((double)item.MOCKUP);
                            else
                                ws.GetRow(i + 1).CreateCell(14).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(14).CellStyle = cellstyle;
                            if (!priceshow) ws.GetRow(i + 1).GetCell(14).SetCellValue(string.Empty);

                            if(item.TOOLING != null)
                                ws.GetRow(i + 1).CreateCell(15).SetCellValue((double)item.TOOLING);
                            else
                                ws.GetRow(i + 1).CreateCell(15).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(15).CellStyle = cellstyle;

                            if (item.FPCA_PCBA != null)
                                ws.GetRow(i + 1).CreateCell(16).SetCellValue((double)item.FPCA_PCBA);
                            else
                                ws.GetRow(i + 1).CreateCell(16).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(16).CellStyle = cellstyle;
                            if (!priceshow) ws.GetRow(i + 1).GetCell(16).SetCellValue(string.Empty);

                            ws.GetRow(i + 1).CreateCell(17).SetCellValue(item.ISCALCULATE);
                            ws.GetRow(i + 1).GetCell(17).CellStyle = cellstyle;

                            if(item.SOURCERAMOUNT != null)
                                ws.GetRow(i + 1).CreateCell(18).SetCellValue((double)item.SOURCERAMOUNT);
                            else
                                ws.GetRow(i + 1).CreateCell(18).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(18).CellStyle = cellstyle;
                            if (!priceshow) ws.GetRow(i + 1).GetCell(18).SetCellValue(string.Empty);

                            if(item.PMAMOUNT != null)
                                ws.GetRow(i + 1).CreateCell(19).SetCellValue((double)item.PMAMOUNT);
                            else
                                ws.GetRow(i + 1).CreateCell(19).SetCellValue(string.Empty);
                            ws.GetRow(i + 1).GetCell(19).CellStyle = cellstyle;
                            if (!priceshow) ws.GetRow(i + 1).GetCell(19).SetCellValue(string.Empty);

                            for (var j = 0; j < pricetrend.Rows.Count; j++)
                            {
                                decimal? temp_number = null;
                                if (item.pricetrend != null)
                                {
                                    var temp_price = item.pricetrend.Where(p => p.ID.ToString() == pricetrend.Rows[j]["ID"].ToString()).FirstOrDefault();
                                    if (temp_price != null)
                                        temp_number = temp_price.PRICE;
                                }
                                if(temp_number != null)
                                    ws.GetRow(i + 1).CreateCell(20 + j).SetCellValue((double)temp_number);
                                else
                                    ws.GetRow(i + 1).CreateCell(20 + j).SetCellValue(string.Empty);
                                ws.GetRow(i + 1).GetCell(20 + j).CellStyle = cellstyle;
                                if(!priceshow) ws.GetRow(i + 1).GetCell(20 + j).SetCellValue(string.Empty);
                            }
                            ws.GetRow(i + 1).CreateCell(20 + pricetrend.Rows.Count).SetCellValue(priceshow ? item.SOURCE : String.Empty);
                            ws.GetRow(i + 1).GetCell(20 + pricetrend.Rows.Count).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(21 + pricetrend.Rows.Count).SetCellValue(priceshow ? item.SOURCE_NO : String.Empty);
                            ws.GetRow(i + 1).GetCell(21 + pricetrend.Rows.Count).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(22 + pricetrend.Rows.Count).SetCellValue(item.REMARK);
                            ws.GetRow(i + 1).GetCell(22 + pricetrend.Rows.Count).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(23 + pricetrend.Rows.Count).SetCellValue(item.REMARK_PM);
                            ws.GetRow(i + 1).GetCell(23 + pricetrend.Rows.Count).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(24 + pricetrend.Rows.Count).SetCellValue(item.REMARK_PUR);
                            ws.GetRow(i + 1).GetCell(24 + pricetrend.Rows.Count).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(25 + pricetrend.Rows.Count).SetCellValue(item.FILE_STATUS);
                            ws.GetRow(i + 1).GetCell(25 + pricetrend.Rows.Count).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(26 + pricetrend.Rows.Count).SetCellValue((DateTime.MinValue == item.UPDATED_DATE) ? String.Empty : item.UPDATED_DATE.ToString("yyyy/MM/dd"));
                            ws.GetRow(i + 1).GetCell(26 + pricetrend.Rows.Count).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(27 + pricetrend.Rows.Count).SetCellValue(item.UPDATED_BY);
                            ws.GetRow(i + 1).GetCell(27 + pricetrend.Rows.Count).CellStyle = cellstyle;
                        }
                        else
                        {
                            ws.GetRow(i + 1).CreateCell(11).SetCellValue(item.REMARK);
                            ws.GetRow(i + 1).GetCell(11).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(12).SetCellValue(item.REMARK_PM);
                            ws.GetRow(i + 1).GetCell(12).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(13).SetCellValue(item.REMARK_PUR);
                            ws.GetRow(i + 1).GetCell(13).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(14).SetCellValue(item.FILE_STATUS);
                            ws.GetRow(i + 1).GetCell(14).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(15).SetCellValue((DateTime.MinValue == item.UPDATED_DATE) ? String.Empty : item.UPDATED_DATE.ToString("yyyy/MM/dd"));
                            ws.GetRow(i + 1).GetCell(15).CellStyle = cellstyle;
                            ws.GetRow(i + 1).CreateCell(16).SetCellValue(item.UPDATED_BY);
                            ws.GetRow(i + 1).GetCell(16).CellStyle = cellstyle;
                        }
                        #endregion
                        i++;
                    }

                    for (var c = 0; c < ws.GetRow(0).Cells.Count; c++)
                    {
                        ws.AutoSizeColumn(c);
                    }
                }
            }
            #endregion

            MemoryStream stream = new MemoryStream();
            wb.Write(stream);
            wb = null;
            return stream.ToArray();
        }

        private IList<H_Product_DetailEntity> ComputeSubTotalForPartNo(IList<H_Product_DetailEntity> model)
        {
            var total = new H_Product_DetailEntity()
            {
                MTL_GROUP = "總計",
                ISMODIFY = "Y",
                PRICE = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE),
                PRICE_PM = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE_PM),
                MOQ = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOQ),
                MOCKUP = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOCKUP),
                TOOLING = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.TOOLING),
                SOURCERAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.SOURCERAMOUNT),
                PMAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PMAMOUNT)
            };

            var temp_pricetrend = new List<H_Product_PriceTrend>();
            var temp_totalpricetrend = model.Where(p => p.ISMODIFY.Equals("Y") && p.pricetrend.Any()).Select(p => p.pricetrend).ToList();
            foreach (var item in temp_totalpricetrend)
            {
                temp_pricetrend.AddRange(item);
                temp_pricetrend = temp_pricetrend.GroupBy(p => p.ID).Select(p => new H_Product_PriceTrend()
                {
                    ID = p.Key,
                    RSI_NO = p.Select(a => a.RSI_NO).FirstOrDefault(),
                    SN = 0,
                    PRICE = p.Sum(a => a.PRICE)
                }).ToList();
            }

            total.pricetrend = temp_pricetrend;

            model.Add(total);
            return model;
        }

        private IList<H_Product_DetailEntity> ComputeSubTotalForMaterialPart(IList<H_Product_DetailEntity> model)
        {
            decimal temp_sourcerprice = 0, temp_pmprice = 0, temp_moq = 0, temp_mockup = 0, temp_tooling = 0;
            decimal temp_sourceramount = 0, temp_pmamount = 0;
            List<H_Product_PriceTrend> temp_pricetrend = new List<H_Product_PriceTrend>();
            string temp_ismodify = "N";
            var temp_mtlpart = String.Empty;
            IList<H_Product_DetailEntity> result = new List<H_Product_DetailEntity>();
            H_Product_DetailEntity subtotal = null;
            foreach (var item in model)
            {
                if (temp_mtlpart != item.MTL_PARTS && result.Count > 0)
                {
                    subtotal = new H_Product_DetailEntity()
                    {
                        MTL_PARTS = "小計",
                        PRICE = temp_sourcerprice,
                        PRICE_PM = temp_pmprice,
                        MOQ = temp_moq,
                        MOCKUP = temp_mockup,
                        TOOLING = temp_tooling,
                        SOURCERAMOUNT = temp_sourceramount,
                        PMAMOUNT = temp_pmamount,
                        ISMODIFY = temp_ismodify,
                        pricetrend = temp_pricetrend
                    };
                    result.Add(subtotal);
                    temp_sourcerprice = 0;
                    temp_pmprice = 0;
                    temp_moq = 0;
                    temp_mockup = 0;
                    temp_tooling = 0;
                    temp_sourceramount = 0;
                    temp_pmamount = 0;
                    temp_ismodify = "N";
                    temp_pricetrend = new List<H_Product_PriceTrend>();
                }

                temp_mtlpart = item.MTL_PARTS;
                if (!String.IsNullOrEmpty(item.ISMODIFY))
                {
                    if (item.ISMODIFY.Equals("Y"))
                    {
                        temp_sourcerprice += item.PRICE ?? 0;
                        temp_pmprice += item.PRICE_PM ?? 0;
                        temp_moq += item.MOQ ?? 0;
                        temp_mockup += item.MOCKUP ?? 0;
                        temp_tooling += item.TOOLING ?? 0;
                        temp_sourceramount += item.SOURCERAMOUNT ?? 0;
                        temp_pmamount += item.PMAMOUNT ?? 0;
                        temp_ismodify = "Y";

                        if (item.pricetrend.Any())
                        {
                            temp_pricetrend.AddRange(item.pricetrend);
                            temp_pricetrend = temp_pricetrend.GroupBy(p => p.ID).Select(p => new H_Product_PriceTrend()
                            {
                                ID = p.Key,
                                RSI_NO = p.Select(a => a.RSI_NO).FirstOrDefault(),
                                SN = 0,
                                PRICE = p.Sum(a => a.PRICE)
                            }).ToList();
                        }
                    }
                }
                result.Add(item);
            }

            subtotal = new H_Product_DetailEntity()
            {
                MTL_PARTS = "小計",
                PRICE = temp_sourcerprice,
                PRICE_PM = temp_pmprice,
                MOQ = temp_moq,
                MOCKUP = temp_mockup,
                TOOLING = temp_tooling,
                SOURCERAMOUNT = temp_sourceramount,
                PMAMOUNT = temp_pmamount,
                ISMODIFY = temp_ismodify,
                pricetrend = temp_pricetrend
            };
            result.Add(subtotal);

            temp_pricetrend = new List<H_Product_PriceTrend>();
            var temp_totalpricetrend = model.Where(p => p.ISMODIFY.Equals("Y") && p.pricetrend.Any()).Select(p => p.pricetrend).ToList();
            foreach (var item in temp_totalpricetrend)
            {
                temp_pricetrend.AddRange(item);
                temp_pricetrend = temp_pricetrend.GroupBy(p => p.ID).Select(p => new H_Product_PriceTrend()
                {
                    ID = p.Key,
                    RSI_NO = p.Select(a => a.RSI_NO).FirstOrDefault(),
                    SN = 0,
                    PRICE = p.Sum(a => a.PRICE)
                }).ToList();
            }

            var total = new H_Product_DetailEntity()
            {
                MTL_GROUP = "總計",
                ISMODIFY = "Y",
                PRICE = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE),
                PRICE_PM = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE_PM),
                MOQ = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOQ),
                MOCKUP = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOCKUP),
                TOOLING = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.TOOLING),
                SOURCERAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.SOURCERAMOUNT),
                PMAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PMAMOUNT),
                pricetrend = temp_pricetrend
            };
            result.Add(total);
            return result;
        }

        private IList<H_Product_DetailEntity> ComputeSubTotalForPartsGroup(IList<H_Product_DetailEntity> model)
        {
            decimal temp_sourcerprice = 0, temp_pmprice = 0, temp_moq = 0, temp_mockup = 0, temp_tooling = 0;
            decimal temp_sourceramount = 0, temp_pmamount = 0;
            List<H_Product_PriceTrend> temp_pricetrend = new List<H_Product_PriceTrend>();
            var temp_partsgroup = String.Empty;
            IList<H_Product_DetailEntity> result = new List<H_Product_DetailEntity>();
            H_Product_DetailEntity subtotal = null;
            foreach (var item in model)
            {
                if (temp_partsgroup != item.PARTS_GROUP && result.Count > 0)
                {
                    subtotal = new H_Product_DetailEntity()
                    {
                        PARTS_GROUP = "小計",
                        ISMODIFY = "Y",
                        PRICE = temp_sourcerprice,
                        PRICE_PM = temp_pmprice,
                        MOQ = temp_moq,
                        MOCKUP = temp_mockup,
                        TOOLING = temp_tooling,
                        SOURCERAMOUNT = temp_sourceramount,
                        PMAMOUNT = temp_pmamount,
                        pricetrend = temp_pricetrend
                    };
                    result.Add(subtotal);
                    temp_sourcerprice = 0;
                    temp_pmprice = 0;
                    temp_moq = 0;
                    temp_mockup = 0;
                    temp_tooling = 0;
                    temp_sourceramount = 0;
                    temp_pmamount = 0;
                    temp_pricetrend = new List<H_Product_PriceTrend>();
                }

                temp_partsgroup = item.PARTS_GROUP;
                if (!String.IsNullOrEmpty(item.ISMODIFY))
                {
                    if (item.ISMODIFY.Equals("Y"))
                    {
                        temp_sourcerprice += item.PRICE ?? 0;
                        temp_pmprice += item.PRICE_PM ?? 0;
                        temp_moq += item.MOQ ?? 0;
                        temp_mockup += item.MOCKUP ?? 0;
                        temp_tooling += item.TOOLING ?? 0;
                        temp_sourceramount += item.SOURCERAMOUNT ?? 0;
                        temp_pmamount += item.PMAMOUNT ?? 0;

                        if (item.pricetrend.Any())
                        {
                            temp_pricetrend.AddRange(item.pricetrend);
                            temp_pricetrend = temp_pricetrend.GroupBy(p => p.ID).Select(p => new H_Product_PriceTrend()
                            {
                                ID = p.Key,
                                RSI_NO = p.Select(a => a.RSI_NO).FirstOrDefault(),
                                SN = 0,
                                PRICE = p.Sum(a => a.PRICE)
                            }).ToList();
                        }
                    }
                }
                result.Add(item);
            }

            subtotal = new H_Product_DetailEntity()
            {
                PARTS_GROUP = "小計",
                ISMODIFY = "Y",
                PRICE = temp_sourcerprice,
                PRICE_PM = temp_pmprice,
                MOQ = temp_moq,
                MOCKUP = temp_mockup,
                TOOLING = temp_tooling,
                SOURCERAMOUNT = temp_sourceramount,
                PMAMOUNT = temp_pmamount,
                pricetrend = temp_pricetrend
            };
            result.Add(subtotal);

            temp_pricetrend = new List<H_Product_PriceTrend>();
            var temp_totalpricetrend = model.Where(p => p.ISMODIFY.Equals("Y") && p.pricetrend.Any()).Select(p => p.pricetrend).ToList();
            foreach (var item in temp_totalpricetrend)
            {
                temp_pricetrend.AddRange(item);
                temp_pricetrend = temp_pricetrend.GroupBy(p => p.ID).Select(p => new H_Product_PriceTrend()
                {
                    ID = p.Key,
                    RSI_NO = p.Select(a => a.RSI_NO).FirstOrDefault(),
                    SN = 0,
                    PRICE = p.Sum(a => a.PRICE)
                }).ToList();
            }

            var total = new H_Product_DetailEntity()
            {
                MTL_GROUP = "總計",
                ISMODIFY = "Y",
                PRICE = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE),
                PRICE_PM = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE_PM),
                MOQ = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOQ),
                MOCKUP = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOCKUP),
                TOOLING = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.TOOLING),
                SOURCERAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.SOURCERAMOUNT),
                PMAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PMAMOUNT),
                pricetrend = temp_pricetrend
            };
            result.Add(total);
            return result;
        }

        private IList<H_Product_DetailEntity> ComputeSubTotalForMaterialGroup(IList<H_Product_DetailEntity> model)
        {
            decimal temp_sourcerprice = 0, temp_pmprice = 0, temp_moq = 0, temp_mockup = 0, temp_tooling = 0;
            decimal temp_sourceramount = 0, temp_pmamount = 0;
            List<H_Product_PriceTrend> temp_pricetrend = new List<H_Product_PriceTrend>();
            string temp_ismodify = "N";
            var temp_mtlgroup = String.Empty;
            IList<H_Product_DetailEntity> result = new List<H_Product_DetailEntity>();
            H_Product_DetailEntity subtotal = null;
            foreach (var item in model)
            {
                if (temp_mtlgroup != item.MTL_GROUP && result.Count > 0)
                {
                    subtotal = new H_Product_DetailEntity()
                    {
                        MTL_GROUP = "小計",
                        ISMODIFY = temp_ismodify,
                        PRICE = temp_sourcerprice,
                        PRICE_PM = temp_pmprice,
                        MOQ = temp_moq,
                        MOCKUP = temp_mockup,
                        TOOLING = temp_tooling,
                        SOURCERAMOUNT = temp_sourceramount,
                        PMAMOUNT = temp_pmamount,
                        pricetrend = temp_pricetrend
                    };
                    result.Add(subtotal);
                    temp_sourcerprice = 0;
                    temp_pmprice = 0;
                    temp_moq = 0;
                    temp_mockup = 0;
                    temp_tooling = 0;
                    temp_sourceramount = 0;
                    temp_pmamount = 0;
                    temp_ismodify = "N";
                    temp_pricetrend = new List<H_Product_PriceTrend>();
                }

                temp_mtlgroup = item.MTL_GROUP;
                if (!String.IsNullOrEmpty(item.ISMODIFY))
                {
                    if (item.ISMODIFY.Equals("Y"))
                    {
                        temp_sourcerprice += item.PRICE ?? 0;
                        temp_pmprice += item.PRICE_PM ?? 0;
                        temp_moq += item.MOQ ?? 0;
                        temp_mockup += item.MOCKUP ?? 0;
                        temp_tooling += item.TOOLING ?? 0;
                        temp_sourceramount += item.SOURCERAMOUNT ?? 0;
                        temp_pmamount += item.PMAMOUNT ?? 0;
                        temp_ismodify = "Y";

                        if (item.pricetrend.Any())
                        {
                            temp_pricetrend.AddRange(item.pricetrend);
                            temp_pricetrend = temp_pricetrend.GroupBy(p => p.ID).Select(p => new H_Product_PriceTrend()
                            {
                                ID = p.Key,
                                RSI_NO = p.Select(a => a.RSI_NO).FirstOrDefault(),
                                SN = 0,
                                PRICE = p.Sum(a => a.PRICE)
                            }).ToList();
                        }
                    }
                }
                result.Add(item);
            }

            subtotal = new H_Product_DetailEntity()
            {
                MTL_GROUP = "小計",
                ISMODIFY = temp_ismodify,
                PRICE = temp_sourcerprice,
                PRICE_PM = temp_pmprice,
                MOQ = temp_moq,
                MOCKUP = temp_mockup,
                TOOLING = temp_tooling,
                SOURCERAMOUNT = temp_sourceramount,
                PMAMOUNT = temp_pmamount,
                pricetrend = temp_pricetrend
            };
            result.Add(subtotal);

            temp_pricetrend = new List<H_Product_PriceTrend>();
            var temp_totalpricetrend = model.Where(p => p.ISMODIFY.Equals("Y") && p.pricetrend.Any()).Select(p => p.pricetrend).ToList();
            foreach (var item in temp_totalpricetrend)
            {
                temp_pricetrend.AddRange(item);
                temp_pricetrend = temp_pricetrend.GroupBy(p => p.ID).Select(p => new H_Product_PriceTrend()
                {
                    ID = p.Key,
                    RSI_NO = p.Select(a => a.RSI_NO).FirstOrDefault(),
                    SN = 0,
                    PRICE = p.Sum(a => a.PRICE)
                }).ToList();
            }

            var total = new H_Product_DetailEntity()
            {
                MTL_GROUP = "總計",
                ISMODIFY = "Y",
                PRICE = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE),
                PRICE_PM = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE_PM),
                MOQ = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOQ),
                MOCKUP = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOCKUP),
                TOOLING = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.TOOLING),
                SOURCERAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.SOURCERAMOUNT),
                PMAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PMAMOUNT),
                pricetrend = temp_pricetrend
            };
            result.Add(total);
            return result;
        }

        private IList<H_Product_DetailEntity> ComputeSubTotalByConfig(IList<H_Product_DetailEntity> model, DataTable config)
        {
            List<H_Product_DetailEntity> result = new List<H_Product_DetailEntity>();
            string temp_mtl_group = string.Empty, temp_parts_group = string.Empty, temp_mtl_parts = string.Empty, temp_part_type = string.Empty;
            decimal temp_sourcerprice = 0, temp_pmprice = 0, temp_moq = 0, temp_mockup = 0, temp_tooling = 0;
            decimal temp_sourceramount = 0, temp_pmamount = 0;
            List<H_Product_PriceTrend> temp_pricetrend = new List<H_Product_PriceTrend>();
            var temp_price_config = string.Empty;
            foreach (var item in model)
            {
                if (!string.IsNullOrEmpty(temp_price_config))
                {
                    if ((temp_price_config.Equals("Material_Group") && temp_mtl_group != item.MTL_GROUP) ||
                        (temp_price_config.Equals("Parts_Group") && temp_parts_group != item.PARTS_GROUP) ||
                        (temp_price_config.Equals("Material_Parts") && temp_mtl_parts != item.MTL_PARTS))
                    {
                        H_Product_DetailEntity subtotal = new H_Product_DetailEntity();
                        subtotal.MTL_GROUP = "小計";
                        subtotal.PART_TYPE = temp_part_type;
                        subtotal.ISMODIFY = "Y";
                        subtotal.PRICE = temp_sourcerprice;
                        subtotal.PRICE_PM = temp_pmprice;
                        subtotal.MOQ = temp_moq;
                        subtotal.MOCKUP = temp_mockup;
                        subtotal.TOOLING = temp_tooling;
                        subtotal.SOURCERAMOUNT = temp_sourceramount;
                        subtotal.PMAMOUNT = temp_pmamount;
                        if (temp_pricetrend.Any())
                        {
                            subtotal.pricetrend = temp_pricetrend.GroupBy(p => p.ID)
                                .Select(p => new H_Product_PriceTrend()
                                {
                                    RSI_NO = p.Select(x => x.RSI_NO).FirstOrDefault(),
                                    ID = p.Key,
                                    PRICE = p.Select(x => x.PRICE).Sum(),
                                    SN = 0
                                }).ToList();
                        }
                        result.Add(subtotal);

                        temp_sourcerprice = 0;
                        temp_pmprice = 0;
                        temp_moq = 0;
                        temp_mockup = 0;
                        temp_tooling = 0;
                        temp_sourceramount = 0;
                        temp_pmamount = 0;
                        temp_pricetrend = new List<H_Product_PriceTrend>();
                    }
                }

                temp_price_config = config.AsEnumerable().Where(p => p.Field<string>("ATTRIBUTE2") == item.PART_TYPE)
                    .Select(p => p.Field<string>("ATTRIBUTE4")).FirstOrDefault();
                temp_mtl_group = item.MTL_GROUP;
                temp_parts_group = item.PARTS_GROUP;
                temp_mtl_parts = item.MTL_PARTS;
                temp_part_type = item.PART_TYPE;

                if (item.ISMODIFY.Equals("Y"))
                {
                    temp_sourcerprice += item.PRICE ?? 0;
                    temp_pmprice += item.PRICE_PM ?? 0;
                    temp_moq += item.MOQ ?? 0;
                    temp_mockup += item.MOCKUP ?? 0;
                    temp_tooling += item.TOOLING ?? 0;
                    temp_sourceramount += item.SOURCERAMOUNT ?? 0;
                    temp_pmamount += item.PMAMOUNT ?? 0;

                    if(item.pricetrend.Any())
                        temp_pricetrend.AddRange(item.pricetrend);
                }
                result.Add(item);
            }

            if (!temp_price_config.Equals("Part_No")) 
            {
                H_Product_DetailEntity subtotal = new H_Product_DetailEntity();
                subtotal.MTL_GROUP = "小計";
                subtotal.PART_TYPE = temp_part_type;
                subtotal.ISMODIFY = "Y";
                subtotal.PRICE = temp_sourcerprice;
                subtotal.PRICE_PM = temp_pmprice;
                subtotal.MOQ = temp_moq;
                subtotal.MOCKUP = temp_mockup;
                subtotal.TOOLING = temp_tooling;
                subtotal.SOURCERAMOUNT = temp_sourceramount;
                subtotal.PMAMOUNT = temp_pmamount;
                if (temp_pricetrend.Any())
                {
                    subtotal.pricetrend = temp_pricetrend.GroupBy(p => p.ID)
                        .Select(p => new H_Product_PriceTrend()
                        {
                            RSI_NO = p.Select(x => x.RSI_NO).FirstOrDefault(),
                            ID = p.Key,
                            PRICE = p.Select(x => x.PRICE).Sum(),
                            SN = 0
                        }).ToList();
                }

                result.Add(subtotal);
            }

            H_Product_DetailEntity total = new H_Product_DetailEntity();
            total.MTL_GROUP = "總計";
            total.PART_TYPE = temp_part_type;
            total.ISMODIFY = "Y";
            total.PRICE = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE);
            total.PRICE_PM = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PRICE_PM);
            total.MOQ = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOQ); 
            total.MOCKUP = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.MOCKUP); 
            total.TOOLING = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.TOOLING); 
            total.SOURCERAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.SOURCERAMOUNT); 
            total.PMAMOUNT = model.Where(p => p.ISMODIFY.Equals("Y")).Sum(p => p.PMAMOUNT);
            if (model.Select(p => p.pricetrend).Any())
            {
                temp_pricetrend = new List<H_Product_PriceTrend>();
                foreach (var pricetrend in model.Select(p => p.pricetrend).ToList())
                {
                    temp_pricetrend.AddRange(pricetrend);
                }
                total.pricetrend = temp_pricetrend.GroupBy(p => p.ID)
                    .Select(p => new H_Product_PriceTrend()
                    {
                        RSI_NO = p.Select(x => x.RSI_NO).FirstOrDefault(),
                        ID = p.Key,
                        PRICE = p.Select(x => x.PRICE).Sum(),
                        SN = 0
                    }).ToList();
            }
            result.Add(total);
            return result;
        }

        private bool IsShowPrice(string priceidentity, H_Product_DetailEntity model)
        {
            if (!model.ISMODIFY.Equals("Y"))
                return false;

            if (priceidentity.Equals("Part_No"))
                return true;

            var isMaterialParts = model.MTL_PARTS != null ? model.MTL_PARTS.Equals("小計") : false;
            var isPartsGroup = model.PARTS_GROUP != null ? model.PARTS_GROUP.Equals("小計") : false;
            var isMaterialGroup = model.MTL_GROUP != null ? model.MTL_GROUP.Equals("小計") || model.MTL_GROUP.Equals("總計") : false;
            return isMaterialParts || isPartsGroup || isMaterialGroup;
        }

        private IList<H_Product_DetailEntity> GetPriceTrend(IList<H_Product_DetailEntity> model)
        {
            foreach (var item in model)
            {
                item.pricetrend = H_Product_DetailManager.GetPriceTrend(item.RSI_NO, item.SN);
            }
            return model;
        }

        private List<H_Product_DetailEntity> CheckModels(List<H_Product_DetailEntity> oldModel, List<H_Product_DetailEntity> newModel)
        {
            List<H_Product_DetailEntity> result = new List<H_Product_DetailEntity>();
            var equals = true;

            foreach (var item in newModel)
            {
                if (item.SN == 0) continue;

                var oldModelItem = oldModel.Where(p => p.SN == item.SN && p.GROUP_ID == item.GROUP_ID).FirstOrDefault();
                if (!oldModelItem.ISASSIGNER.Equals(item.ISASSIGNER)) equals = false;
                if (!oldModelItem.PRICE_PM.Equals(item.PRICE_PM)) equals = false;
                if (!oldModelItem.MOQ.Equals(item.MOQ)) equals = false;
                if (!oldModelItem.MOCKUP.Equals(item.MOCKUP)) equals = false;
                if (!oldModelItem.TOOLING.Equals(item.TOOLING)) equals = false;
                if (!oldModelItem.FPCA_PCBA.Equals(item.FPCA_PCBA)) equals = false;
                if (!oldModelItem.ISCALCULATE.Equals(item.ISCALCULATE)) equals = false;
                if (item.pricetrend != null)
                {
                    if (oldModelItem.pricetrend.Count != item.pricetrend.Count) equals = false;
                }
                
                for (int i = 0; i < oldModelItem.pricetrend.Count(); i++)
                {
                    if (!oldModelItem.pricetrend[i].PRICE.Equals(item.pricetrend[i].PRICE)) equals = false;
                }
                if (String.IsNullOrEmpty(oldModelItem.REMARK_PM) && !String.IsNullOrEmpty(item.REMARK_PM)) equals = false;
                if (!String.IsNullOrEmpty(oldModelItem.REMARK_PM) && String.IsNullOrEmpty(item.REMARK_PM)) equals = false;
                if (!String.IsNullOrEmpty(oldModelItem.REMARK_PM) && !String.IsNullOrEmpty(item.REMARK_PM))
                {
                    if (oldModelItem.REMARK_PM.Equals(item.REMARK_PM)) equals = false;
                }
                if (String.IsNullOrEmpty(oldModelItem.REMARK_PUR) && !String.IsNullOrEmpty(item.REMARK_PUR)) equals = false;
                if (!String.IsNullOrEmpty(oldModelItem.REMARK_PUR) && String.IsNullOrEmpty(item.REMARK_PUR)) equals = false;
                if (!String.IsNullOrEmpty(oldModelItem.REMARK_PUR) && !String.IsNullOrEmpty(item.REMARK_PUR))
                {
                    if (oldModelItem.REMARK_PUR.Equals(item.REMARK_PUR)) equals = false;
                }

                if (!equals)
                    result.Add(item);

                equals = true;
            }

            return result;
        }

        private byte[] DownloadExcel(IList<H_Product_DetailEntity> productModel)
        {
            var model = productModel.Select(x => new
            {
                x.RSI_NO,
                x.MTL_GROUP,
                x.PARTS_GROUP,
                x.MTL_PARTS,
                x.PARTNUMBER_PARENT,
                x.PART_NO,
                x.ENGLISH_NAME,
                x.PART_DESC,
                x.PART_SPEC,
                x.PART_TYPE,
                x.USAGE,
                x.ISASSIGNER,
                x.PRICE,
                x.PRICE_PM,
                x.MOQ,
                x.MOCKUP,
                x.TOOLING,
                x.FPCA_PCBA,
                x.ISCALCULATE,
                x.SOURCERAMOUNT,
                x.PMAMOUNT,
                x.pricetrend,
                x.SOURCE,
                x.SOURCE_NO,
                x.REMARK,
                x.REMARK_PM,
                x.REMARK_PUR,
                x.SN,
                x.MTL_TYPE
            }).ToList();

            IWorkbook wb = null;

            using (var fs = new FileStream(Server.MapPath("~/File/sourcerIndexTemplate.xlsm"), FileMode.Open, FileAccess.ReadWrite))
            {
                wb = new XSSFWorkbook(fs);
            }
            ISheet ws = wb.GetSheet("sheet1");
            IRow row = ws.CreateRow(0);
            var properties = model.FirstOrDefault().GetType().GetProperties();
            DataTable pricetrend = RSI_ConfigEntityDAL.GetPriceTrend(model.FirstOrDefault().RSI_NO.ToString());
            int col = 1;
            XSSFCellStyle cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
            IFont fontstyle = wb.CreateFont();
            fontstyle.Color = IndexedColors.White.Index;
            fontstyle.Boldweight = (short)FontBoldWeight.Bold;
            fontstyle.FontName = "微軟正黑體";
            cellstyle.SetFont(fontstyle);
            var color = new XSSFColor(new byte[] { 60, 141, 188 });
            cellstyle.FillForegroundColorColor = color;
            cellstyle.FillPattern = FillPattern.SolidForeground;
            foreach (var property in properties)
            {
                var colName = property.Name;
                if (colName.Equals("pricetrend"))
                {
                    foreach (DataRow dt_row in pricetrend.Rows)
                    {
                        ws.GetRow(0).CreateCell(col).SetCellValue(dt_row["description"].ToString());
                        ws.GetRow(0).GetCell(col).CellStyle = cellstyle;
                        col++;
                    }
                }
                else
                {
                    ws.GetRow(0).CreateCell(col).SetCellValue(property.Name);
                    ws.GetRow(0).GetCell(col).CellStyle = cellstyle;
                    col++;
                }

            }

            int rowIndex = 1;
            
            foreach (var item in model)
            {
                ws.CreateRow(rowIndex);
                col = 1;
                foreach (var property in properties)
                {
                    var colName = property.Name;
                    var excelcheckreadonly = ExcelCheckReadOnly(colName);

                    cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                    color = new XSSFColor(new byte[] { 239, 248, 154 });
                    cellstyle.FillForegroundColorColor = color;
                    cellstyle.FillPattern = FillPattern.SolidForeground;
                    cellstyle.IsLocked = false;

                    if (colName.Equals("pricetrend"))
                    {
                        cellstyle.Alignment = HorizontalAlignment.Right;
                        if (item.pricetrend.Any())
                        {
                            foreach (var price in item.pricetrend)
                            {
                                ws.GetRow(rowIndex).CreateCell(col).SetCellValue(price.PRICE.ToString());
                                if (!excelcheckreadonly) ws.GetRow(rowIndex).GetCell(col).CellStyle = cellstyle;
                                col++;
                            }

                            for (int i = 0; i < pricetrend.Rows.Count - item.pricetrend.Count(); i++)
                            {
                                ws.GetRow(rowIndex).CreateCell(col).SetCellValue(string.Empty);
                                if (!excelcheckreadonly) ws.GetRow(rowIndex).GetCell(col).CellStyle = cellstyle;
                                col++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < pricetrend.Rows.Count; i++)
                            {
                                ws.GetRow(rowIndex).CreateCell(col).SetCellValue(string.Empty);
                                if (!excelcheckreadonly) ws.GetRow(rowIndex).GetCell(col).CellStyle = cellstyle;
                                col++;
                            }
                        }

                    }
                    else
                    {
                        if (colName.Equals("USAGE") || colName.Equals("PRICE") || colName.Equals("PRICE_PM") || colName.Equals("MOQ") || colName.Equals("MOCKUP")
                        || colName.Equals("TOOLING") || colName.Equals("FPCA_PCBA") || colName.Equals("SOURCERAMOUNT") || colName.Equals("PMAMOUNT"))
                            cellstyle.Alignment = HorizontalAlignment.Right;

                        var obj = property.GetValue(item);
                        ws.GetRow(rowIndex).CreateCell(col).SetCellValue(obj != null ? obj.ToString() : string.Empty);
                        if (!excelcheckreadonly) ws.GetRow(rowIndex).GetCell(col).CellStyle = cellstyle;

                        //if (colName.Equals("SOURCERAMOUNT"))
                        //    ws.GetRow(rowIndex).GetCell(col).CellFormula = string.Concat("M", rowIndex, "*", "K", rowIndex);

                        //if (colName.Equals("PMAMOUNT"))
                        //    ws.GetRow(rowIndex).GetCell(col).CellFormula = string.Concat("N", rowIndex, "*", "K", rowIndex);

                        col++;
                    }
                }

                rowIndex++;
            }

            for (int i = 0; i < col; i++)
            {
                ws.AutoSizeColumn(i);
            }

            //var dt_SigningNotice = RSI_ConfigEntityDAL.GetSigningNotice(bu, "RESEND", phase_id);
            ws.ProtectSheet("8888");
            ws.SetColumnHidden(0, true);
            ws.SetColumnHidden(col - 2, true);
            ws.SetColumnHidden(col - 1, true);
            ws.CreateFreezePane(7, 1);

            MemoryStream stream = new MemoryStream();
            wb.Write(stream);
            wb = null;
            return stream.ToArray();
        }

        private bool ExcelCheckReadOnly(string colName)
        {
            bool result = true;
            switch (colName)
            {
                case "ISASSIGNER":
                    result = false;
                    break;
                case "PRICE_PM":
                    result = false;
                    break;
                case "MOQ":
                    result = false;
                    break;
                case "MOCKUP":
                    result = false;
                    break;
                case "TOOLING":
                    result = false;
                    break;
                case "FPCA_PCBA":
                    result = false;
                    break;
                case "ISCALCULATE":
                    result = false;
                    break;
                case "pricetrend":
                    result = false;
                    break;
                case "REMARK_PM":
                    result = false;
                    break;
                case "REMARK_PUR":
                    result = false;
                    break;
                default:
                    break;
            }
            return result;
        }

        private void SendMailForReassign(string phase_id, string rsi_no, string part_type, string bu, string projectname, string reassign, string emp_no)
        {
            var sendMailCode = ConfigurationManager.AppSettings["SendMailCode"].ToString();
            using (IDS.MailSoapClient ids = new IDS.MailSoapClient())
            {
                var title = "【RSI】轉簽通知 : {0}";
                title = string.Format(title, string.Concat("[", bu, "] ", projectname, " [", part_type, "] "));
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
                                    RSI表單由{empInfo}轉簽給您簽核，資訊如下:<br/><br/>
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
                body = body.Replace("{empInfo}", ticketData.DepartmentId + "/" + Employee.EmpNO + " " + Employee.EmpName);
                body = body.Replace("{0}", "http://" + Request.Url.Host).Replace("{1}", bu).Replace("{2}", projectname).Replace("{3}", part_type);

                #region table Content 簽核資訊
                var tableData = DASHBOARD_LIST_VManager.GetDetailApprove(rsi_no, part_type);
                string tableContent = GetTableContent(tableData);
                #endregion

                body = body.Replace("{tableContent}", tableContent);

                var dt_mailto = RSI_ConfigEntityDAL.GetMail(reassign);
                var mailto = dt_mailto.Rows[0]["email"].ToString();
                var dt_mailcc = RSI_ConfigEntityDAL.GetMail(emp_no);
                var mailcc = dt_mailcc.Rows[0]["email"].ToString();

                ids.ManualSend_07(sendMailCode, mailto, mailcc, title, body);
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