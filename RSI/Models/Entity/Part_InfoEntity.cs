using Oracle.ManagedDataAccess.Client;
using RSI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace RSI.Models.Entity
{
    #region DTO
    public class Part_InfoEntity
    {
        public string ITEM_NO { get; set; }
        public string ENGLIST_NAME { get; set; }
        public string ITEM_DESC { get; set; }
        public string VENDOR { get; set; }
        public string PART_LEVEL { get; set; }
        public string Maker_PN { get; set; }
        public DateTime RELEASE_DATE { get; set; }
    }
    #endregion

    #region DAL
    public static class Part_InfoEntityDAL
    {
        public static IList<Part_InfoEntity> GetPart_InfoEntities(string part_level, string english_name, string item_no, string item_desc, string maker_pn, string vendor, string remark)
        {
            IList<Part_InfoEntity> part_Infos = new List<Part_InfoEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select i.part_no as item_no, i.english_name as Englist_name, i.part_description as item_Desc,
                                                substr(i.maker, 0, 15) as Vendor,i.maker, i.part_kind_key as PART_LEVEL,
                                                i.maker_part_number as Maker_PN, i.Release_Date
                                                from rsi_r_part_info i
                                                where (:part_level is null or i.part_kind_key like :part_level)  
                                                and (:english_name is null or i.english_name like :english_name)
                                                 and (:item_no is null or i.part_no like :item_no)
                                                and (:item_desc is null or i.part_description like :item_desc)
                                                and (:maker_pn is null or i.maker_part_number like :maker_pn)
                                                and (:vendor is null or i.maker like :vendor)
                                                and (:remark is null or i.remark like :remark) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("part_level", OracleDbType.Varchar2, String.IsNullOrEmpty(part_level) ? DBNull.Value : (object)String.Format("%{0}%",part_level) , ParameterDirection.Input));
                param.Add(new OracleParameter("english_name", OracleDbType.Varchar2, String.IsNullOrEmpty(english_name) ? DBNull.Value : (object)String.Format("%{0}%", english_name), ParameterDirection.Input));
                param.Add(new OracleParameter("item_no", OracleDbType.Varchar2, String.IsNullOrEmpty(item_no) ? DBNull.Value : (object)String.Format("%{0}%", item_no), ParameterDirection.Input));
                param.Add(new OracleParameter("item_desc", OracleDbType.Varchar2, String.IsNullOrEmpty(item_desc) ? DBNull.Value : (object)String.Format("%{0}%", item_desc), ParameterDirection.Input));
                param.Add(new OracleParameter("maker_pn", OracleDbType.Varchar2, String.IsNullOrEmpty(maker_pn) ? DBNull.Value : (object)String.Format("%{0}%", maker_pn), ParameterDirection.Input));
                param.Add(new OracleParameter("vendor", OracleDbType.Varchar2, String.IsNullOrEmpty(vendor) ? DBNull.Value : (object)String.Format("%{0}%", vendor), ParameterDirection.Input));
                param.Add(new OracleParameter("remark", OracleDbType.Varchar2, String.IsNullOrEmpty(remark) ? DBNull.Value : (object)String.Format("%{0}%", remark), ParameterDirection.Input));

                part_Infos = SqlExcute.GetOraObjList<Part_InfoEntity>(sqlText, param.ToArray(), "GetH_Product_DetailEntities", "取得Part_Info資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return part_Infos;
            }
            return part_Infos;
        }

        public static DataTable GetPrice_Group(string bu, string part_type, string price_type)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT *
                                   FROM GPO.RSI_C_PARAMETER 
                                  WHERE FUNCTION = :price_type
                                    AND ATTRIBUTE1 = :bu
                                    AND DECODE( :part_type, UPPER(ATTRIBUTE2), 1, INSTR(ATTRIBUTE2, 'ALL')) > 0
                                  ORDER BY DECODE(UPPER(ATTRIBUTE3), 'MATERIAL_GROUP', 0, 'PARTS_GROUP', 1, 'MATERIAL_PARTS', 2, 'PART_NO', 3, 4)  ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.Varchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("price_type", OracleDbType.Varchar2, price_type, ParameterDirection.Input));

                dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetPrice_Group", "取得Price_Group資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dt;
            }
            return dt;
        }

        public static DataTable GetPageNote(string phase_id)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM GPO.RSI_C_PARAMETER
                                  WHERE FUNCTION = 'Pages_Note'
                                    AND ATTRIBUTE1 = :phase_id ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));

                dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetPageNote", "取得Page Note資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dt;
            }
            return dt;
        }

        public static DataTable GetPartLevelEnglish()
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct m.mtl_group,
                                                 m.parts_group,
                                                 m.mtl_parts,
                                                 p.part_kind_key as part_level,
                                                 p.english_name
                                  from rsi_r_part_info p
                                  left join gpo.rsi_c_mtl_parts m
                                    on p.part_kind_key = m.part_level
                                   and p.english_name = m.english_name
                                   and m.active = 'Y'
                                 --where p.part_kind_key not like '9%'
                                   where p.english_name is not null
                                 order by p.part_kind_key ");

                return SqlExcute.GetOraDateTable(sqlText, "GetPartLevelEnglish", string.Empty);
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion

}