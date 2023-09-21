using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
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
    public class PMController : Controller
    {
        // GET: PM
        public ActionResult Index()
        {
            return View();
        }

        [Auto_Identify]
        public ActionResult PLReview()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            H_Product_InfoEntity h_Product_Info = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();
            return View(h_Product_Info);
        }

        [HttpPost]
        public ActionResult PLReview_NormalParts(string rsi_no, string part_type)
        {
            return Json(H_Product_DetailManager.GetRDReviewNormalParts(rsi_no, part_type));
        }

        [HttpPost]
        public ActionResult PLReview_SpecialParts(string rsi_no, string group_id)
        {
            var model = H_Product_DetailManager.GetRDReviewSpecialParts(rsi_no);
            if (model.Any() && !String.IsNullOrEmpty(group_id))
                model = model.Where(p => p.GROUP_ID == group_id).ToList();
            return Json(model);
        }

        [Auto_Identify]
        public ActionResult Confirm()
        {
            var rsi_no = Validate.DecryptValue(Request.QueryString["rsi_no"]);
            var part_type = Validate.DecryptValue(Request.QueryString["part_type"]);
            ViewData["h_Product_Info"] = H_Product_InfoManager.GetH_Product_InfoEntities(rsi_no, part_type).FirstOrDefault();

            var groups_id = H_Product_DetailManager.GetGroupID(rsi_no);
            var model = new List<C_MTL_PARTSEntity>();
            if (groups_id.Count() == 0) {
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
            else {
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

        public ActionResult PortfolioDetail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PortfolioDetail(string rsi_no, string group_id)
        {
            var portfolio_detail = H_Product_DetailManager.Get_PM_Portfolio_Detail(rsi_no, group_id);
            return Json(portfolio_detail);
        }

        [HttpPost]
        public ActionResult ExportExcel(string rsi_no, string projectname, string bu, string part_type, string phase_id, string form_no, string page_name)
        {
            var exportexcelconfig = RSI_ConfigEntityDAL.GetExprotExcelConfigForConfrim(rsi_no);
            var normalPartsData = H_Product_DetailManager.Get_PMConfrim_ExportData(rsi_no);
            normalPartsData = GetPriceTrend(normalPartsData);
            normalPartsData = ComputeSubTotalByConfig(normalPartsData, exportexcelconfig);

            var groupIdList = H_Product_DetailManager.GetGroupID(rsi_no);
            Dictionary<string, IList<H_Product_DetailEntity>> specialPartsData = new Dictionary<string, IList<H_Product_DetailEntity>>();
            foreach (var group_id in groupIdList)
            {
                var data = H_Product_DetailManager.Get_PMConfirm_Special_ExportData(rsi_no, group_id);
                data = GetPriceTrend(data);
                specialPartsData.Add(group_id, data);
            }

            string fileName = String.Format("{0}_{1}_{2}.xlsx", projectname, Employee.GetLogidID(Employee.EmpNO), DateTime.Now.ToString("yyyyMMddhhmmss"));
            var fileStream = this.ExchangeExcel(rsi_no, normalPartsData, specialPartsData, exportexcelconfig);
            var config = RSI_ConfigEntityDAL.GetExportExcelConfig(bu, null, phase_id);
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
                    var flmidentity = config.Rows[0]["attribute7"].ToString();
                    var flmvaliddays = config.Rows[0]["attribute8"].ToString();
                    var attribute9 = config.Rows[0]["attribute9"].ToString();
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

        private IList<H_Product_DetailEntity> GetPriceTrend(IList<H_Product_DetailEntity> model)
        {
            foreach (var item in model)
            {
                item.pricetrend = H_Product_DetailManager.GetPriceTrend(item.RSI_NO, item.SN);
            }
            return model;
        }

        private byte[] ExchangeExcel(string rsi_no, IList<H_Product_DetailEntity> normalPartsDatas, Dictionary<string, IList<H_Product_DetailEntity>> specialPartsData, DataTable config)
        {
            DataTable pricetrend = RSI_ConfigEntityDAL.GetPriceTrend(rsi_no);
            //IWorkbook wb = new HSSFWorkbook();
            //ISheet ws;

            ////建立Excel 2007檔案
            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = null;

            #region NormalParts Sheet
            if (normalPartsDatas.Count() > 0)
            {
                ws = wb.CreateSheet("Component Price List");
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

                ws.GetRow(0).CreateCell(0).SetCellValue("產品組合");
                ws.GetRow(0).GetCell(0).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(1).SetCellValue("Material Group");
                ws.GetRow(0).GetCell(1).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(2).SetCellValue("Parts Group");
                ws.GetRow(0).GetCell(2).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(3).SetCellValue("Material Parts");
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
                ws.GetRow(0).CreateCell(10).SetCellValue("PM Price");
                ws.GetRow(0).GetCell(10).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(11).SetCellValue("MOQ");
                ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(12).SetCellValue("MOCKUP");
                ws.GetRow(0).GetCell(12).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(13).SetCellValue("TOOLING");
                ws.GetRow(0).GetCell(13).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(14).SetCellValue("Valuation");
                ws.GetRow(0).GetCell(14).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(15).SetCellValue("PM Amount");
                ws.GetRow(0).GetCell(15).CellStyle = cellstyle;
                for (var i = 0; i < pricetrend.Rows.Count; i++)
                {
                    ws.GetRow(0).CreateCell(16 + i).SetCellValue(pricetrend.Rows[i]["description"].ToString());
                    ws.GetRow(0).GetCell(16 + i).CellStyle = cellstyle;
                }
                ws.GetRow(0).CreateCell(16 + pricetrend.Rows.Count).SetCellValue("RD Remark");
                ws.GetRow(0).GetCell(16 + pricetrend.Rows.Count).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(17 + pricetrend.Rows.Count).SetCellValue("To PM Remark");
                ws.GetRow(0).GetCell(17 + pricetrend.Rows.Count).CellStyle = cellstyle;
                ws.GetRow(0).CreateCell(18 + pricetrend.Rows.Count).SetCellValue("Sorcer Remark");
                ws.GetRow(0).GetCell(18 + pricetrend.Rows.Count).CellStyle = cellstyle;
                
                ws.CreateFreezePane(0, 1);
                #endregion

                for (int i = 0; i < normalPartsDatas.Count(); i++)
                {
                    cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
                    fontstyle = wb.CreateFont();
                    fontstyle.FontName = "微軟正黑體";
                    cellstyle.SetFont(fontstyle);

                    if (!String.IsNullOrEmpty(normalPartsDatas[i].MTL_TYPE))
                    {
                        if (normalPartsDatas[i].MTL_TYPE.Equals("Special"))
                        {
                            color = new XSSFColor(new byte[] { 243, 156, 18 });
                            cellstyle.FillForegroundColorColor = color;
                            cellstyle.FillPattern = FillPattern.SolidForeground;
                        }

                        if (normalPartsDatas[i].MTL_GROUP.Equals("小計"))
                        {
                            color = new XSSFColor(new byte[] { 147, 205, 221 });
                            cellstyle.FillForegroundColorColor = color;
                            cellstyle.FillPattern = FillPattern.SolidForeground;
                        }
                    }

                    ws.CreateRow(i + 1);
                    #region 綁定資料
                    
                    ws.GetRow(i + 1).CreateCell(0).SetCellValue(normalPartsDatas[i].MTL_TYPE.Equals("Special") ? normalPartsDatas[i].GROUP_DESC : String.Empty);
                    ws.GetRow(i + 1).GetCell(0).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(1).SetCellValue(normalPartsDatas[i].MTL_GROUP);
                    ws.GetRow(i + 1).GetCell(1).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(2).SetCellValue(normalPartsDatas[i].PARTS_GROUP);
                    ws.GetRow(i + 1).GetCell(2).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(3).SetCellValue(normalPartsDatas[i].MTL_PARTS);
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
                    var price_group = config.AsEnumerable().Where(p => p.Field<string>("PART_TYPE") == normalPartsDatas[i].PART_TYPE).
                        Select(p => p.Field<string>("PRICE_GROUP")).FirstOrDefault();
                    bool priceshow = normalPartsDatas[i].MTL_GROUP == "小計" || price_group == "Part_No";

                    if (normalPartsDatas[i].PRICE_PM != null)
                        ws.GetRow(i + 1).CreateCell(10).SetCellValue((double)normalPartsDatas[i].PRICE_PM);
                    else
                        ws.GetRow(i + 1).CreateCell(10).SetCellValue(string.Empty);
                    ws.GetRow(i + 1).GetCell(10).CellStyle = cellstyle;
                    if (!priceshow) ws.GetRow(i + 1).GetCell(10).SetCellValue(string.Empty);

                    if (normalPartsDatas[i].MOQ != null)
                        ws.GetRow(i + 1).CreateCell(11).SetCellValue((double)normalPartsDatas[i].MOQ);
                    else
                        ws.GetRow(i + 1).CreateCell(11).SetCellValue(string.Empty);
                    ws.GetRow(i + 1).GetCell(11).CellStyle = cellstyle;
                    if (!priceshow) ws.GetRow(i + 1).GetCell(11).SetCellValue(string.Empty);

                    if (normalPartsDatas[i].MOCKUP != null)
                        ws.GetRow(i + 1).CreateCell(12).SetCellValue((double)normalPartsDatas[i].MOCKUP);
                    else
                        ws.GetRow(i + 1).CreateCell(12).SetCellValue(string.Empty);
                    ws.GetRow(i + 1).GetCell(12).CellStyle = cellstyle;
                    if (!priceshow) ws.GetRow(i + 1).GetCell(12).SetCellValue(string.Empty);

                    if (normalPartsDatas[i].TOOLING != null)
                        ws.GetRow(i + 1).CreateCell(13).SetCellValue((double)normalPartsDatas[i].TOOLING);
                    else
                        ws.GetRow(i + 1).CreateCell(13).SetCellValue(string.Empty);
                    ws.GetRow(i + 1).GetCell(13).CellStyle = cellstyle;
                    if (!priceshow) ws.GetRow(i + 1).GetCell(13).SetCellValue(string.Empty);

                    ws.GetRow(i + 1).CreateCell(14).SetCellValue(normalPartsDatas[i].ISCALCULATE);
                    ws.GetRow(i + 1).GetCell(14).CellStyle = cellstyle;

                    if (normalPartsDatas[i].PMAMOUNT != null)
                        ws.GetRow(i + 1).CreateCell(15).SetCellValue((double)normalPartsDatas[i].PMAMOUNT);
                    else
                        ws.GetRow(i + 1).CreateCell(15).SetCellValue(string.Empty);
                    ws.GetRow(i + 1).GetCell(15).CellStyle = cellstyle;
                    if (!priceshow) ws.GetRow(i + 1).GetCell(15).SetCellValue(string.Empty);

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
                            ws.GetRow(i + 1).CreateCell(16 + j).SetCellValue((double)temp_number);
                        else
                            ws.GetRow(i + 1).CreateCell(16 + j).SetCellValue(string.Empty);
                        ws.GetRow(i + 1).GetCell(16 + j).CellStyle = cellstyle;
                        if (!priceshow) ws.GetRow(i + 1).GetCell(16 + j).SetCellValue(string.Empty);
                    }
                    ws.GetRow(i + 1).CreateCell(16 + pricetrend.Rows.Count).SetCellValue(normalPartsDatas[i].REMARK);
                    ws.GetRow(i + 1).GetCell(16 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(17 + pricetrend.Rows.Count).SetCellValue(normalPartsDatas[i].REMARK_PM);
                    ws.GetRow(i + 1).GetCell(17 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    ws.GetRow(i + 1).CreateCell(18 + pricetrend.Rows.Count).SetCellValue(normalPartsDatas[i].REMARK_PUR);
                    ws.GetRow(i + 1).GetCell(18 + pricetrend.Rows.Count).CellStyle = cellstyle;
                    #endregion
                }

                for (var i = 0; i < ws.GetRow(0).Cells.Count; i++)
                {
                    ws.AutoSizeColumn(i);
                }

                //開啟篩選功能,避免RMS加密限制複製功能後無法使用篩選
                CellRangeAddress c = new CellRangeAddress(0, 1, 0, 18 + pricetrend.Rows.Count);
                ws.SetAutoFilter(c);
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
                    ws.GetRow(0).CreateCell(11).SetCellValue("RD Remark");
                    ws.GetRow(0).GetCell(11).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(12).SetCellValue("To PM Remark");
                    ws.GetRow(0).GetCell(12).CellStyle = cellstyle;
                    ws.GetRow(0).CreateCell(13).SetCellValue("Sorcer Remark");
                    ws.GetRow(0).GetCell(13).CellStyle = cellstyle;
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
                        ws.GetRow(i + 1).CreateCell(0).SetCellValue(item.DISPLAYPARTNO ?? String.Empty);
                        ws.GetRow(i + 1).GetCell(0).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(1).SetCellValue(item.BOM_LEVEL.ToString());
                        ws.GetRow(i + 1).GetCell(1).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(2).SetCellValue(item.PART_LEVEL);
                        ws.GetRow(i + 1).GetCell(2).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(3).SetCellValue(item.MTL_GROUP);
                        ws.GetRow(i + 1).GetCell(3).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(4).SetCellValue(item.MTL_PARTS);
                        ws.GetRow(i + 1).GetCell(4).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(5).SetCellValue(item.PART_TYPE);
                        ws.GetRow(i + 1).GetCell(5).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(6).SetCellValue(item.ENGLISH_NAME);
                        ws.GetRow(i + 1).GetCell(6).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(7).SetCellValue(item.PART_DESC);
                        ws.GetRow(i + 1).GetCell(7).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(8).SetCellValue(item.PART_SPEC);
                        ws.GetRow(i + 1).GetCell(8).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(9).SetCellValue((double)item.USAGE);
                        ws.GetRow(i + 1).GetCell(9).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(10).SetCellValue(item.PART_UNIT);
                        ws.GetRow(i + 1).GetCell(10).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(11).SetCellValue(item.REMARK);
                        ws.GetRow(i + 1).GetCell(11).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(12).SetCellValue(item.REMARK_PM);
                        ws.GetRow(i + 1).GetCell(12).CellStyle = cellstyle;
                        ws.GetRow(i + 1).CreateCell(13).SetCellValue(item.REMARK_PUR);
                        ws.GetRow(i + 1).GetCell(13).CellStyle = cellstyle;
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

        private IList<H_Product_DetailEntity> ComputeSubTotalByConfig(IList<H_Product_DetailEntity> model, DataTable config)
        {
            List<H_Product_DetailEntity> result = new List<H_Product_DetailEntity>();
            string temp_mtl_group = string.Empty, temp_parts_group = string.Empty, temp_mtl_parts = string.Empty, temp_part_type = string.Empty, temp_mtl_type = string.Empty;
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
                        if (temp_price_config.Equals("Material_Group")) subtotal.MTL_GROUP = temp_mtl_group;
                        if (temp_price_config.Equals("Parts_Group")) subtotal.PARTS_GROUP = temp_parts_group;
                        if (temp_price_config.Equals("Material_Parts")) subtotal.MTL_PARTS = temp_mtl_parts;

                        subtotal.MTL_GROUP = "小計";
                        subtotal.MTL_TYPE = temp_mtl_type;
                        subtotal.PART_TYPE = temp_part_type;
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

                temp_price_config = config.AsEnumerable().Where(p => p.Field<string>("PART_TYPE") == item.PART_TYPE)
                    .Select(p => p.Field<string>("PRICE_GROUP")).FirstOrDefault();
                temp_mtl_group = item.MTL_GROUP;
                temp_parts_group = item.PARTS_GROUP;
                temp_mtl_parts = item.MTL_PARTS;
                temp_part_type = item.PART_TYPE;
                temp_mtl_type = item.MTL_TYPE;

                temp_sourcerprice += item.PRICE ?? 0;
                temp_pmprice += item.PRICE_PM ?? 0;
                temp_moq += item.MOQ ?? 0;
                temp_mockup += item.MOCKUP ?? 0;
                temp_tooling += item.TOOLING ?? 0;
                temp_sourceramount += item.SOURCERAMOUNT ?? 0;
                temp_pmamount += item.PMAMOUNT ?? 0;

                if (item.pricetrend.Any())
                    temp_pricetrend.AddRange(item.pricetrend);
                result.Add(item);
            }

            if (!temp_price_config.Equals("Part_No"))
            {
                H_Product_DetailEntity subtotal = new H_Product_DetailEntity();
                subtotal.MTL_GROUP = "小計";
                subtotal.MTL_TYPE = temp_mtl_type;
                subtotal.PART_TYPE = temp_part_type;
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

            return result;
        }
    }
}