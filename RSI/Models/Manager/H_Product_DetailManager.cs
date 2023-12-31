﻿using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class H_Product_DetailManager
    {
        public static IList<H_Product_DetailEntity> Get_Normal_Product_DetailEntities(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_Normal_Product_DetailEntities(rsi_no, part_type);
        }

        public static IList<H_Product_DetailEntity> Get_Normal_RD_Boss(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_Normal_RD_Boss(rsi_no, part_type);
        }
        public static IList<H_Product_DetailEntity> Get_Normal_Sourcer(string rsi_no, string emp_no)
        {
            return H_Product_DetailEntityDAL.Get_Normal_Sourcer(rsi_no, emp_no);
        }

        public static IList<H_Product_DetailEntity> Get_Normal_Sourcer_View(string rsi_no, string emp_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_Normal_Sourcer_View(rsi_no, emp_no, part_type);
        }

        public static IList<H_Product_DetailEntity> Get_Normal_Sourcer_Boss(string rsi_no, string emp_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_Normal_Sourcer_Boss(rsi_no, emp_no, part_type);
        }

        public static IList<H_Product_DetailEntity> Get_Sourcer_Reassign_View(string rsi_no, string emp_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_Sourcer_Reassign_View(rsi_no, emp_no, part_type);
        }

        public static IList<H_Product_DetailEntity> Get_PMConfrim_ExportData(string rsi_no)
        {
            return H_Product_DetailEntityDAL.Get_PMConfrim_ExportData(rsi_no);
        }

        public static IList<H_Product_DetailEntity> Get_PMConfirm_Special_ExportData(string rsi_no, string group_id)
        {
            return H_Product_DetailEntityDAL.Get_PMConfirm_Special_ExportData(rsi_no, group_id);
        }

        public static IList<H_Product_DetailEntity> Get_Special_Product_DetailEntities(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_Special_Product_DetailEntities(rsi_no, part_type);
        }

        public static IList<H_Product_DetailEntity> Get_Special_Sourcer(string rsi_no, string emp_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_Special_Sourcer(rsi_no, emp_no, part_type);
        }

        public static IList<H_Product_DetailEntity> Get_Special_Sourcer_Boss(string rsi_no, string emp_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_Special_Sourcer_Boss(rsi_no, emp_no, part_type);
        }

        public static IList<H_Product_DetailEntity> Get_PM_Portfolio_Detail(string rsi_no, string group_id)
        {
            return H_Product_DetailEntityDAL.Get_PM_Portfolio_Detail(rsi_no, group_id);
        }

        public static bool Delete_Product_Detail(H_Product_DetailEntity h_Product_Detail, string group_id)
        {
            return H_Product_DetailEntityDAL.Delete_Product_Detail(h_Product_Detail, group_id);
        }

        public static bool Create_Product_Detail(H_Product_DetailEntity h_Product_Detail)
        {
            return H_Product_DetailEntityDAL.Create_Product_Detail(h_Product_Detail);
        }

        public static bool Update_Product_Detail(H_Product_DetailEntity h_Product_Detail)
        {
            return H_Product_DetailEntityDAL.Update_Product_Detail(h_Product_Detail);
        }

        public static IList<string> GetEOL(string item_no)
        {
            return H_Product_DetailEntityDAL.GetEOL(item_no);
        }

        public static IList<string> GetUNI_SPEC_STATUS(string item_no)
        {
            return H_Product_DetailEntityDAL.GetUNI_SPEC_STATUS(item_no);
        }

        public static IList<string> GetGroupID(string rsi_no)
        {
            return H_Product_DetailEntityDAL.GetGroupID(rsi_no);
        }

        public static void UpdateSpecialDesc(string rsi_no, string group_id, string group_name, string group_desc)
        {
            H_Product_DetailEntityDAL.UpdateSpecialDesc(rsi_no, group_id, group_name, group_desc);
        }

        public static string GetGroupName(string rsi_no, string group_id)
        {
            return H_Product_DetailEntityDAL.GetGroupName(rsi_no, group_id);
        }

        public static string GetGroupDesc(string rsi_no, string group_id)
        {
            return H_Product_DetailEntityDAL.GetGroupDesc(rsi_no, group_id);
        }

        public static void UpdateForDeleteSpecialPart(string group_id)
        {
            H_Product_DetailEntityDAL.UpdateForDeleteSpecialPart(group_id);
        }

        public static int GetNumberForCheckAllPMPrice(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.GetNumberForCheckAllPMPrice(rsi_no, part_type);
        }

        public static int GetSpecialNameRepeat(string rsi_no, string group_name)
        {
            return H_Product_DetailEntityDAL.GetSpecialNameRepeat(rsi_no, group_name);
        }

        public static void Delete_Product_Detail(string rsi_no)
        {
            H_Product_DetailEntityDAL.Delete_Product_Detail(rsi_no);
        }

        public static IList<H_Product_DetailEntity> Get_New_Product_Detail(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.Get_New_Product_Detail(rsi_no, part_type);
        }

        public static void Update_Product_Detail_For_Layer1(H_Product_DetailEntity h_Product_Detail)
        {
            H_Product_DetailEntityDAL.Update_Product_Detail_For_Layer1(h_Product_Detail);
        }

        public static void Update_Product_Detail_For_Layer1_Submit(string rsi_no, string part_type)
        {
            H_Product_DetailEntityDAL.Update_Product_Detail_For_Layer1_Submit(rsi_no, part_type);
        }
        public static IList<H_Product_DetailEntity> GetSourcerManagerDetail(string rsi_no, string emp_no, string group_id)
        {
            return H_Product_DetailEntityDAL.GetSourcerManagerDetail(rsi_no, emp_no, group_id);
        }

        public static IList<H_Product_DetailEntity> GetRDReviewNormalParts(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.GetRDReviewNormalParts(rsi_no, part_type);
        }

        public static IList<H_Product_DetailEntity> GetBOMTableDataForSourcer(string rsi_no, string part_type, string phase_id, string emp_no)
        {
            return H_Product_DetailEntityDAL.GetBOMTableDataForSourcer(rsi_no, part_type, phase_id, emp_no);
        }

        public static IList<H_Product_DetailEntity> GetRDReviewNormalpartsBySN(string rsi_no, string sn)
        {
            return H_Product_DetailEntityDAL.GetRDReviewNormalpartsBySN(rsi_no, sn);
        }

        public static IList<H_Product_DetailEntity> GetRDReviewSpecialParts(string rsi_no)
        {
            return H_Product_DetailEntityDAL.GetRDReviewSpecialParts(rsi_no);
        }

        public static bool RDReviewSpecialModefyTypeD(string rsi_no, string group_id)
        {
            return H_Product_DetailEntityDAL.RDReviewSpecialModefyTypeD(rsi_no, group_id);
        }

        public static bool RDReviewNormalModifyTypeSToEmpty(string rsi_no)
        {
            return H_Product_DetailEntityDAL.RDReviewNormalModifyTypeSToEmpty(rsi_no);
        }

        public static bool RDReviewNormalModifyTypeSToEmptyWithSpecialDelete(string rsi_no, string part_no, string parent_part_no)
        {
            return H_Product_DetailEntityDAL.RDReviewNormalModifyTypeSToEmptyWithSpecialDelete(rsi_no, part_no, parent_part_no);
        }

        public static bool RDReviewNormalModifyTypeEmptyToS(string rsi_no, string group_id)
        {
            return H_Product_DetailEntityDAL.RDReviewNormalModifyTypeEmptyToS(rsi_no, group_id);
        }

        public static bool RDReviewChangeCallPackage(string partno, string partlevel, string plant)
        {
            return H_Product_DetailEntityDAL.RDReviewChangeCallPackage(partno, partlevel, plant);
        }

        public static IList<H_Product_DetailEntity> GetRDReviewChangeOtherTableData(string rsi_no, string partno)
        {
            return H_Product_DetailEntityDAL.GetRDReviewChangeOtherTableData(rsi_no, partno);
        }

        public static bool UpdateSpecialPartsName(string rsi_no, string group_id, string group_name)
        {
            return H_Product_DetailEntityDAL.UpdateSpecialPartsName(rsi_no, group_id, group_name);
        }

        public static bool UpdateSpecialPartsDesc(string rsi_no, string group_id, string group_desc)
        {
            return H_Product_DetailEntityDAL.UpdateSpecialPartsDesc(rsi_no, group_id, group_desc);
        }

        public static DataTable GetMaterialGroupMaterialParts(string rsi_no, string part_level, string english_name, string parent_part_level, string sn)
        {
            return H_Product_DetailEntityDAL.GetMaterialGroupMaterialParts(rsi_no, part_level, english_name, parent_part_level, sn);
        }

        public static String GetUniSpecStatus(string part_no)
        {
            return H_Product_DetailEntityDAL.GetUniSpecStatus(part_no);
        }

        public static String GetEolStatus(string part_no)
        {
            return H_Product_DetailEntityDAL.GetEolStatus(part_no);
        }

        public static void UpdateParentPartNo(string rsi_no, List<string> resultSN, string parent_no)
        {
            H_Product_DetailEntityDAL.UpdateParentPartNo(rsi_no, resultSN, parent_no);
        }

        public static void DeleteMOH(string rsi_no, string mtl_type)
        {
            H_Product_DetailEntityDAL.DeleteMOH(rsi_no, mtl_type);
        }

        public static void InsertMOHForNormal(string rsi_no)
        {
            H_Product_DetailEntityDAL.InsertMOHForNormal(rsi_no);
        }

        public static void InsertMOHForSpecial(string rsi_no)
        {
            H_Product_DetailEntityDAL.InsertMOHForSpecial(rsi_no);
        }
        public static void UpdateChooseToSpecialStatus(string rsi_no)
        {
            H_Product_DetailEntityDAL.UpdateChooseToSpecialStatus(rsi_no);
        }

        public static IList<H_Product_DetailEntity> GetProductDetailForExportExcelSpecialData(string rsi_no, string group_id, string emp_no)
        {
            return H_Product_DetailEntityDAL.GetProductDetailForExportExcelSpecialData(rsi_no, group_id, emp_no);
        }

        public static IList<H_Product_DetailEntity> ProductReview_GetNormalParts(string rsi_no, string part_type, string emp_no)
        {
            return H_Product_DetailEntityDAL.ProductReview_GetNormalParts(rsi_no, part_type, emp_no);
        }

        public static IList<H_Product_DetailEntity> ProductReview_GetSpecialParts(string rsi_no, string part_type, string emp_no)
        {
            return H_Product_DetailEntityDAL.ProductReview_GetSpecialParts(rsi_no, part_type, emp_no);
        }

        public static IList<H_Product_PriceTrend> GetPriceTrend(Decimal rsi_no, Decimal sn)
        {
            return H_Product_DetailEntityDAL.GetPriceTrend(rsi_no, sn);
        }

        public static void UpdatePriceTrend(string rsi_no, string sn, string id, string price)
        {
            H_Product_DetailEntityDAL.UpdatePriceTrend(rsi_no, sn, id, price);
        }

        public static void UpdateParentSNForChange(string rsi_no)
        {
            H_Product_DetailEntityDAL.UpdateParentSNForChange(rsi_no);
        }

        public static void UpdateParentSNbyBOM(string rsi_no, string old_sn, string new_sn)
        {
            H_Product_DetailEntityDAL.UpdateParentSNbyBOM(rsi_no, old_sn, new_sn);
        }

        public static string GetProductDetailRSINOSN(string rsi_no, string part_no)
        {
            return H_Product_DetailEntityDAL.GetProductDetailRSINOSN(rsi_no, part_no);
        }

        public static Decimal GetSNByProductDetail()
        {
            return H_Product_DetailEntityDAL.GetSNByProductDetail();
        }

        public static void ModalEdit_Save(H_Product_DetailEntity model)
        {
            H_Product_DetailEntityDAL.ModalEdit_Save(model);
        }

        public static void InsertLogForReturnSubmit(string rsi_no, string part_type, string emp_no)
        {
            H_Product_DetailEntityDAL.InsertLogForReturnSubmit(rsi_no, part_type, emp_no);
        }

        public static DataTable GetSheet1Data(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.GetSheet1Data(rsi_no, part_type);
        }

        public static DataTable GetSheet2Data(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.GetSheet2Data(rsi_no, part_type);
        }

        public static DataTable GetSheet3Data()
        {
            return H_Product_DetailEntityDAL.GetSheet3Data();
        }

        public static void DeleteDetailForUpload(string rsi_no, string part_type, string emp_no)
        {
            H_Product_DetailEntityDAL.DeleteDetailForUpload(rsi_no, part_type, emp_no);
        }

        public static void DeleteDetailTempForUpload(string rsi_no, string part_type)
        {
            H_Product_DetailEntityDAL.DeleteDetailTempForUpload(rsi_no, part_type);
        }

        public static void InsertDetailTempForUpload(string rsi_no, H_Product_DetailTree data, string emp_no)
        {
            H_Product_DetailEntityDAL.InsertDetailTempForUpload(rsi_no, data, emp_no);
        }

        public static void Ins_rd_upload_bom(string rsi_no, string part_type, string emp_no)
        {
            H_Product_DetailEntityDAL.Ins_rd_upload_bom(rsi_no, part_type, emp_no);
        }

        public static void UpdateDetailByFileManagement(string rsi_no, string sn, string emp_no)
        {
            H_Product_DetailEntityDAL.UpdateDetailByFileManagement(rsi_no, sn, emp_no);
        }

        public static bool UpdateRDDefineOwner(string rsi_no, string part_type)
        {
            return H_Product_DetailEntityDAL.UpdateRDDefineOwner(rsi_no, part_type);
        }

        public static DataTable PartNoApprovedForSubmit(string rsi_no, string part_type, string emp_no)
        {
            return H_Product_DetailEntityDAL.PartNoApprovedForSubmit(rsi_no, part_type, emp_no);
        }

        public static IList<H_Product_DetailEntity> ProductValuation(string rsi_no, string part_type, string phase_id, string sn, string emp_no)
        {
            return H_Product_DetailEntityDAL.ProductValuation(rsi_no, part_type, phase_id, sn, emp_no);
        }

        public static void ProductValuationSave(H_Product_DetailEntity model)
        {
            H_Product_DetailEntityDAL.ProductValuationSave(model);
        }

        public static IList<string> GetPartNoMfg(string part_no)
        {
            return H_Product_DetailEntityDAL.GetPartNoMfg(part_no);
        }

        public static void Gen_ref_bom_data(string partnumber_top, string partnumber_top_mfg, string part_no, string emp_no)
        {
            H_Product_DetailEntityDAL.Gen_ref_bom_data(partnumber_top, partnumber_top_mfg, part_no, emp_no);
        }

        public static IList<H_Product_DetailEntity> GetNewOtherTableData(string rsi_no, string emp_no)
        {
            return H_Product_DetailEntityDAL.GetNewOtherTableData(rsi_no, emp_no);
        }
    }
}