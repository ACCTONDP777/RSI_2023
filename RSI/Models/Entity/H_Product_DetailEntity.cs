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
    public class H_Product_DetailEntity
    {
        public decimal RSI_NO { get; set; }
        public string MTL_GROUP { get; set; }
        public string MTL_PARTS { get; set; }
        public string PART_TYPE { get; set; }
        public string PART_NO { get; set; }
        public string PART_DESC { get; set; }
        public string PART_LEVEL { get; set; }
        public string ENGLISH_NAME { get; set; }
        public string MAKER_SOURCE { get; set; }
        public string MAKER_PART_NO { get; set; }
        public string PART_SPEC { get; set; }
        public DateTime RELEASE_DATE { get; set; }
        public decimal USAGE { get; set; }
        public string EOL_STATUS { get; set; }
        public string UNI_SPEC_STATUS { get; set; }
        public string REMARK { get; set; }
        public string MTL_TYPE { get; set; }
        public string SPEC_DEF { get; set; }
        public string MODIFY_TYPE { get; set; }
        public decimal SN { get; set; }
        public decimal? PARENT_SN { get; set; }
        public string GROUP_ID { get; set; }
        public string BU { get; set; }
        public string STATUS { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? PRICE_PM { get; set; }
        public decimal? MOQ { get; set; }
        public decimal? MOCKUP { get; set; }
        public decimal? TOOLING { get; set; }
        public decimal? FPCA_PCBA { get; set; }
        public decimal? SOURCERAMOUNT { get; set; }
        public decimal? PMAMOUNT { get; set; }
        public string CURRENCY { get; set; }
        public decimal RATE { get; set; }
        public string SOURCE_NO { get; set; }
        public string SOURCE { get; set; }
        public string ISMODIFY { get; set; }
        public string PHASE_ID { get; set; }
        public decimal? PRICE_HIS_H { get; set; }
        public decimal? PRICE_HIS_L { get; set; }
        public string REMARK_PUR { get; set; }
        public string PARTS_GROUP { get; set; }
        public string PRICE_GROUP { get; set; }
        public string FILE_STATUS { get; set; }
        public string GROUP_DESC { get; set; }
        public string GROUP_NAME { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string EMP_NAME { get; set; }
        public string ISASSIGNER { get; set; }
        public string ISCALCULATE { get; set; }
        public decimal PRICE_SOURCER { get; set; }
        public decimal GAP { get; set; }
        public string REMARK_PM { get; set; }
        public string PARTNUMBER_TOP { get; set; }
        public string PARTNUMBER_PARENT { get; set; }
        public string PARTNUMBER_CHILD { get; set; }
        public string DISPLAYPARTNO { get; set; }
        public string BOM_PATH { get; set; }
        public decimal BOM_LEVEL { get; set; }
        public string PART_UNIT { get; set; }
        public string REASSIGN_FROM { get; set; }
        public string REASSIGN_TO { get; set; }
        public DateTime REASSIGN_DATE { get; set; }
        public IList<H_Product_PriceTrend> pricetrend { get; set; }

        public string ISAPPROVED { get; set; }
        public string SOURCER_OWNER { get; set; }
        public string SOURCER_OWNER_ID { get; set; }
        public string PART_NO_MFG { get; set; }
    }

    public class H_Product_PriceTrend
    {
        public string RSI_NO { get; set; }
        public Decimal SN { get; set; }
        public Decimal ID { get; set; }
        public Decimal? PRICE { get; set; }
    }

    public class H_Product_DetailVM
    {
        public IList<H_Product_DetailEntity> normal_Detail { get; set; }
        public IList<H_Product_DetailEntity> specail_Detail { get; set; }
    }

    public class H_Product_SpecailDesc
    {
        public string rsi_no { get; set; }
        public string group_id { get; set; }
        public string group_name { get; set; }
        public string group_desc { get; set; }
    }

    public class H_Product_DetailTree
    {
        public string parent_level { get; set; }
        public string part_type { get; set; }
        public string part_no { get; set; }
        public string part_level { get; set; }
        public string english_name { get; set; }
        public string part_spec { get; set; }
        public string usage { get; set; }
        public string unit { get; set; }
        public string remark { get; set; }
        public string ismodify { get; set; }
        public List<H_Product_DetailTree> nodes { get; set; }
        public int excelrowindex { get; set; }
    }
    #endregion

    #region DAL
    public static class H_Product_DetailEntityDAL
    {
        public static IList<H_Product_DetailEntity> Get_Normal_Product_DetailEntities(string rsi_no, string part_type)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select *
                 from (
                       --RSI大表內材料
                       select distinct d.rsi_no,
                                        nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                        p.mtl_group,
                                        p.mtl_parts,
                                        nvl(d.part_type, p.part_type) part_type,
                                        d.part_no,
                                        d.part_desc,
                                        nvl(r.part_level, d.part_level) PART_LEVEL,
                                        nvl(r.english_name, d.english_name) ENGLISH_NAME,
                                        d.maker_source,
                                        d.maker_part_no,
                                        d.part_spec,
                                        c.SPEC_DEF,
                                        d.release_date,
                                        d.usage,
                                        d.eol_status,
                                        d.uni_spec_status,
                                        d.remark,
                                        d.modify_type,
                                        d.sn,
                                        decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                        d.phase_id
                         from RSI_C_PARTS_TYPE_DEF p
                         join RSI_C_MTL_PARTS r
                           on r.mtl_group = p.mtl_group AND nvl(r.mtl_parts, '-') = nvl(p.mtl_parts, '-')
                         join gpo.c_pms_bgbu_mapping m
                           on m.bg = p.bg
                         join RSI_H_PRODUCT_INFO i
                           on m.bu = i.bu and i.rsi_no = :rsi_no
                         LEFT JOIN RSI_C_MTL_SPECIAL_PARTS s
                           ON s.mtl_parts = p.mtl_parts
                         LEFT JOIN (select *
                                      from RSI_H_PRODUCT_DETAIL
                                     where rsi_no = :rsi_no
                                       AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')) d --OR MODIFY_TYPE <> 'NEW'
                           on p.mtl_group = d.mtl_group
                          and p.mtl_parts = d.mtl_parts
                         and r.english_name = d.english_name
                         LEFT JOIN (SELECT DISTINCT MTL_PARTS, SPEC_DEF
                                      from RSI_C_MTL_SPEC_DEF
                                     WHERE ACTIVE = 'Y') c
                           ON p.MTL_PARTS = c.MTL_PARTS
                         LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                           ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                        WHERE nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) <> 'Special'
                       UNION --RD 自定義相目
                       select distinct d.rsi_no,
                                        nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                        d.mtl_group,
                                        d.mtl_parts,
                                        d.part_type,
                                        d.part_no,
                                        d.part_desc,
                                        nvl(d.part_level, r.part_level) PART_LEVEL,
                                        nvl(d.english_name, r.english_name) ENGLISH_NAME,
                                        d.maker_source,
                                        d.maker_part_no,
                                        d.part_spec,
                                        c.SPEC_DEF,
                                        d.release_date,
                                        d.usage,
                                        d.eol_status,
                                        d.uni_spec_status,
                                        d.remark,
                                        d.modify_type,
                                        d.sn,
                                        decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                        d.phase_id
                         from RSI_H_PRODUCT_DETAIL d
                         left join RSI_C_MTL_PARTS r
                           ON r.mtl_group = d.mtl_group and r.mtl_parts = d.mtl_parts
                         left join RSI_C_MTL_SPECIAL_PARTS s
                           ON s.mtl_parts = d.mtl_parts
                         left join (SELECT DISTINCT MTL_PARTS, SPEC_DEF
                                      from RSI_C_MTL_SPEC_DEF
                                     WHERE ACTIVE = 'Y') c
                           ON d.MTL_PARTS = c.MTL_PARTS
                         LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                           ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                        WHERE d.rsi_no = :rsi_no
                          --and d.MODIFY_TYPE = 'NEW'
                          AND D.MTL_PARTS = 'RD DEFINE'
                        and nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) <> 'Special') a ");
                string orderText = String.Empty;
                if (part_type.Equals("EE"))
                    orderText = "ORDER BY DECODE(MTL_GROUP || '-' || PART_TYPE, 'RD DEFINE-EE', 0), DECODE(PART_TYPE, 'EE', 1, 'ACD', 2, 'OM', 3, 'PACKING', 4, 5), MTL_GROUP, MTL_PARTS, PART_NO";
                if (part_type.Equals("OM"))
                    orderText = "ORDER BY DECODE(MTL_GROUP || '-' || PART_TYPE, 'RD DEFINE-OM', 0), DECODE(PART_TYPE, 'OM', 1, 'ACD', 2, 'EE', 3, 'PACKING', 4, 5), MTL_GROUP, MTL_PARTS, PART_NO";
                if (part_type.Equals("PACKING"))
                    orderText = "ORDER BY DECODE(MTL_GROUP || '-' || PART_TYPE, 'RD DEFINE-PACKING', 0), DECODE(PART_TYPE, 'PACKING', 1, 'ACD', 2, 'EE', 3, 'OM', 4, 5), MTL_GROUP, MTL_PARTS, PART_NO";
                if (part_type.Equals("ACD") || String.IsNullOrEmpty(orderText))
                    orderText = "ORDER BY DECODE(MTL_GROUP || '-' || PART_TYPE, 'RD DEFINE-ACD', 0), DECODE(PART_TYPE, 'ACD', 1, 'EE', 2, 'OM', 3, 'PACKING', 4, 5), MTL_GROUP, MTL_PARTS, PART_NO";
                sqlText.Append(orderText);
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));

                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetH_Product_DetailEntities", "取得H_Product_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static IList<H_Product_DetailEntity> Get_Normal_RD_Boss(string rsi_no, string part_type)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select * 
                                         from
                                         (
                                         select distinct d.rsi_no, nvl(d.mtl_type,decode(s.active,'Y','Special','Normal')) mtl_type, p.mtl_group, p.mtl_parts, 
                                                nvl(d.part_type,p.part_type) part_type, 
                                                d.part_no, d.part_desc,
                                                nvl(d.part_level,r.part_level) PART_LEVEL, 
                                                nvl(d.english_name, r.english_name) ENGLISH_NAME, 
                                                d.maker_source, d.maker_part_no, d.part_spec, c.SPEC_DEF,
                                                d.release_date, d.usage, d.eol_status, d.uni_spec_status, d.remark, d.modify_type, d.sn, decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                                d.phase_id
                                                from RSI_C_PARTS_TYPE_DEF p
                                                JOIN RSI_C_MTL_PARTS r
                                                ON r.mtl_group = p.mtl_group AND r.mtl_parts = p.mtl_parts
                                                join gpo.c_pms_bgbu_mapping m
                                                on m.bg = p.bg
                                                join RSI_H_PRODUCT_INFO i
                                                on m.bu = i.bu
                                                and i.rsi_no= :rsi_no
                                                LEFT JOIN RSI_C_MTL_SPECIAL_PARTS s
                                                ON s.mtl_parts = p.mtl_parts
                                                LEFT JOIN (select * from RSI_H_PRODUCT_DETAIL where rsi_no= :rsi_no AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')) d --OR MODIFY_TYPE <> 'NEW'
                                                on p.mtl_group = d.mtl_group
                                                and p.mtl_parts = d.mtl_parts
                                                LEFT JOIN (SELECT DISTINCT MTL_PARTS, SPEC_DEF from RSI_C_MTL_SPEC_DEF  WHERE ACTIVE = 'Y') c ON p.MTL_PARTS = c.MTL_PARTS
                                                LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                                WHERE nvl(d.mtl_type,decode(s.active,'Y','Special','Normal')) <> 'Special'
                                                union
                                                select distinct d.rsi_no,
                                                nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                                d.mtl_group,
                                                d.mtl_parts,
                                                d.part_type,
                                                d.part_no,
                                                d.part_desc,
                                                nvl(d.part_level, r.part_level) PART_LEVEL,
                                                nvl(d.english_name, r.english_name) ENGLISH_NAME,
                                                d.maker_source,
                                                d.maker_part_no,
                                                d.part_spec,
                                                c.SPEC_DEF,
                                                d.release_date,
                                                d.usage,
                                                d.eol_status,
                                                d.uni_spec_status,
                                                d.remark,
                                                d.modify_type,
                                                d.sn,
                                                decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                                d.phase_id
                                                from RSI_H_PRODUCT_DETAIL d
                                                LEFT JOIN RSI_C_MTL_PARTS r
                                                ON r.mtl_group = d.mtl_group
                                                AND r.mtl_parts = d.mtl_parts
                                                LEFT JOIN RSI_C_MTL_SPECIAL_PARTS s
                                                ON s.mtl_parts = d.mtl_parts
                                                LEFT JOIN (SELECT DISTINCT MTL_PARTS, SPEC_DEF
                                                from RSI_C_MTL_SPEC_DEF
                                                WHERE ACTIVE = 'Y') c
                                                ON d.MTL_PARTS = c.MTL_PARTS
                                                LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                                WHERE d.rsi_no = :rsi_no
                                                --and d.MODIFY_TYPE = 'NEW'
                                                AND D.MTL_PARTS = 'RD DEFINE'
                                                and nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) <> 'Special'
                                                ) D ");

                string orderText = String.Empty;
                if (part_type.Equals("EE"))
                    orderText = "ORDER BY DECODE(MTL_GROUP, 'RD DEFINE', 0), DECODE(D.PART_TYPE, 'EE', 0, 'ACD', 1, 'OM', 2, 'PACKING', 3, 4), D.MTL_TYPE, D.MTL_GROUP, D.MTL_PARTS, D.SN";
                if (part_type.Equals("OM"))
                    orderText = "ORDER BY DECODE(MTL_GROUP, 'RD DEFINE', 0), DECODE(D.PART_TYPE, 'OM', 0, 'ACD', 1, 'EE', 2, 'PACKING', 3, 4), D.MTL_TYPE, D.MTL_GROUP, D.MTL_PARTS, D.SN";
                if (part_type.Equals("PACKING"))
                    orderText = "ORDER BY DECODE(MTL_GROUP, 'RD DEFINE', 0), DECODE(D.PART_TYPE, 'PACKING', 0, 'ACD', 1, 'EE', 2, 'OM', 3, 4), D.MTL_TYPE, D.MTL_GROUP, D.MTL_PARTS, D.SN";
                if (part_type.Equals("ACD") || String.IsNullOrEmpty(orderText))
                    orderText = "ORDER BY DECODE(MTL_GROUP, 'RD DEFINE', 0), DECODE(D.PART_TYPE, 'ACD', 0, 'EE', 1, 'OM', 2, 'PACKING', 3, 4), D.MTL_TYPE, D.MTL_GROUP, D.MTL_PARTS, D.SN";

                sqlText.Append(orderText);
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));

                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetH_Product_DetailEntities", "取得H_Product_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static IList<H_Product_DetailEntity> Get_Normal_Sourcer(string rsi_no, string emp_no)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT RSI_NO, MTL_TYPE, D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, A.PART_TYPE_PUR AS PART_TYPE, D.PART_NO, D.PART_DESC, D.PART_LEVEL, D.ENGLISH_NAME, D.MAKER_SOURCE, D.MAKER_PART_NO, D.PART_SPEC, D.RELEASE_DATE, D.USAGE, D.EOL_STATUS, D.UNI_SPEC_STATUS, D.REMARK, 
                                                P.PRICE, P.PRICE_PM, P.MOQ, P.MOCKUP, P.TOOLING, P.SourcerAmount, P.PMAmount, P.CURRENCY, P.RATE, P.SOURCE_NO, P.SOURCE, D.MODIFY_TYPE, D.BU, NVL(P.ISModify, 'N') ISModify, D.PHASE_ID, P.REMARK_PUR, D.GROUP_DESC,  D.SN, decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                                D.UPDATED_DATE, D.UPDATED_BY, E.EMP_NAME, P.ISASSIGNER, P.ISCALCULATE, D.REMARK_PM
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                                JOIN GPO.C_PMS_BGBU_MAPPING M ON D.BU = M.BU
                                                JOIN GPO.RSI_C_PARTS_TYPE_DEF A ON A.ACTIVE = 'Y' 
                                                 AND A.BG = M.BG 
                                                 AND A.MTL_GROUP = D.MTL_GROUP 
                                                 AND DECODE(A.MTL_GROUP, 'RD DEFINE', '-', A.MTL_PARTS) = DECODE(D.MTL_GROUP, 'RD DEFINE', '-', D.MTL_PARTS)  --因為mtl_parts由RD自行輸入
                                                 AND D.PART_TYPE = A.PART_TYPE
                                                LEFT JOIN (SELECT DISTINCT D.MTL_GROUP,    --一般申請權限
                                                D.MTL_PARTS, 
                                                A.PART_TYPE_PUR AS PART_TYPE, 
                                                D.PART_NO, 
                                                D.PART_SPEC,
                                                D.USAGE, 
                                                D.PRICE, 
                                                D.PRICE_PM, 
                                                D.MOQ, 
                                                D.MOCKUP, 
                                                D.TOOLING, 
                                                nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                                nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,            
                                                NVL(D.SOURCE, '') AS SOURCE,  
                                                NVL(D.CURRENCY, '') AS CURRENCY,  
                                                NVL(D.RATE, 0) AS RATE,
                                                NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                                D.REMARK_PUR,
                                                'Y' ISModify,
                                                NVL(D.ISASSIGNER, 'N') AS ISASSIGNER,
                                                NVL(D.ISCALCULATE, 'N') AS ISCALCULATE
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D
                                                JOIN GPO.RSI_H_AUTHORITY H ON (D.BU = H.BU AND D.MTL_PARTS = H.MTL_PARTS)
                                                JOIN GPO.C_PMS_BGBU_MAPPING M ON D.BU = M.BU
                                                JOIN GPO.RSI_C_PARTS_TYPE_DEF A ON A.ACTIVE = 'Y' 
                                                 AND A.BG = M.BG 
                                                 AND A.MTL_GROUP = D.MTL_GROUP 
                                                 AND DECODE(A.MTL_GROUP, 'RD DEFINE', '-', A.MTL_PARTS) = DECODE(D.MTL_GROUP, 'RD DEFINE', '-', D.MTL_PARTS)
                                                 AND D.PART_TYPE = A.PART_TYPE
                                                WHERE D.RSI_NO = :rsi_no 
                                                AND D.MTL_TYPE = 'Normal' 
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')   
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                AND H.EMP_NO = :emp_no
                                                AND H.ACTIVE = 'Y'
                                                AND H.START_DATE <= SYSDATE
                                                AND H.END_DATE >= SYSDATE
                                                UNION   --若新增Material Group/Material Parts預設人員
                                                SELECT DISTINCT D.MTL_GROUP, 
                                                D.MTL_PARTS, 
                                                D.PART_TYPE, 
                                                D.PART_NO, 
                                                D.PART_SPEC,
                                                D.USAGE, 
                                                D.PRICE, 
                                                D.PRICE_PM, 
                                                D.MOQ, 
                                                D.MOCKUP, 
                                                D.TOOLING, 
                                                nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                                nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,            
                                                NVL(D.SOURCE, '') AS SOURCE,  
                                                NVL(D.CURRENCY, '') AS CURRENCY,  
                                                NVL(D.RATE, 0) AS RATE,
                                                NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                                D.REMARK_PUR,
                                                'Y' ISModify,
                                                NVL(D.ISASSIGNER, 'N') AS ISASSIGNER,
                                                NVL(D.ISCALCULATE, 'N') AS ISCALCULATE
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D
                                                JOIN GPO.RSI_H_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.MTL_PARTS) AND H.TYPE = 'DEFAULT'
                                                WHERE (1 = 1)
                                                --AND D.MODIFY_TYPE = 'NEW' 
                                                AND D.MTL_PARTS = 'RD DEFINE'
                                                AND D.RSI_NO = :rsi_no
                                                AND D.MTL_TYPE = 'Normal' 
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D') 
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                AND H.EMP_NO = :emp_no
                                                AND H.ACTIVE = 'Y'
                                                AND H.START_DATE <= SYSDATE
                                                AND H.END_DATE >= SYSDATE
                                                AND D.mtl_group ='RD DEFINE'
                                                ) P ON D.MTL_GROUP = P.MTL_GROUP AND nvl(D.MTL_PARTS,'-') = nvl(P.MTL_PARTS,'-') AND A.PART_TYPE_PUR = P.PART_TYPE AND NVL(D.PART_NO, D.PART_SPEC) = NVL(P.PART_NO, P.PART_SPEC)
                                                LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                                LEFT JOIN GFIN.EMP_DATA_ALL E ON E.EMP_NO = D.UPDATED_BY
                                                WHERE RSI_NO = :rsi_no  --'2018000001' 
                                                AND MTL_TYPE = 'Normal' 
                                                AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')
                                                AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'S')
                                                ORDER BY DECODE(ISModify, 'Y', 0, 1), PART_TYPE, D.MTL_GROUP, D.MTL_PARTS, PART_NO, SN ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_Normal_Sourcer", "取得H_Product_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static IList<H_Product_DetailEntity> Get_Normal_Sourcer_View(string rsi_no, string emp_no, string part_type)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select DISTINCT D.RSI_NO,
                                    D.MTL_TYPE,
                                    D.MTL_GROUP,
                                    D.PARTS_GROUP,
                                    D.MTL_PARTS,
                                    D.PART_TYPE,
                                    D.PARTNUMBER_PARENT,
                                    NVL(D.PART_NO, D.PART_LEVEL) as PART_NO,
                                    D.Part_Desc,
                                    D.PART_LEVEL,
                                    D.ENGLISH_NAME,
                                    D.Maker_Source,
                                    D.MAKER_PART_NO,
                                    D.PART_SPEC,
                                    D.Release_Date,
                                    D.USAGE,
                                    D.EOL_STATUS,
                                    D.Uni_Spec_Status,
                                    D.Remark,
                                    decode(d.ismodify, 'Y', d.price, null) as price,
                                    decode(d.ismodify, 'Y', d.price_pm, null) as price_pm,
                                    decode(d.ismodify, 'Y', d.MOQ, null) as MOQ,
                                    decode(d.ismodify, 'Y', d.MOCKUP, null) as MOCKUP,
                                    decode(d.ismodify, 'Y', d.TOOLING, null) as TOOLING,
                                    decode(d.ismodify, 'Y', d.FPCA_PCBA, null) as FPCA_PCBA,
                                    decode(d.ismodify, 'Y', nvl2(D.PRICE, D.USAGE * D.PRICE, null), null) as SourcerAmount,
                                    decode(d.ismodify, 'Y', nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null), null) as PMAmount,
                                    decode(d.ismodify, 'Y', NVL(D.CURRENCY, ''), null) as CURRENCY,
                                    decode(d.ismodify, 'Y', NVL(D.RATE, ''), null) as RATE,
                                    decode(d.ismodify, 'Y', NVL(D.SOURCE_NO, ''), null) as SOURCE_NO,
                                    decode(d.ismodify, 'Y', NVL(D.SOURCE, ''), null) as SOURCE,
                                    D.Modify_Type,
                                    D.bu,
                                    D.ismodify,
                                    D.Phase_Id,
                                    D.REMARK_PUR,
                                    D.Group_Desc,
                                    D.SN,
                                    decode(nvl(f.cnt, 0), 0, 'N', 'Y') as file_status,
                                    D.Updated_Date,
                                    D.Updated_By,
                                    E.EMP_NAME,
                                    decode(d.ismodify, 'Y', NVL(D.ISASSIGNER, 'N'), null) as ISASSIGNER,
                                    decode(d.ismodify, 'Y', NVL(D.ISCALCULATE, 'N'), null) as ISCALCULATE,
                                    D.Remark_Pm,
                                    nvl2(a.emp_no, a.emp_no || '/' || a.emp_name, null) as reassign_from,
                                    nvl2(b.emp_no, b.emp_no || '/' || b.emp_name, null) as reassign_to,
                                    au.created_date as reassign_date,
                                    D.Isapproved,
                                    nvl(d.sourcer_owner, s.owner) as sourcer_owner_id,
                                    nvl(e1.eng_name, s.owner_name) as sourcer_owner
                      from (SELECT d.*, nvl2(NULLIF(D.Part_Type, :part_type),'N',nvl2(h.emp_no,nvl2(NULLIF(d.sourcer_owner, h.emp_no), 'N', 'Y'),'N')) as ismodify
                              FROM GPO.RSI_H_PRODUCT_DETAIL D
                              left join (select distinct bu, mtl_parts, emp_no, rsi_no, sn
                                          from rsi_h_authority
                                         where active = 'Y'
                                           and start_date <= SYSDATE
                                           and end_date >= SYSDATE
                                           and emp_no = :emp_no) h
                                on h.bu = d.bu
                               and (h.mtl_parts =
                                   decode(d.mtl_parts, 'RD DEFINE', d.part_type, d.mtl_parts) or
                                   (h.mtl_parts = d.mtl_parts and h.rsi_no = d.rsi_no and
                                   h.sn = d.sn))
                             WHERE D.RSI_NO = :rsi_no
                               AND D.MTL_TYPE = 'Normal'
                               AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                               AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                               AND exists
                             (select 1
                                      from rsi_h_product_detail t
                                     where t.RSI_NO = d.rsi_no
                                       and (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                        OR d.parent_sn = 0
                                        OR d.parent_sn IS NULL)) d
                      LEFT JOIN (SELECT Biz_Id, count(1) AS cnt
                                   FROM GPO.RSI_H_FILELIST
                                  GROUP BY Biz_Id) f
                        ON f.BIZ_ID = d.RSI_NO || '_' || d.SN
                      LEFT JOIN GFIN.EMP_DATA_ALL E
                        ON E.EMP_NO = D.UPDATED_BY
                      LEFT JOIN GFIN.EMP_DATA_ALL E1
                        ON E1.EMP_NO = D.Sourcer_Owner
                      LEFT JOIN (select *
                                   from GPO.RSI_H_AUTHORITY
                                  where type = 'ASSIGN'
                                    and active = 'Y'
                                    and START_DATE <= SYSDATE
                                    AND END_DATE >= SYSDATE) AU
                        on AU.Rsi_No = D.Rsi_No
                       and au.sn = d.sn
                      left join gfin.emp_data_all a
                        on a.emp_no = AU.created_by
                      left join gfin.emp_data_all b
                        on b.emp_no = AU.emp_no
                      left join gpo.rsi_item_owner_v s
                        on s.rsi_no = d.rsi_no
                       and s.sn = d.sn
                     ORDER BY DECODE(d.ISModify, 'Y', 0, 1), d.PART_TYPE, d.MTL_GROUP, d.MTL_PARTS, NVL(D.PART_NO, D.PART_LEVEL), d.SN ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_Normal_Sourcer", "取得H_Product_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static IList<H_Product_DetailEntity> Get_Normal_Sourcer_Boss(string rsi_no, string emp_no, string part_type)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT RSI_NO, MTL_TYPE, D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, D.PART_TYPE,D.PARTNUMBER_PARENT, NVL(D.PART_NO, D.PART_LEVEL) AS PART_NO, D.PART_DESC, D.PART_LEVEL, D.ENGLISH_NAME, D.MAKER_SOURCE, D.MAKER_PART_NO, D.PART_SPEC, D.RELEASE_DATE, D.USAGE, D.EOL_STATUS, D.UNI_SPEC_STATUS, D.REMARK, 
                                        P.PRICE, P.PRICE_PM, P.MOQ, P.MOCKUP, P.TOOLING, P.FPCA_PCBA, P.SourcerAmount, P.PMAmount, P.CURRENCY, P.RATE, P.SOURCE_NO, P.SOURCE, D.MODIFY_TYPE, D.BU, NVL(P.ISModify, 'N') ISMODIFY, D.ISAPPROVED, D.PHASE_ID, P.REMARK_PUR,
                                        D.PRICE_HIS_H AS PRICE_HIS_H,
                                        D.PRICE_HIS_L AS PRICE_HIS_L,
                                        D.GROUP_DESC,
                                        D.SN, decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                        D.UPDATED_DATE, D.UPDATED_BY, E.EMP_NAME, D.MODIFY_TYPE, DECODE(D.ISCALCULATE, 'Y', 'Y', 'N') AS ISCALCULATE, D.REMARK_PM
                                        FROM GPO.RSI_H_PRODUCT_DETAIL D
                                        LEFT JOIN (SELECT DISTINCT D.MTL_GROUP, --一般申請權限
                                        D.PARTS_GROUP,
                                        D.MTL_PARTS, 
                                        D.PART_TYPE, 
                                        D.PARTNUMBER_PARENT,
                                        D.PART_NO, 
                                        D.PART_SPEC,
                                        D.USAGE, 
                                        D.SN,
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE, null) AS PRICE, 
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE_PM, null) AS PRICE_PM, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOQ, null) AS MOQ, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOCKUP, null) AS MOCKUP, 
                                        DECODE(D.ISCALCULATE, 'Y', D.TOOLING, null) AS TOOLING, 
                                        DECODE(D.ISCALCULATE, 'Y', D.FPCA_PCBA, null) AS FPCA_PCBA, 
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE, null) AS SourcerAmount,         
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE_PM, null) AS PMAmount,   
                                        NVL(D.SOURCE, '') AS SOURCE,  
                                        NVL(D.CURRENCY, '') AS CURRENCY,  
                                        NVL(D.RATE, 0) AS RATE,
                                        NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                        D.REMARK_PUR,
                                        'Y' ISModify
                                        FROM GPO.RSI_H_PRODUCT_DETAIL D
                                        JOIN GPO.RSI_H_PS_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE)  
                                        WHERE D.RSI_NO = :rsi_no  --'2018000001' 
                                        AND D.MTL_TYPE = 'Normal'
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')   
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                        AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                    OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                        --AND H.EMP_NO = :emp_no  --'1108633'
                                        AND H.EMP_NO IN (
                                                                          --以 登入主管的工號為主，捉出目前 RSI_H_PS_AUTHORITY 有權限的員工
                                                                            SELECT DISTINCT B.EMP_NO 
                                                                              FROM au_dw.auo_person_reportline A
                                                                             INNER JOIN GPO.RSI_H_PS_AUTHORITY B ON A.EMP_NO = B.EMP_NO
                                                                             WHERE (TRIM(A.BOSS_EMP_NO) = :emp_no OR B.EMP_NO = :emp_no) --EMP_NO
                                                                               AND A.ACTIVE = 'Y'
                                                                               AND B.ACTIVE = 'Y'
                                                                               AND SYSDATE >= B.START_DATE  
                                                                               AND B.END_DATE >= SYSDATE
                                                                              )
                                        AND H.ACTIVE = 'Y'
                                        AND H.START_DATE <= SYSDATE
                                        AND H.END_DATE >= SYSDATE
                                        AND D.PART_TYPE = :part_type
                                        UNION
                                        SELECT DISTINCT D.MTL_GROUP, --若新增Material Group/Material Parts預設指派人員
                                        D.PARTS_GROUP,
                                        D.MTL_PARTS, 
                                        D.PART_TYPE, 
                                        D.PARTNUMBER_PARENT,
                                        D.PART_NO, 
                                        D.PART_SPEC,
                                        D.USAGE,
                                        D.SN,
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE, null) AS PRICE, 
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE_PM, null) AS PRICE_PM,      
                                        DECODE(D.ISCALCULATE, 'Y', D.MOQ, null) AS MOQ, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOCKUP, null) AS MOCKUP, 
                                        DECODE(D.ISCALCULATE, 'Y', D.TOOLING, null) AS TOOLING,
                                        DECODE(D.ISCALCULATE, 'Y', D.FPCA_PCBA, null) AS FPCA_PCBA, 
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE, null) AS SourcerAmount,         
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE_PM, null) AS PMAmount,          
                                        NVL(D.SOURCE, '') AS SOURCE,  
                                        NVL(D.CURRENCY, '') AS CURRENCY,  
                                        NVL(D.RATE, 0) AS RATE,
                                        NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                        D.REMARK_PUR,
                                        'Y' ISModify
                                        FROM GPO.RSI_H_PRODUCT_DETAIL D
                                        JOIN GPO.RSI_H_PS_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE) AND H.TYPE = 'DEFAULT'
                                        WHERE (1 = 1)
                                        --AND D.MODIFY_TYPE = 'NEW' 
                                        AND D.MTL_PARTS = 'RD DEFINE'
                                        AND D.RSI_NO = :rsi_no --'2018000001' 
                                        AND D.MTL_TYPE = 'Normal' 
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')   
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                        AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                    OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                        --AND H.EMP_NO = :emp_no  --'1108633'
                                        AND H.EMP_NO IN (
                                                                          --以 登入主管的工號為主，捉出目前 RSI_H_PS_AUTHORITY 有權限的員工
                                                                            SELECT DISTINCT B.EMP_NO 
                                                                              FROM au_dw.auo_person_reportline A
                                                                             INNER JOIN GPO.RSI_H_PS_AUTHORITY B ON A.EMP_NO = B.EMP_NO
                                                                             WHERE (TRIM(A.BOSS_EMP_NO) = :emp_no OR B.EMP_NO = :emp_no) --EMP_NO
                                                                               AND A.ACTIVE = 'Y'
                                                                               AND B.ACTIVE = 'Y'
                                                                               AND SYSDATE >= B.START_DATE  
                                                                               AND B.END_DATE >= SYSDATE
                                                                              )
                                        AND H.ACTIVE = 'Y'
                                        AND H.START_DATE <= SYSDATE
                                        AND H.END_DATE >= SYSDATE 
                                        AND D.PART_TYPE = :part_type
                                       ) P 
                                       ON P.SN = D.SN
                                       LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                        ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                        LEFT JOIN GFIN.EMP_DATA_ALL E ON E.EMP_NO = D.UPDATED_BY
                                        WHERE RSI_NO = :rsi_no  --'2018000001' 
                                       AND MTL_TYPE = 'Normal' AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')
                                       AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                   OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                       ORDER BY DECODE(ISModify, 'Y', 0, 1), PART_TYPE, D.MTL_GROUP, D.MTL_PARTS, PART_NO, SN ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_Normal_Sourcer_Boss", "取得H_Product_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static IList<H_Product_DetailEntity> Get_Sourcer_Reassign_View(string rsi_no, string emp_no, string part_type)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select DISTINCT D.RSI_NO,
                                    D.MTL_TYPE,
                                    D.MTL_GROUP,
                                    D.PARTS_GROUP,
                                    D.MTL_PARTS,
                                    D.PART_TYPE,
                                    D.PARTNUMBER_PARENT,
                                    NVL(D.PART_NO, D.PART_LEVEL) as PART_NO,
                                    D.Part_Desc,
                                    D.PART_LEVEL,
                                    D.ENGLISH_NAME,
                                    D.Maker_Source,
                                    D.MAKER_PART_NO,
                                    D.PART_SPEC,
                                    D.Release_Date,
                                    D.USAGE,
                                    D.EOL_STATUS,
                                    D.Uni_Spec_Status,
                                    D.Remark,
                                    decode(d.ismodify, 'Y', d.price, null) as price,
                                    decode(d.ismodify, 'Y', d.price_pm, null) as price_pm,
                                    decode(d.ismodify, 'Y', d.MOQ, null) as MOQ,
                                    decode(d.ismodify, 'Y', d.MOCKUP, null) as MOCKUP,
                                    decode(d.ismodify, 'Y', d.TOOLING, null) as TOOLING,
                                    decode(d.ismodify, 'Y', d.FPCA_PCBA, null) as FPCA_PCBA,
                                    decode(d.ismodify, 'Y', nvl2(D.PRICE, D.USAGE * D.PRICE, null), null) as SourcerAmount,
                                    decode(d.ismodify, 'Y', nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null), null) as PMAmount,
                                    decode(d.ismodify, 'Y', NVL(D.CURRENCY, ''), null) as CURRENCY,
                                    decode(d.ismodify, 'Y', NVL(D.RATE, ''), null) as RATE,
                                    decode(d.ismodify, 'Y', NVL(D.SOURCE_NO, ''), null) as SOURCE_NO,
                                    decode(d.ismodify, 'Y', NVL(D.SOURCE, ''), null) as SOURCE,
                                    D.Modify_Type,
                                    D.bu,
                                    D.ismodify,
                                    D.Phase_Id,
                                    D.REMARK_PUR,
                                    D.Group_Desc,
                                    D.SN,
                                    decode(nvl(f.cnt, 0), 0, 'N', 'Y') as file_status,
                                    D.Updated_Date,
                                    D.Updated_By,
                                    E.EMP_NAME,
                                    decode(d.ismodify, 'Y', NVL(D.ISASSIGNER, 'N'), null) as ISASSIGNER,
                                    decode(d.ismodify, 'Y', NVL(D.ISCALCULATE, 'N'), null) as ISCALCULATE,
                                    D.Remark_Pm,
                                    nvl2(a.emp_no, a.emp_no || '/' || a.emp_name, null) as reassign_from,
                                    nvl2(b.emp_no, b.emp_no || '/' || b.emp_name, null) as reassign_to,
                                    au.created_date as reassign_date,
                                    D.Isapproved,
                                    nvl(d.sourcer_owner, s.owner) as sourcer_owner_id,
                                    nvl(e1.eng_name, s.owner_name) as sourcer_owner
                      from (SELECT d.*, nvl2(NULLIF(D.Part_Type, :part_type),'N',nvl2(h.emp_no,nvl2(NULLIF(d.sourcer_owner, h.emp_no), 'N', 'Y'),'N')) as ismodify
                              FROM GPO.RSI_H_PRODUCT_DETAIL D
                              left join (select distinct bu, mtl_parts, emp_no, rsi_no, sn
                                          from rsi_h_authority
                                         where active = 'Y'
                                           and start_date <= SYSDATE
                                           and end_date >= SYSDATE
                                           and emp_no = :emp_no) h
                                on h.bu = d.bu
                               and (h.mtl_parts =
                                   decode(d.mtl_parts, 'RD DEFINE', d.part_type, d.mtl_parts) or
                                   (h.mtl_parts = d.mtl_parts and h.rsi_no = d.rsi_no and
                                   h.sn = d.sn))
                             WHERE D.RSI_NO = :rsi_no
                               AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                               AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                               AND D.Mtl_Group not like 'MOH%'
                               AND exists
                             (select 1
                                      from rsi_h_product_detail t
                                     where t.RSI_NO = d.rsi_no
                                       and (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                        OR d.parent_sn = 0
                                        OR d.parent_sn IS NULL)) d
                      LEFT JOIN (SELECT Biz_Id, count(1) AS cnt
                                   FROM GPO.RSI_H_FILELIST
                                  GROUP BY Biz_Id) f
                        ON f.BIZ_ID = d.RSI_NO || '_' || d.SN
                      LEFT JOIN GFIN.EMP_DATA_ALL E
                        ON E.EMP_NO = D.UPDATED_BY
                      LEFT JOIN GFIN.EMP_DATA_ALL E1
                        ON E1.EMP_NO = D.Sourcer_Owner
                      LEFT JOIN (select *
                                   from GPO.RSI_H_AUTHORITY
                                  where type = 'ASSIGN'
                                    and active = 'Y'
                                    and START_DATE <= SYSDATE
                                    AND END_DATE >= SYSDATE) AU
                        on AU.Rsi_No = D.Rsi_No
                       and au.sn = d.sn
                      left join gfin.emp_data_all a
                        on a.emp_no = AU.created_by
                      left join gfin.emp_data_all b
                        on b.emp_no = AU.emp_no
                      left join gpo.rsi_item_owner_v s
                        on s.rsi_no = d.rsi_no
                       and s.sn = d.sn
                     ORDER BY DECODE(d.ISModify, 'Y', 0, 1), d.PART_TYPE, d.MTL_GROUP, d.MTL_PARTS, NVL(D.PART_NO, D.PART_LEVEL), d.SN ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_Sourcer_Reassign_View", "取得H_Product_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static IList<H_Product_DetailEntity> Get_PMConfrim_ExportData(string rsi_no)
        {
            try
            {
                IList<H_Product_DetailEntity> special_Details = new List<H_Product_DetailEntity>();
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT RSI_NO, MTL_TYPE, D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, A.PART_TYPE_PUR AS PART_TYPE, D.PART_NO, D.PART_DESC, D.PART_LEVEL, D.ENGLISH_NAME, D.MAKER_SOURCE, D.MAKER_PART_NO, D.PART_SPEC, D.RELEASE_DATE, D.USAGE, D.EOL_STATUS, D.UNI_SPEC_STATUS, D.REMARK, 
                                 DECODE(D.ISCALCULATE, 'Y', D.PRICE, null) AS PRICE, 
                                 DECODE(D.ISCALCULATE, 'Y', D.PRICE_PM, null) AS PRICE_PM, 
                                 DECODE(D.ISCALCULATE, 'Y', D.MOQ, null) AS MOQ, 
                                 DECODE(D.ISCALCULATE, 'Y', D.MOCKUP, null) AS MOCKUP, 
                                 DECODE(D.ISCALCULATE, 'Y', D.TOOLING, null) AS TOOLING, 
                                 DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE, null) AS SourcerAmount,         
                                 DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE_PM, null) AS PMAmount, 
                                 D.CURRENCY, D.RATE, D.SOURCE_NO, D.SOURCE, D.MODIFY_TYPE, D.BU, null ISMODIFY, D.PHASE_ID, D.REMARK_PUR,
                                 D.PRICE_HIS_H AS PRICE_HIS_H,
                                 D.PRICE_HIS_L AS PRICE_HIS_L,
                                 D.GROUP_DESC,
                                 D.SN, null FILE_STATUS,
                                 D.UPDATED_DATE, D.UPDATED_BY, E.EMP_NAME, D.MODIFY_TYPE, DECODE(D.ISCALCULATE, 'Y', 'Y', 'N') AS ISCALCULATE, D.REMARK_PM
                                 FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                 JOIN GPO.C_PMS_BGBU_MAPPING M ON D.BU = M.BU
                                 JOIN GPO.RSI_C_PARTS_TYPE_DEF A ON A.ACTIVE = 'Y' 
                                 AND A.BG = M.BG 
                                 AND A.MTL_GROUP = D.MTL_GROUP 
                                 AND DECODE(A.MTL_GROUP, 'RD DEFINE', '-', A.MTL_PARTS) = DECODE(D.MTL_GROUP, 'RD DEFINE', '-', D.MTL_PARTS)
                                 AND D.PART_TYPE = A.PART_TYPE
                                 LEFT JOIN GFIN.EMP_DATA_ALL E ON E.EMP_NO = D.UPDATED_BY
                                 WHERE RSI_NO = :rsi_no
                                 AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')
                                 ORDER BY NVL(GROUP_ID, 0), PART_TYPE, D.MTL_GROUP, D.MTL_PARTS, PART_NO, SN");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                return SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "Get_PMConfrim_ExportData");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<H_Product_DetailEntity> Get_PMConfirm_Special_ExportData(string rsi_no, string group_id)
        {
            try
            {
                IList<H_Product_DetailEntity> special_Details = new List<H_Product_DetailEntity>();
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select b.rsi_no,
                                 'Normal' mtl_type,
                                 '' mtl_group,
                                 '' parts_group,
                                 '' mtl_parts,
                                 '' part_type,
                                 b.partnumber_child as part_no,
                                 b.part_desc,
                                 b.part_level,
                                 b.english_name,
                                 '' maker_source,
                                 '' maker_part_no,
                                 '' part_spce,
                                 '' spec_def,
                                 b.release_date,
                                 b.part_quantity as usage,
                                 b.part_unit,
                                 '' eol_status,
                                 '' uni_spec_status,
                                 '' remark,
                                 '' modify_type,
                                 0 sn,
                                 '' file_status,
                                 '' phase_id,
                                 '' group_id,
                                 '' group_name,
                                 '' group_desc,
                                 b.partnumber_top,
                                 b.partnumber_child,
                                 b.partnumber_parent,
                                 0 PRICE,
                                 0 PRICE_PM,
                                 0 Moq,
                                 0 Mockup,
                                 0 Tooling,
                                 0 SourcerAmount,
                                 0 PMAmount,
                                 '' CURRENCY,
                                 0 RATE,
                                 '' SOURCE_NO,
                                 '' SOURCE,
                                 'N' ISModify,
                                 '' REMARK_PUR,
                                 'N' ISASSIGNER,
                                 'N' ISCALCULATE,
                                 b.partnumber_child as DisplayPartNo,
                                 0 bom_level
                                 from gpo.rsi_h_bom_all b
                                 where rsi_no = '20190225004' --rsi_no
                                 and bom_level = 0
                                 union all
                                 select *
                                   from (select d.*,
                                                LPAD(' ', (Level) * 4) || d.part_no as DisplayPartNo,
                                                Level bom_level
                                           from (select distinct d.rsi_no,
                                                                 nvl(d.mtl_type,
                                                                     decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                                                 d.mtl_group,
                                                                 d.parts_group,
                                                                 d.mtl_parts,
                                                                 d.part_type,
                                                                 d.part_no,
                                                                 d.part_desc,
                                                                 d.part_level,
                                                                 d.english_name,
                                                                 d.maker_source,
                                                                 d.maker_part_no,
                                                                 d.part_spec,
                                                                 c.SPEC_DEF,
                                                                 d.release_date,
                                                                 d.usage,
                                                                 d.part_unit,
                                                                 d.eol_status eol_status,
                                                                 d.uni_spec_status uni_spec_status,
                                                                 d.remark,
                                                                 d.modify_type,
                                                                 d.sn,
                                                                 NULL file_status,
                                                                 d.phase_id,
                                                                 d.group_id,
                                                                 d.group_name,
                                                                 d.group_desc,
                                                                 i.ref_product partnumber_top,
                                                                 d.part_no partnumber_child,
                                                                 d.partnumber_parent,
                                                                 d.PRICE,
                                                                 d.PRICE_PM,
                                                                 d.Moq,
                                                                 d.Mockup,
                                                                 d.Tooling,
                                                                 DECODE(D.PRICE,
                                                                        NULL,
                                                                        0,
                                                                        NVL(D.USAGE * NVL(D.PRICE, 0), 0)) AS SourcerAmount,
                                                                 DECODE(D.PRICE_PM,
                                                                        NULL,
                                                                        0,
                                                                        NVL(D.USAGE * NVL(D.PRICE_PM, 0), 0)) AS PMAmount,
                                                                 d.CURRENCY,
                                                                 d.RATE,
                                                                 d.SOURCE_NO,
                                                                 d.SOURCE,
                                                                 null AS ISModify,
                                                                 d.REMARK_PUR,
                                                                 d.ISASSIGNER,
                                                                 d.ISCALCULATE
                                                   from gpo.rsi_h_product_detail d
                                                   join gpo.rsi_h_product_info i
                                                     on d.rsi_no = i.rsi_no
                                                   left join gpo.rsi_c_mtl_special_parts s
                                                     on s.mtl_parts = d.mtl_parts
                                                    and s.active = 'Y'
                                                   left join (select distinct mtl_parts, spec_def
                                                               from gpo.rsi_c_mtl_spec_def
                                                              where active = 'Y') c
                                                     on d.mtl_parts = c.mtl_parts
                                                  where d.rsi_no = :rsi_no --rsi_no
                                                    and (d.modify_type is null or d.modify_type <> 'D')
                                                    and (d.group_id is null or
                                                        d.group_id = :group_id) --group_id
                                                 ) d
                                          start with partnumber_top = partnumber_parent
                                         connect by nocycle
                                          prior partnumber_top = partnumber_top
                                                and prior partnumber_child = partnumber_parent
                                          order siblings by part_level desc) a ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                return SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "Get_PMConfirm_Special_ExportData");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<H_Product_DetailEntity> Get_Special_Product_DetailEntities(string rsi_no, string part_type)
        {
            IList<H_Product_DetailEntity> special_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                //sqlText.Append(@"select distinct d.rsi_no, nvl(d.mtl_type,decode(s.active,'Y','Special','Normal')) mtl_type, p.mtl_group, p.mtl_parts, 
                //                                nvl(d.part_type,p.part_type) part_type, 
                //                                d.part_no, d.part_desc, 
                //                                nvl(d.part_level,r.part_level) PART_LEVEL, 
                //                                nvl(d.english_name, r.english_name) ENGLISH_NAME,
                //                                d.maker_source, d.maker_part_no, d.part_spec, c.SPEC_DEF,
                //                                d.release_date, d.usage, d.eol_status, d.uni_spec_status, d.remark, d.modify_type, d.sn, nvl2(f.file_id, 'Y', 'N') file_status,
                //                                nvl(d.group_id,(select distinct group_id
                //                                                            from RSI_H_PRODUCT_DETAIL g 
                //                                                            where g.rsi_no = :rsi_no and mtl_type = 'Special' and rownum = 1)) as group_id,
                //                                -- nvl(d.group_id, to_char(systimestamp, 'yyyymmddhh24missff6')) as group_id,
                //                                d.GROUP_DESC, d.GROUP_NAME
                //                                from RSI_C_PARTS_TYPE_DEF p
                //                                JOIN RSI_C_MTL_PARTS r
                //                                ON r.mtl_group = p.mtl_group AND r.mtl_parts = p.mtl_parts
                //                                join gpo.c_pms_bgbu_mapping m
                //                                on m.bg = p.bg
                //                                join RSI_H_PRODUCT_INFO i
                //                                on m.bu = i.bu
                //                                and i.rsi_no= :rsi_no
                //                                LEFT JOIN RSI_C_MTL_SPECIAL_PARTS s
                //                                ON s.mtl_parts = p.mtl_parts
                //                                LEFT JOIN (select * from RSI_H_PRODUCT_DETAIL where rsi_no= :rsi_no AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D' )) d --OR MODIFY_TYPE <> 'NEW'
                //                                on p.mtl_group = d.mtl_group
                //                                and p.mtl_parts = d.mtl_parts
                //                                LEFT JOIN (SELECT DISTINCT MTL_PARTS, SPEC_DEF from RSI_C_MTL_SPEC_DEF  WHERE ACTIVE = 'Y') c ON p.MTL_PARTS = c.MTL_PARTS
                //                                left join RSI_H_FILELIST f
                //                                on f.biz_id = d.rsi_no||'_'||d.sn
                //                                WHERE nvl(d.mtl_type,decode(s.active,'Y','Special','Normal')) = 'Special' ");

                sqlText.Append(@"select *
                     from (
                           --Special 組合
                           select distinct d.rsi_no,
                                            nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                            d.mtl_group,
                                            d.mtl_parts,
                                            d.part_type part_type,
                                            d.part_no,
                                            d.part_desc,
                                            nvl(d.part_level, r.part_level) PART_LEVEL,
                                            nvl(d.english_name, r.english_name) ENGLISH_NAME,
                                            d.maker_source,
                                            d.maker_part_no,
                                            d.part_spec,
                                            c.SPEC_DEF,
                                            d.release_date,
                                            d.usage,
                                            d.eol_status,
                                            d.uni_spec_status,
                                            d.remark,
                                            d.modify_type,
                                            d.sn,
                                            decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                            nvl(d.group_id, to_char(systimestamp, 'yyyymmddhh24missff6')) as group_id,
                                            d.GROUP_DESC,
                                            d.GROUP_NAME,
                                            d.phase_id
                             from RSI_H_PRODUCT_DETAIL d
                             JOIN RSI_C_MTL_PARTS r
                               ON r.mtl_group = d.mtl_group AND nvl(r.mtl_parts, '-') = nvl(d.mtl_parts, '-')
                             LEFT JOIN RSI_C_MTL_SPECIAL_PARTS s
                               ON s.mtl_parts = d.mtl_parts
                             LEFT JOIN (SELECT DISTINCT MTL_PARTS, SPEC_DEF
                                          from RSI_C_MTL_SPEC_DEF
                                         WHERE ACTIVE = 'Y') c
                               ON d.mtl_parts = c.mtl_parts
                             LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                            WHERE d.rsi_no = :rsi_no
                              and (d.MODIFY_TYPE IS NULL OR d.MODIFY_TYPE <> 'D')
                              and nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) = 'Special'
                           union --RD 自定義相目
                           select distinct d.rsi_no,
                                            nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                            d.mtl_group,
                                            d.mtl_parts,
                                            d.part_type part_type,
                                            d.part_no,
                                            d.part_desc,
                                            nvl(d.part_level, r.part_level) PART_LEVEL,
                                            nvl(d.english_name, r.english_name) ENGLISH_NAME,
                                            d.maker_source,
                                            d.maker_part_no,
                                            d.part_spec,
                                            c.SPEC_DEF,
                                            d.release_date,
                                            d.usage,
                                            d.eol_status,
                                            d.uni_spec_status,
                                            d.remark,
                                            d.modify_type,
                                            d.sn,
                                            decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                            nvl(d.group_id, to_char(systimestamp, 'yyyymmddhh24missff6')) as group_id,
                                            d.GROUP_DESC,
                                            d.GROUP_NAME,
                                            d.phase_id
                             from RSI_H_PRODUCT_DETAIL d
                             JOIN RSI_C_MTL_PARTS r
                               ON r.mtl_group = d.mtl_group
                             LEFT JOIN RSI_C_MTL_SPECIAL_PARTS s
                               ON s.mtl_parts = d.mtl_parts
                             LEFT JOIN (SELECT DISTINCT MTL_PARTS, SPEC_DEF
                                          from RSI_C_MTL_SPEC_DEF
                                         WHERE ACTIVE = 'Y') c
                               ON d.mtl_parts = c.mtl_parts
                             LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                            WHERE d.rsi_no = :rsi_no
                              --and d.MODIFY_TYPE = 'NEW'
                              AND D.MTL_PARTS = 'RD DEFINE'
                              and nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) = 'Special'
                      ) a ");

                string orderText = String.Empty;
                if (part_type.Equals("EE"))
                    orderText = "ORDER BY DECODE(MTL_GROUP || '-' || PART_TYPE, 'RD DEFINE-EE', 0), DECODE(PART_TYPE, 'EE', 1, 'ACD', 2, 'OM', 3, 'PACKING', 4, 5), MTL_GROUP, MTL_PARTS, PART_NO";
                if (part_type.Equals("OM"))
                    orderText = "ORDER BY DECODE(MTL_GROUP || '-' || PART_TYPE, 'RD DEFINE-OM', 0), DECODE(PART_TYPE, 'OM', 1, 'ACD', 2, 'EE', 3, 'PACKING', 4, 5), MTL_GROUP, MTL_PARTS, PART_NO";
                if (part_type.Equals("PACKING"))
                    orderText = "ORDER BY DECODE(MTL_GROUP || '-' || PART_TYPE, 'RD DEFINE-PACKING', 0), DECODE(PART_TYPE, 'PACKING', 1, 'ACD', 2, 'EE', 3, 'OM', 4, 5), MTL_GROUP, MTL_PARTS, PART_NO";
                if (part_type.Equals("ACD") || String.IsNullOrEmpty(orderText))
                    orderText = "ORDER BY DECODE(MTL_GROUP || '-' || PART_TYPE, 'RD DEFINE-ACD', 0), DECODE(PART_TYPE, 'ACD', 1, 'EE', 2, 'OM', 3, 'PACKING', 4, 5), MTL_GROUP, MTL_PARTS, PART_NO";

                sqlText.Append(orderText);
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));

                special_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_Special_Detail", "取得RD Special資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return special_Details;
            }
            return special_Details;
        }

        public static IList<H_Product_DetailEntity> Get_Special_Sourcer(string rsi_no, string emp_no, string part_type)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select DISTINCT D.RSI_NO,
                                    D.MTL_TYPE,
                                    D.MTL_GROUP,
                                    D.PARTS_GROUP,
                                    D.MTL_PARTS,
                                    D.PART_TYPE,
                                    D.PARTNUMBER_PARENT,
                                    NVL(D.PART_NO, D.PART_LEVEL) as PART_NO,
                                    D.Part_Desc,
                                    D.PART_LEVEL,
                                    D.ENGLISH_NAME,
                                    D.Maker_Source,
                                    D.MAKER_PART_NO,
                                    D.PART_SPEC,
                                    D.Release_Date,
                                    D.USAGE,
                                    D.EOL_STATUS,
                                    D.Uni_Spec_Status,
                                    D.Remark,
                                    decode(d.ismodify, 'Y', d.price, null) as price,
                                    decode(d.ismodify, 'Y', d.price_pm, null) as price_pm,
                                    decode(d.ismodify, 'Y', d.MOQ, null) as MOQ,
                                    decode(d.ismodify, 'Y', d.MOCKUP, null) as MOCKUP,
                                    decode(d.ismodify, 'Y', d.TOOLING, null) as TOOLING,
                                    decode(d.ismodify, 'Y', d.FPCA_PCBA, null) as FPCA_PCBA,
                                    decode(d.ismodify, 'Y', nvl2(D.PRICE, D.USAGE * D.PRICE, null), null) as SourcerAmount,
                                    decode(d.ismodify, 'Y', nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null), null) as PMAmount,
                                    decode(d.ismodify, 'Y', NVL(D.CURRENCY, ''), null) as CURRENCY,
                                    decode(d.ismodify, 'Y', NVL(D.RATE, ''), null) as RATE,
                                    decode(d.ismodify, 'Y', NVL(D.SOURCE_NO, ''), null) as SOURCE_NO,
                                    decode(d.ismodify, 'Y', NVL(D.SOURCE, ''), null) as SOURCE,
                                    D.Modify_Type,
                                    D.bu,
                                    D.ismodify,
                                    D.Phase_Id,
                                    D.REMARK_PUR,
                                    D.Group_id,
                                    D.Group_Desc,
                                    D.SN,
                                    decode(nvl(f.cnt, 0), 0, 'N', 'Y') as file_status,
                                    D.Updated_Date,
                                    D.Updated_By,
                                    E.EMP_NAME,
                                    decode(d.ismodify, 'Y', NVL(D.ISASSIGNER, 'N'), null) as ISASSIGNER,
                                    decode(d.ismodify, 'Y', NVL(D.ISCALCULATE, 'N'), null) as ISCALCULATE,
                                    D.Remark_Pm,
                                    nvl2(a.emp_no, a.emp_no || '/' || a.emp_name, null) as reassign_from,
                                    nvl2(b.emp_no, b.emp_no || '/' || b.emp_name, null) as reassign_to,
                                    au.created_date as reassign_date,
                                    D.Isapproved,
                                    nvl(d.sourcer_owner, s.owner) as sourcer_owner_id,
                                    nvl(e1.eng_name, s.owner_name) as sourcer_owner
                      from (SELECT d.*, nvl2(NULLIF(D.Part_Type, :part_type),'N',nvl2(h.emp_no,nvl2(NULLIF(d.sourcer_owner, h.emp_no), 'N', 'Y'),'N')) as ismodify
                              FROM GPO.RSI_H_PRODUCT_DETAIL D
                              left join (select distinct bu, mtl_parts, emp_no, rsi_no, sn
                                          from rsi_h_authority
                                         where active = 'Y'
                                           and start_date <= SYSDATE
                                           and end_date >= SYSDATE
                                           and emp_no = :emp_no) h
                                on h.bu = d.bu
                               and (h.mtl_parts =
                                   decode(d.mtl_parts, 'RD DEFINE', d.part_type, d.mtl_parts) or
                                   (h.mtl_parts = d.mtl_parts and h.rsi_no = d.rsi_no and
                                   h.sn = d.sn))
                             WHERE D.RSI_NO = :rsi_no
                               AND D.MTL_TYPE = 'Special'
                               AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                               AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                               AND exists
                             (select 1
                                      from rsi_h_product_detail t
                                     where t.RSI_NO = d.rsi_no
                                       and (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                        OR d.parent_sn = 0
                                        OR d.parent_sn IS NULL)) d
                      LEFT JOIN (SELECT Biz_Id, count(1) AS cnt
                                   FROM GPO.RSI_H_FILELIST
                                  GROUP BY Biz_Id) f
                        ON f.BIZ_ID = d.RSI_NO || '_' || d.SN
                      LEFT JOIN GFIN.EMP_DATA_ALL E
                        ON E.EMP_NO = D.UPDATED_BY
                      LEFT JOIN GFIN.EMP_DATA_ALL E1
                        ON E1.EMP_NO = D.Sourcer_Owner
                      LEFT JOIN (select *
                                   from GPO.RSI_H_AUTHORITY
                                  where type = 'ASSIGN'
                                    and active = 'Y'
                                    and START_DATE <= SYSDATE
                                    AND END_DATE >= SYSDATE) AU
                        on AU.Rsi_No = D.Rsi_No
                       and au.sn = d.sn
                      left join gfin.emp_data_all a
                        on a.emp_no = AU.created_by
                      left join gfin.emp_data_all b
                        on b.emp_no = AU.emp_no
                      left join gpo.rsi_item_owner_v s
                        on s.rsi_no = d.rsi_no
                       and s.sn = d.sn
                     ORDER BY GROUP_ID, DECODE(ISModify, 'Y', 0, 1), DECODE(substr(MTL_GROUP,1,3), 'MOH', 99, 0), MTL_PARTS, PART_NO
                     ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_Special_Sourcer", "取得H_Product_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static IList<H_Product_DetailEntity> Get_Special_Sourcer_Boss(string rsi_no, string emp_no, string part_type)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT D.RSI_NO, D.Group_ID, D.MTL_TYPE, D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, D.PART_TYPE, D.PARTNUMBER_PARENT, NVL(D.PART_NO, D.PART_LEVEL) as PART_NO, D.PART_DESC, D.PART_LEVEL, D.ENGLISH_NAME, D.MAKER_SOURCE, D.MAKER_PART_NO, D.PART_SPEC, D.RELEASE_DATE, D.USAGE, D.EOL_STATUS, D.UNI_SPEC_STATUS, D.REMARK, 
                                        P.PRICE, P.PRICE_PM, P.MOQ, P.MOCKUP, P.TOOLING, P.FPCA_PCBA, P.SourcerAmount, P.PMAmount, P.CURRENCY, P.RATE, P.SOURCE_NO, P.SOURCE, MODIFY_TYPE, D.BU, P.ISModify, D.PHASE_ID, P.REMARK_PUR, 
                                        D.PRICE_HIS_H AS PRICE_HIS_H,
                                        D.PRICE_HIS_L AS PRICE_HIS_L,
                                        D.GROUP_DESC,
                                        D.SN, 
                                        D.GROUP_NAME,
                                        decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                        D.UPDATED_DATE, D.UPDATED_BY, E.EMP_NAME, D.MODIFY_TYPE, DECODE(D.ISCALCULATE, 'Y', 'Y', 'N') AS ISCALCULATE, D.REMARK_PM, D.ISAPPROVED
                                        FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                        JOIN GPO.C_PMS_BGBU_MAPPING M ON D.BU = M.BU
                                        LEFT JOIN (SELECT DISTINCT D.MTL_GROUP,  --一般申請權限
                                        D.PARTS_GROUP,
                                        D.MTL_PARTS, 
                                        D.PART_TYPE, 
                                        D.PARTNUMBER_PARENT,
                                        D.PART_NO, 
                                        D.USAGE, 
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE, null) AS PRICE, 
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE_PM, null) AS PRICE_PM,      
                                        DECODE(D.ISCALCULATE, 'Y', D.MOQ, null) AS MOQ, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOCKUP, null) AS MOCKUP, 
                                        DECODE(D.ISCALCULATE, 'Y', D.TOOLING, null) AS TOOLING, 
                                        DECODE(D.ISCALCULATE, 'Y', D.FPCA_PCBA, null) AS FPCA_PCBA,
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE, null) AS SourcerAmount,         
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE_PM, null) AS PMAmount,         
                                        NVL(D.SOURCE, '') AS SOURCE,  
                                        NVL(D.CURRENCY, '') AS CURRENCY,  
                                        NVL(D.RATE, 0) AS RATE,
                                        NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                        D.REMARK_PUR,
                                        D.GROUP_ID,
                                        D.SN,
                                        'Y' ISModify      
                                        FROM GPO.RSI_H_PRODUCT_DETAIL D
                                        JOIN GPO.RSI_H_PS_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE)
                                        JOIN GPO.C_PMS_BGBU_MAPPING M ON D.BU = M.BU
                                        JOIN GPO.RSI_C_PARTS_TYPE_DEF A ON A.ACTIVE = 'Y' 
                                         AND A.BG = M.BG 
                                         AND A.MTL_GROUP = D.MTL_GROUP 
                                         AND DECODE(A.MTL_GROUP, 'RD DEFINE', '-', A.MTL_PARTS) = DECODE(D.MTL_GROUP, 'RD DEFINE', '-', D.MTL_PARTS)
                                         AND D.PART_TYPE = A.PART_TYPE
                                        WHERE D.RSI_NO = :rsi_no
                                        AND D.MTL_TYPE = 'Special' AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')   
                                        --AND D.BU = H.BU
                                        --AND D.MTL_PARTS = H.MTL_PARTS
                                        --AND H.EMP_NO = :emp_no  --'0505500'
                                        AND H.EMP_NO IN (
                                                                  --以 登入主管的工號為主，捉出目前 RSI_H_PS_AUTHORITY 有權限的員工
                                                                    SELECT DISTINCT B.EMP_NO 
                                                                      FROM au_dw.auo_person_reportline A
                                                                     INNER JOIN GPO.RSI_H_PS_AUTHORITY B ON A.EMP_NO = B.EMP_NO
                                                                     WHERE (TRIM(A.BOSS_EMP_NO) = :emp_no OR B.EMP_NO = :emp_no) --EMP_NO
                                                                       AND A.ACTIVE = 'Y'
                                                                       AND B.ACTIVE = 'Y'
                                                                       AND SYSDATE >= B.START_DATE  
                                                                       AND B.END_DATE >= SYSDATE
                                                                      )
                                        AND H.ACTIVE = 'Y'
                                        AND H.START_DATE <= SYSDATE
                                        AND H.END_DATE >= SYSDATE
                                        AND D.PART_TYPE = :part_type
                                        UNION
                                        SELECT DISTINCT D.MTL_GROUP,  --若新增Material Group/Material Parts預設指派人員
                                        D.PARTS_GROUP,
                                        D.MTL_PARTS, 
                                        D.PART_TYPE, 
                                        D.PARTNUMBER_PARENT,
                                        D.PART_NO, 
                                        D.USAGE, 
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE, null) AS PRICE, 
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE_PM, null) AS PRICE_PM, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOQ, null) AS MOQ, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOCKUP, null) AS MOCKUP, 
                                        DECODE(D.ISCALCULATE, 'Y', D.TOOLING, null) AS TOOLING, 
                                        DECODE(D.ISCALCULATE, 'Y', D.FPCA_PCBA, null) AS FPCA_PCBA,
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE, null) AS SourcerAmount,         
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE_PM, null) AS PMAmount,    
                                        NVL(D.SOURCE, '') AS SOURCE,  
                                        NVL(D.CURRENCY, '') AS CURRENCY,  
                                        NVL(D.RATE, 0) AS RATE,
                                        NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                        D.REMARK_PUR,
                                        D.GROUP_ID,
                                        D.SN,
                                        'Y' ISModify      
                                        FROM GPO.RSI_H_PRODUCT_DETAIL D
                                        JOIN GPO.RSI_H_PS_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE) AND H.TYPE = 'DEFAULT'
                                        WHERE (1 = 1)
                                        --AND D.MODIFY_TYPE = 'NEW' 
                                        AND D.MTL_PARTS = 'RD DEFINE'
                                        AND D.RSI_NO = :rsi_no
                                        AND D.MTL_TYPE = 'Special' 
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')   
                                        --AND H.EMP_NO = :emp_no  --'0505500'
                                        AND H.EMP_NO IN (
                                                                  --以 登入主管的工號為主，捉出目前 RSI_H_PS_AUTHORITY 有權限的員工
                                                                    SELECT DISTINCT B.EMP_NO 
                                                                     FROM au_dw.auo_person_reportline A
                                                                     INNER JOIN GPO.RSI_H_PS_AUTHORITY B ON A.EMP_NO = B.EMP_NO
                                                                     WHERE (TRIM(A.BOSS_EMP_NO) = :emp_no OR B.EMP_NO = :emp_no) --EMP_NO
                                                                       AND A.ACTIVE = 'Y'
                                                                       AND B.ACTIVE = 'Y'
                                                                       AND SYSDATE >= B.START_DATE  
                                                                       AND B.END_DATE >= SYSDATE
                                                                      )
                                        AND H.ACTIVE = 'Y'
                                        AND H.START_DATE <= SYSDATE
                                        AND H.END_DATE >= SYSDATE
                                        AND D.PART_TYPE = :part_type
                                        ) P ON D.SN = P.SN
                                        AND D.GROUP_ID = P.GROUP_ID
                                        LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                        ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                        LEFT JOIN GFIN.EMP_DATA_ALL E ON E.EMP_NO = D.UPDATED_BY
                                        WHERE RSI_NO = :rsi_no  ----'2018000001' 
                                        AND MTL_TYPE = 'Special' AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')
                                        ORDER BY RSI_NO, GROUP_ID, DECODE(ISModify, 'Y', 0, 1), DECODE(substr(MTL_GROUP,1,3), 'MOH', 99, 0), MTL_PARTS, PART_NO ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_Special_Sourcer_Boss", "取得H_Product_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static IList<H_Product_DetailEntity> Get_PM_Portfolio_Detail(string rsi_no, string group_id)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT D.RSI_NO, D.Group_ID, D.PART_TYPE, D.MTL_TYPE, D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, D.PARTNUMBER_PARENT, NVL(D.PART_NO, D.PART_LEVEL) AS PART_NO, D.PART_DESC, 
                                                D.ENGLISH_NAME, D.PART_SPEC, D.UNI_SPEC_STATUS, D.EOL_STATUS, D.USAGE,
                                                DECODE(D.ISCALCULATE, 'Y', NVL(D.PRICE_PM, 0), 0) AS PRICE_PM,
                                                DECODE(D.ISCALCULATE, 'Y', NVL(D.MOQ, 0), 0) AS MOQ,
                                                DECODE(D.ISCALCULATE, 'Y', NVL(D.MOCKUP, 0), 0) AS MOCKUP,
                                                DECODE(D.ISCALCULATE, 'Y', NVL(D.TOOLING, 0), 0) AS TOOLING,
                                                DECODE(D.ISCALCULATE, 'Y', NVL(D.FPCA_PCBA, 0), 0) AS FPCA_PCBA,
                                                DECODE(D.ISCALCULATE, 'Y', NVL(D.USAGE * NVL(D.PRICE_PM, 0), 0), 0) AS PMAmount,
                                                D.MODIFY_TYPE, H.PRICE_GROUP, D.REMARK_PM
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                                JOIN GPO.RSI_H_FORM_HEADER H ON D.RSI_NO = H.RSI_NO AND D.PART_TYPE = H.PART_TYPE
                                                WHERE D.RSI_NO = :rsi_no  --從querystring的rsi_no取得 
                                                AND (D.GROUP_ID IS NULL OR D.GROUP_ID = :group_id) --)  --從querystring的GROUP_ID取得 
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                             OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                                ORDER BY D.RSI_NO, DECODE(D.PART_TYPE, 'ACD', 0, 'TPACD', 1, 'EE', 2, 'TPEE', 3, 'OM', 4, 'TPOM', 5, 'PACKING', 6, 7), D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, D.MTL_TYPE, NVL(D.PART_NO, D.PART_LEVEL) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_PM_Portfolio_Detail", "取得Portfolio_Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static bool Delete_Product_Detail(H_Product_DetailEntity h_Product_Detail, string group_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" UPDATE GPO.RSI_H_PRODUCT_DETAIL SET 
                                                MODIFY_TYPE = 'D',
                                                UPDATED_DATE = SYSDATE,
                                                UPDATED_BY = :emp_no
                                                WHERE RSI_NO = :RSI_NO 
                                                AND SN = :SN ");
                if (!string.IsNullOrEmpty(group_id))
                    sqlText.Append("AND GROUP_ID = :group_id ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("RSI_NO", OracleDbType.Varchar2, h_Product_Detail.RSI_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("SN", OracleDbType.Varchar2, h_Product_Detail.SN, ParameterDirection.Input));
                if (!string.IsNullOrEmpty(group_id))
                    param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RSI_H_Product_Detail", "Delete 方法");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
            return result;
        }

        public static bool Create_Product_Detail(H_Product_DetailEntity h_Product_Detail)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"INSERT INTO GPO.RSI_H_PRODUCT_DETAIL(
                                                RSI_NO, MTL_TYPE, MTL_GROUP, MTL_PARTS, PART_TYPE, PART_NO, PART_DESC, 
                                                PART_LEVEL, ENGLISH_NAME, MAKER_SOURCE, MAKER_PART_NO, 
                                                PART_SPEC, RELEASE_DATE, USAGE, EOL_STATUS, UNI_SPEC_STATUS, 
                                                REMARK, MODIFY_TYPE, BU, Group_ID, CREATED_DATE, CREATED_BY, PART_UNIT, PARTNUMBER_PARENT,
                                                GROUP_NAME, GROUP_DESC, PARTS_GROUP, PARENT_SN, SN, PART_NO_ORI)
                                                VALUES(
                                                :RSI_NO, :MTL_TYPE, :MTL_GROUP, :MTL_PARTS, :PART_TYPE, :PART_NO, :PART_DESC, 
                                                :PART_LEVEL, :ENGLISH_NAME, substr(:MAKER_SOURCE, 0, 20), :MAKER_PART_NO, 
                                                :PART_SPEC, to_date(:RELEASE_DATE, 'yyyy/mm/dd'), :USAGE, :EOL_STATUS, :UNI_SPEC_STATUS, 
                                                :REMARK, :MODIFY_TYPE, :BU, :Group_ID, sysdate, :emp_no, :PART_UNIT, :PARTNUMBER_PARENT,
                                                :GROUP_NAME, :GROUP_DESC, :PARTS_GROUP, :PARENT_SN, :SN, :PART_NO)");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("RSI_NO", OracleDbType.Varchar2, h_Product_Detail.RSI_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("MTL_TYPE", OracleDbType.Varchar2, h_Product_Detail.MTL_TYPE, ParameterDirection.Input));
                param.Add(new OracleParameter("MTL_GROUP", OracleDbType.Varchar2, h_Product_Detail.MTL_GROUP, ParameterDirection.Input));
                param.Add(new OracleParameter("MTL_PARTS", OracleDbType.Varchar2, h_Product_Detail.MTL_PARTS, ParameterDirection.Input));
                param.Add(new OracleParameter("PART_TYPE", OracleDbType.Varchar2, h_Product_Detail.PART_TYPE, ParameterDirection.Input));
                param.Add(new OracleParameter("PART_NO", OracleDbType.Varchar2, h_Product_Detail.PART_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("PART_DESC", OracleDbType.Varchar2, h_Product_Detail.PART_DESC, ParameterDirection.Input));
                param.Add(new OracleParameter("PART_LEVEL", OracleDbType.Varchar2, h_Product_Detail.PART_LEVEL, ParameterDirection.Input));
                param.Add(new OracleParameter("ENGLISH_NAME", OracleDbType.Varchar2, h_Product_Detail.ENGLISH_NAME, ParameterDirection.Input));
                param.Add(new OracleParameter("MAKER_SOURCE", OracleDbType.Varchar2, h_Product_Detail.MAKER_SOURCE == null ? DBNull.Value : (Object)h_Product_Detail.MAKER_SOURCE.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("MAKER_PART_NO", OracleDbType.Varchar2, h_Product_Detail.MAKER_PART_NO == null ? DBNull.Value : (Object)h_Product_Detail.MAKER_PART_NO.Trim(), ParameterDirection.Input));
                param.Add(new OracleParameter("PART_SPEC", OracleDbType.Varchar2, h_Product_Detail.PART_SPEC, ParameterDirection.Input));
                param.Add(new OracleParameter("RELEASE_DATE", OracleDbType.Varchar2, h_Product_Detail.RELEASE_DATE.Year == 1 ? DBNull.Value : (object)h_Product_Detail.RELEASE_DATE.ToString("yyyy/MM/dd"), ParameterDirection.Input));
                param.Add(new OracleParameter("USAGE", OracleDbType.Varchar2, h_Product_Detail.USAGE, ParameterDirection.Input));
                param.Add(new OracleParameter("EOL_STATUS", OracleDbType.Varchar2, h_Product_Detail.EOL_STATUS, ParameterDirection.Input));
                param.Add(new OracleParameter("UNI_SPEC_STATUS", OracleDbType.Varchar2, h_Product_Detail.UNI_SPEC_STATUS, ParameterDirection.Input));
                param.Add(new OracleParameter("REMARK", OracleDbType.Varchar2, h_Product_Detail.REMARK, ParameterDirection.Input));
                param.Add(new OracleParameter("MODIFY_TYPE", OracleDbType.Varchar2, h_Product_Detail.MODIFY_TYPE, ParameterDirection.Input));
                param.Add(new OracleParameter("BU", OracleDbType.Varchar2, h_Product_Detail.BU, ParameterDirection.Input));
                param.Add(new OracleParameter("Group_ID", OracleDbType.Varchar2, h_Product_Detail.GROUP_ID, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("PART_UNIT", OracleDbType.Varchar2, h_Product_Detail.PART_UNIT, ParameterDirection.Input));
                param.Add(new OracleParameter("PARTNUMBER_PARENT", OracleDbType.Varchar2, h_Product_Detail.PARTNUMBER_PARENT, ParameterDirection.Input));
                param.Add(new OracleParameter("GROUP_NAME", OracleDbType.Varchar2, h_Product_Detail.GROUP_NAME, ParameterDirection.Input));
                param.Add(new OracleParameter("GROUP_DESC", OracleDbType.Varchar2, h_Product_Detail.GROUP_DESC, ParameterDirection.Input));
                param.Add(new OracleParameter("PARTS_GROUP", OracleDbType.Varchar2, h_Product_Detail.PARTS_GROUP, ParameterDirection.Input));
                param.Add(new OracleParameter("PARENT_SN", OracleDbType.Varchar2, h_Product_Detail.PARENT_SN == null ? DBNull.Value : (object)h_Product_Detail.PARENT_SN, ParameterDirection.Input));
                param.Add(new OracleParameter("SN", OracleDbType.Varchar2, h_Product_Detail.SN, ParameterDirection.Input));
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RSI_H_Product_Detail", "Insert 方法");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
            return result;
        }

        public static bool Update_Product_Detail(H_Product_DetailEntity h_Product_Detail)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE GPO.RSI_H_PRODUCT_DETAIL SET 
                                PRICE = :PRICE,
                                PRICE_PM = :PRICE_PM,
                                MOQ = :MOQ,
                                MOCKUP = :MOCKUP,
                                TOOLING = :TOOLING,
                                FPCA_PCBA = :FPCA_PCBA,
                                CURRENCY = :CURRENCY,
                                RATE = :RATE,
                                SOURCE_NO = :SOURCE_NO,
                                SOURCE = :SOURCE,
                                REMARK_PM = :REMARK_PM,
                                REMARK_PUR = :REMARK_PUR,
                                UPDATED_DATE = SYSDATE, 
                                UPDATED_BY = :emp_no,
                                PRICE_HIS_H = :price_his_h,
                                PRICE_HIS_L = :price_his_l,
                                ISASSIGNER = :isassigner,
                                ISCALCULATE = :iscalculate
                                WHERE RSI_NO = :RSI_NO 
                                AND MTL_TYPE = :MTL_TYPE
                                AND SN = :SN ");
                if (h_Product_Detail.MTL_TYPE.Equals("Special"))
                {
                    sqlText.Append("AND GROUP_ID = :GROUP_ID");
                }

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("PRICE", OracleDbType.Varchar2, h_Product_Detail.PRICE, ParameterDirection.Input));
                param.Add(new OracleParameter("PRICE_PM", OracleDbType.Varchar2, h_Product_Detail.PRICE_PM, ParameterDirection.Input));
                param.Add(new OracleParameter("MOQ", OracleDbType.Varchar2, h_Product_Detail.MOQ, ParameterDirection.Input));
                param.Add(new OracleParameter("MOCKUP", OracleDbType.Varchar2, h_Product_Detail.MOCKUP, ParameterDirection.Input));
                param.Add(new OracleParameter("TOOLING", OracleDbType.Varchar2, h_Product_Detail.TOOLING, ParameterDirection.Input));
                param.Add(new OracleParameter("FPCA_PCBA", OracleDbType.Varchar2, h_Product_Detail.FPCA_PCBA, ParameterDirection.Input));
                param.Add(new OracleParameter("CURRENCY", OracleDbType.Varchar2, h_Product_Detail.CURRENCY, ParameterDirection.Input));
                param.Add(new OracleParameter("RATE", OracleDbType.Varchar2, h_Product_Detail.RATE, ParameterDirection.Input));
                param.Add(new OracleParameter("SOURCE_NO", OracleDbType.Varchar2, h_Product_Detail.SOURCE_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("SOURCE", OracleDbType.Varchar2, h_Product_Detail.SOURCE, ParameterDirection.Input));
                param.Add(new OracleParameter("REMARK_PM", OracleDbType.Varchar2, h_Product_Detail.REMARK_PM, ParameterDirection.Input));
                param.Add(new OracleParameter("REMARK_PUR", OracleDbType.Varchar2, h_Product_Detail.REMARK_PUR, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("price_his_h", OracleDbType.Varchar2, h_Product_Detail.PRICE_HIS_H, ParameterDirection.Input));
                param.Add(new OracleParameter("price_his_l", OracleDbType.Varchar2, h_Product_Detail.PRICE_HIS_L, ParameterDirection.Input));
                param.Add(new OracleParameter("isassigner", OracleDbType.Varchar2, h_Product_Detail.ISASSIGNER, ParameterDirection.Input));
                param.Add(new OracleParameter("iscalculate", OracleDbType.Varchar2, h_Product_Detail.ISCALCULATE, ParameterDirection.Input));
                param.Add(new OracleParameter("RSI_NO", OracleDbType.Varchar2, h_Product_Detail.RSI_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("MTL_TYPE", OracleDbType.Varchar2, h_Product_Detail.MTL_TYPE, ParameterDirection.Input));
                param.Add(new OracleParameter("SN", OracleDbType.Varchar2, h_Product_Detail.SN, ParameterDirection.Input));
                if (h_Product_Detail.MTL_TYPE.Equals("Special"))
                {
                    param.Add(new OracleParameter("GROUP_ID", OracleDbType.Varchar2, h_Product_Detail.GROUP_ID, ParameterDirection.Input));
                }
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RSI_H_Product_Detail", "Update方法");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
            return result;

        }

        public static IList<string> GetEOL(string item_no)
        {
            IList<string> EOLs = new List<string>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT NVL(SUBSTR(MATERIAL_STATUS,0,1), '') EOL_STATUS  
                                                --FROM AU_DW.VSPO_DW_PHASE_OUT_MAPPING 
                                                FROM GPO.RSI_R_MTL_PHASE_OUT
                                                WHERE ITEM_NO = :item_no");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("item_no", OracleDbType.Varchar2, item_no, ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetEOL", "取得EOL資訊");
                EOLs = dt.AsEnumerable().Select(p => p.Field<string>("EOL_STATUS")).ToList();
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return EOLs;
            }
            return EOLs;
        }

        public static IList<string> GetUNI_SPEC_STATUS(string item_no)
        {
            IList<string> Uni_Spec_Status = new List<string>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM RSI_C_UNIQUE_SPEC 
                                                WHERE PART_NO = :item_no AND ACTIVE = 'Y'");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("item_no", OracleDbType.Varchar2, item_no, ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetUNI_SPEC_STATUS", "取得UNI_SPEC_STATUS資訊");
                Uni_Spec_Status = dt.AsEnumerable().Select(p => p.Field<string>("active")).ToList();
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return Uni_Spec_Status;
            }
            return Uni_Spec_Status;
        }

        public static IList<string> GetGroupID(string rsi_no)
        {
            IList<string> group_id = new List<string>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT GROUP_ID
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                                WHERE RSI_NO = :rsi_no   --querystring取的
                                                AND GROUP_ID IS NOT NULL
                                                AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')
                                                ORDER BY GROUP_ID ASC ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetGroupID", "取得Group_ID資訊");
                group_id = dt.AsEnumerable().Select(p => p.Field<string>("GROUP_ID")).ToList();
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return group_id;
            }
            return group_id;
        }

        public static void UpdateSpecialDesc(string rsi_no, string group_id, string group_name, string group_desc)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE GPO.RSI_H_PRODUCT_DETAIL SET 
                                                GROUP_DESC = :group_desc,
                                                GROUP_NAME = substr(:group_name, 0, 10)
                                                WHERE RSI_NO = :rsi_no AND GROUP_ID = :group_id ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("group_desc", OracleDbType.Varchar2, group_desc, ParameterDirection.Input));
                param.Add(new OracleParameter("group_name", OracleDbType.Varchar2, group_name, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateSpecialDesc", "更新special desc資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
        }

        public static string GetGroupName(string rsi_no, string group_id)
        {
            IList<string> groupnames = new List<string>();
            string groupname = String.Empty;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT GROUP_NAME
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                                WHERE RSI_NO = :rsi_no   --querystring取的
                                                AND (GROUP_ID IS NULL OR GROUP_ID = :group_id)
                                                AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')
                                                ORDER BY GROUP_NAME ASC");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetGroupName", "取得Group_Name資訊");
                groupnames = dt.AsEnumerable().Select(p => p.Field<string>("GROUP_NAME")).ToList();
                groupname = groupnames.FirstOrDefault() == null ? string.Empty : groupnames.FirstOrDefault();
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return groupname;
            }
            return groupname;
        }

        public static string GetGroupDesc(string rsi_no, string group_id)
        {
            IList<string> groupdescs = new List<string>();
            string groupdesc = String.Empty;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT GROUP_DESC
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                                WHERE RSI_NO = :rsi_no   --querystring取的
                                                AND (GROUP_ID IS NULL OR GROUP_ID = :group_id)
                                                AND (MODIFY_TYPE IS NULL OR MODIFY_TYPE <> 'D')
                                                ORDER BY GROUP_DESC ASC");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetGroupName", "取得Group_Name資訊");
                groupdescs = dt.AsEnumerable().Select(p => p.Field<string>("GROUP_DESC")).ToList();
                groupdesc = groupdescs.FirstOrDefault() == null ? String.Empty : groupdescs.FirstOrDefault();
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return groupdesc;
            }
            return groupdesc;
        }

        public static void UpdateForDeleteSpecialPart(string group_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE GPO.RSI_H_PRODUCT_DETAIL SET 
                                                MODIFY_TYPE = 'D',
                                                UPDATED_DATE = SYSDATE,
                                                UPDATED_BY = :emp_no
                                                WHERE GROUP_ID = :group_id ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateForDeleteSpecialPart", "更新 Modify_type資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
        }

        public static int GetNumberForCheckAllPMPrice(string rsi_no, string part_type)
        {
            int result = 0;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT COUNT(*) AS COUNT 
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D
                                                WHERE D.RSI_NO = :rsi_no
                                                AND D.PART_TYPE = :part_type
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')  
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                AND (D.PHASE_ID IS NULL OR D.PHASE_ID = '40') 
                                                AND NVL(D.MTL_PARTS,'-') <> '-'  --節點不需要認領
                                                AND NVL(D.MTL_GROUP, '-') not like  'MOH%'   --MOH材料由Product Sourcer負責
                                                AND (D.ISAPPROVED IS NULL OR D.ISAPPROVED <> 'Y') ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetNumberForCheckAllPMPrice", "取得GetNumber資訊");
                var results = dt.AsEnumerable().Select(p => p.Field<decimal>("COUNT")).ToList();
                result = Convert.ToInt32(results.FirstOrDefault());
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return result;
            }
            return result;
        }

        public static int GetSpecialNameRepeat(string rsi_no, string group_name)
        {
            int result = 0;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct group_name from rsi_h_product_detail
                                                where rsi_no = :rsi_no
                                                and mtl_type = 'Special'
                                                and group_name = :group_name ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_name", OracleDbType.Varchar2, group_name, ParameterDirection.Input));

                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetSpecialNameRepeat", "取得Special Name資訊");
                result = dt.Rows.Count;
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return result;
            }
            return result;

        }

        public static void Delete_Product_Detail(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"delete from RSI_H_PRODUCT_DETAIL 
                                                where rsi_no = :rsi_no and part_type = 'OM' --and mtl_type = 'Special' 
                                                and part_no is null and part_spec is null ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Delete_Product_Detail", "刪除special空白資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
        }

        public static IList<H_Product_DetailEntity> Get_New_Product_Detail(string rsi_no, string part_type)
        {
            IList<H_Product_DetailEntity> h_Product_Details = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct d.rsi_no,
                                                nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                                d.mtl_group,
                                                d.mtl_parts,
                                                d.part_type,
                                                d.part_no,
                                                d.part_desc,
                                                nvl(d.part_level, r.part_level) PART_LEVEL,
                                                nvl(d.english_name, r.english_name) ENGLISH_NAME,
                                                d.maker_source,
                                                d.maker_part_no,
                                                d.part_spec,
                                                c.SPEC_DEF,
                                                d.release_date,
                                                d.usage,
                                                d.eol_status,
                                                d.uni_spec_status,
                                                d.remark,
                                                d.modify_type,
                                                d.sn,
                                                decode(nvl(f.cnt,0),0,'N','Y') as file_status
                                                from RSI_H_PRODUCT_DETAIL d
                                                LEFT JOIN RSI_C_MTL_PARTS r
                                                ON r.mtl_group = d.mtl_group
                                                AND r.mtl_parts = d.mtl_parts
                                                LEFT JOIN RSI_C_MTL_SPECIAL_PARTS s
                                                ON s.mtl_parts = d.mtl_parts
                                                LEFT JOIN (SELECT DISTINCT MTL_PARTS, SPEC_DEF
                                                from RSI_C_MTL_SPEC_DEF
                                                WHERE ACTIVE = 'Y') c
                                                ON d.MTL_PARTS = c.MTL_PARTS
                                                LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                                WHERE d.rsi_no = :rsi_no
                                                --and d.MODIFY_TYPE = 'NEW'
                                                AND D.MTL_PARTS = 'RD DEFINE'
                                                and nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) <> 'Special' ");
                string orderText = String.Empty;
                if (part_type.Equals("EE"))
                    orderText = "ORDER BY DECODE(PART_TYPE, 'EE', 0, 'ACD', 1, 'OM', 2, 'PACKING', 3, 4), d.MTL_GROUP, d.MTL_PARTS, PART_NO";
                if (part_type.Equals("OM"))
                    orderText = "ORDER BY DECODE(PART_TYPE, 'OM', 0, 'ACD', 1, 'EE', 2, 'PACKING', 3, 4), d.MTL_GROUP, d.MTL_PARTS, PART_NO";
                if (part_type.Equals("PACKING"))
                    orderText = "ORDER BY DECODE(PART_TYPE, 'PACKING', 0, 'ACD', 1, 'EE', 2, 'OM', 3, 4), d.MTL_GROUP, d.MTL_PARTS, PART_NO";
                if (part_type.Equals("ACD") || String.IsNullOrEmpty(orderText))
                    orderText = "ORDER BY DECODE(PART_TYPE, 'ACD', 0, 'EE', 1, 'OM', 2, 'PACKING', 3, 4), d.MTL_GROUP, d.MTL_PARTS, PART_NO";
                sqlText.Append(orderText);

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));

                h_Product_Details = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "Get_New_Product_Detail", "取得New Product Detail資訊");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return h_Product_Details;
            }
            return h_Product_Details;
        }

        public static void Update_Product_Detail_For_Layer1(H_Product_DetailEntity h_Product_Detail)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update rsi_h_product_detail set 
                                                part_no = :part_no, 
                                                part_desc = :part_desc,
                                                maker_source = :maker_source,
                                                maker_part_no = :maker_part_no,
                                                release_date = to_date(:release_date, 'yyyy/mm/dd'),
                                                part_spec = :part_spec, 
                                                usage = :usage, 
                                                remark = :remark, 
                                                updated_date = sysdate,
                                                updated_by = :emp_no,
                                                modify_type = 'A'
                                               where rsi_no = :rsi_no and part_type = :part_type and sn = :sn ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("part_no", OracleDbType.Varchar2, h_Product_Detail.PART_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("part_desc", OracleDbType.Varchar2, h_Product_Detail.PART_DESC, ParameterDirection.Input));
                param.Add(new OracleParameter("maker_source", OracleDbType.Varchar2, h_Product_Detail.MAKER_SOURCE, ParameterDirection.Input));
                param.Add(new OracleParameter("maker_part_no", OracleDbType.Varchar2, h_Product_Detail.MAKER_PART_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("release_date", OracleDbType.Varchar2, h_Product_Detail.RELEASE_DATE.Year == 1 ? DBNull.Value : (object)h_Product_Detail.RELEASE_DATE.ToString("yyyy/MM/dd"), ParameterDirection.Input));
                param.Add(new OracleParameter("part_spec", OracleDbType.Varchar2, h_Product_Detail.PART_SPEC, ParameterDirection.Input));
                param.Add(new OracleParameter("usage", OracleDbType.Varchar2, h_Product_Detail.USAGE, ParameterDirection.Input));
                param.Add(new OracleParameter("remark", OracleDbType.Varchar2, h_Product_Detail.REMARK, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, h_Product_Detail.RSI_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, h_Product_Detail.PART_TYPE, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Varchar2, h_Product_Detail.SN, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Product_Detail_For_Layer1", "Update方法");
                //list = SqlExcute.GetOraDateTable(sqlText,param.ToArray(), "GetWIPInfo", "获取ERP工单数据").ToList<WIPEntity>();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
        }

        public static void Update_Product_Detail_For_Layer1_Submit(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update rsi_h_product_detail set phase_id = '' where rsi_no = :rsi_no and part_type = :part_type");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Product_Detail_For_Layer1_Submit", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<H_Product_DetailEntity> GetSourcerManagerDetail(string rsi_no, string emp_no, string group_id)
        {
            IList<H_Product_DetailEntity> model = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" SELECT DISTINCT D.RSI_NO, D.GROUP_ID, D.PART_TYPE, D.MTL_TYPE, D.MTL_GROUP, D.MTL_PARTS, D.PARTS_GROUP, NVL(D.PART_NO, D.PART_LEVEL) as PART_NO, D.PART_DESC,
                                    D.ENGLISH_NAME, D.PART_SPEC, D.UNI_SPEC_STATUS, D.EOL_STATUS, D.USAGE,                    
                                    DECODE(D.ISCALCULATE, 'Y', NVL(D.PRICE, 0), 0) AS PRICE_SOURCER, 
                                    DECODE(D.ISCALCULATE, 'Y', DECODE(D.PRICE, NULL, 0, NVL(D.USAGE * NVL(D.PRICE, 0), 0)), 0) AS SourcerAmount,
                                    DECODE(D.ISCALCULATE, 'Y', NVL(D.PRICE_PM, 0), 0) AS PRICE_PM,
                                    DECODE(D.ISCALCULATE, 'Y', NVL(D.MOQ, 0), 0) AS MOQ,
                                    DECODE(D.ISCALCULATE, 'Y', NVL(D.MOCKUP, 0), 0) AS MOCKUP,
                                    DECODE(D.ISCALCULATE, 'Y', NVL(D.TOOLING, 0), 0) AS TOOLING,
                                    DECODE(D.ISCALCULATE, 'Y', DECODE(D.PRICE_PM, NULL, 0, NVL(D.USAGE * NVL(D.PRICE_PM, 0), 0)), 0) AS PMAmount,
                                    ROUND(NVL(DECODE(D.ISCALCULATE, 'Y', DECODE(D.PRICE_PM, NULL, 0, NVL(D.USAGE * NVL(D.PRICE_PM, 0), 0)), 0), 0) - NVL(DECODE(D.ISCALCULATE, 'Y', DECODE(D.PRICE, NULL, 0, NVL(D.USAGE * NVL(D.PRICE, 0), 0)), 0), 0), 4) AS GAP,  -- PM_Price - Sourcer_Price                        
                                    'Y' ISMODIFY,
                                    D.ISCALCULATE,
                                    D.MODIFY_TYPE,
                                    D.REMARK_PM,
                                    D.SN
                                    FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                    WHERE D.RSI_NO = :rsi_no  --從querystring的rsi_no取得 
                                    AND (D.GROUP_ID IS NULL OR D.GROUP_ID = :group_id) --)  --從querystring的GROUP_ID取得 
                                    AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                                    AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                    AND NVL(D.MTL_PARTS,'-') <> '-'  --節點不需要認領
                                    AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                OR d.parent_sn = 0 OR d.parent_sn IS NULL) 
                                    ORDER BY D.RSI_NO, DECODE(D.PART_TYPE, 'ACD', 0, 'EE', 1, 'OM', 2, 'PACKING', 3, 4), D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, D.MTL_TYPE, NVL(D.PART_NO, D.PART_LEVEL) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                model = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetSourcerManagerDetail", "取得Sourcer Manager Detail資訊");
            }
            catch (Exception ex)
            {
                return model;
            }

            return model;
        }

        public static IList<H_Product_DetailEntity> GetRDReviewNormalParts(string rsi_no, string part_type)
        {
            IList<H_Product_DetailEntity> models = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select b.rsi_no, 'Normal' mtl_type, '' mtl_group, '' parts_group, '' mtl_parts, '' part_type,
                                                decode(ismodify, 'Y', substr(b.partnumber_child, 0, 2), b.partnumber_child) as part_no,
                                                b.part_desc, b.part_level, b.english_name, '' maker_source, '' maker_part_no, '' part_spec,
                                                '' spec_def, b.release_date, b.part_quantity as usage, b.part_unit, '' eol_status,
                                                '' uni_spec_status, '' remark, decode(ismodify, 'Y', 'U', null) as modify_type, 0 sn, 0 parent_sn, '' file_status, '' phase_id,
                                                b.partnumber_top, decode(ismodify, 'Y', substr(b.partnumber_child, 0, 2), b.partnumber_child) as partnumber_child,
                                                b.partnumber_parent, decode(ismodify, 'Y', substr(b.partnumber_child, 0, 2), b.partnumber_child) as DisplayPartNo,
                                                0 bom_level
                                                from gpo.rsi_h_bom_all b
                                                where rsi_no = :rsi_no
                                                and bom_level = 0
                                                --and not exists (select 1 from rsi_h_product_detail d where d.rsi_no = :rsi_no)
                                                union all
                                                select *
                                                from (
                                                    select d.*, LPAD(' ', (Level) * 4) || nvl(d.part_no, d.part_level) as DisplayPartNo, Level bom_level
                                                    from (
                                                        select distinct d.rsi_no,
                                                        nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                                        d.mtl_group,
                                                        d.parts_group,
                                                        d.mtl_parts,
                                                        d.part_type, 
                                                        d.part_no,
                                                        d.part_desc,
                                                        d.part_level,
                                                        d.english_name,
                                                        d.maker_source,
                                                        d.maker_part_no,
                                                        d.part_spec,
                                                        c.SPEC_DEF,
                                                        d.release_date,
                                                        d.usage,
                                                        d.part_unit,
                                                        d.eol_status eol_status,
                                                        d.uni_spec_status uni_spec_status,
                                                        d.remark,
                                                        d.modify_type,
                                                        d.sn,
                                                        d.parent_sn,
                                                        decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                                        d.phase_id,
                                                        i.ref_product partnumber_top,
                                                        nvl(d.part_no, d.part_level) partnumber_child, 
                                                        d.partnumber_parent
                                                        from gpo.rsi_h_product_detail d
                                                        join gpo.rsi_h_product_info i on d.rsi_no = i.rsi_no
                                                        left join gpo.rsi_c_mtl_special_parts s on s.mtl_parts = d.mtl_parts and s.active = 'Y'
                                                        left join (select distinct mtl_parts, spec_def
                                                                     from gpo.rsi_c_mtl_spec_def
                                                                    where active = 'Y') c
                                                          on d.mtl_parts = c.mtl_parts
                                                        left join (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                          on f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                                        where nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) <> 'Special' 
                                                        and (d.modify_type is null or d.modify_type <> 'D')
                                                        and nvl(d.mtl_group,'-') not like 'MOH%'
                                                        and d.rsi_no = :rsi_no
                                                    ) d
                                                    start with partnumber_top = partnumber_parent
                                                    connect by nocycle prior partnumber_top = partnumber_top
                                                                    and prior sn = parent_sn
                                                                   --and prior partnumber_child = partnumber_parent
                                                    order siblings by part_level desc
                                                ) a ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                models = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetRDReviewNormalParts", "取得Normal Parts資訊");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return models;
        }

        public static IList<H_Product_DetailEntity> GetBOMTableDataForSourcer(string rsi_no, string part_type, string phase_id, string emp_no)
        {
            IList<H_Product_DetailEntity> models = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select b.rsi_no,
                                        'Normal' mtl_type,
                                        '' mtl_group,
                                        '' parts_group,
                                        '' mtl_parts,
                                        '' part_type,
                                        decode(ismodify,
                                                'Y',
                                                substr(b.partnumber_child, 0, 2),
                                                b.partnumber_child) as part_no,
                                        b.part_desc,
                                        b.part_level,
                                        b.english_name,
                                        '' maker_source,
                                        '' maker_part_no,
                                        '' part_spec,
                                        '' spec_def,
                                        b.release_date,
                                        b.part_quantity as usage,
                                        b.part_unit,
                                        '' eol_status,
                                        '' uni_spec_status,
                                        '' remark,
                                        decode(ismodify, 'Y', 'U', null) as modify_type,
                                        0 sn,
                                        0 parent_sn,
                                        '' file_status,
                                        '' phase_id,
                                        b.partnumber_top,
                                        decode(ismodify,
                                                'Y',
                                                substr(b.partnumber_child, 0, 2),
                                                b.partnumber_child) as partnumber_child,
                                        b.partnumber_parent,
                                        decode(ismodify,
                                                'Y',
                                                substr(b.partnumber_child, 0, 2),
                                                b.partnumber_child) as DisplayPartNo,
                                        0 bom_level,
                                        0 as seq,
                                        null PRICE_PM,
                                        null PMAmount,
                                        null ISAPPROVED,
                                        null ISCALCULATE,
                                        null ISModify
                                    from gpo.rsi_h_bom_all b
                                    where rsi_no = :rsi_no
                                    and bom_level = 0
                                union all
                                select b.*,
                                        P.PRICE_PM,
                                        P.PMAmount,
                                        P.ISAPPROVED,
                                        P.ISCALCULATE,
                                        NVL(P.ISModify, 'N') ISModify
                                    from (select a.*, ROWNUM as seq
                                            from (select d.*,
                                                        LPAD(' ', (Level) * 4) || nvl(d.part_no, d.part_level) as DisplayPartNo,
                                                        Level bom_level
                                                    from (select distinct d.rsi_no,
                                                                        nvl(d.mtl_type,
                                                                            decode(s.active,
                                                                                    'Y',
                                                                                    'Special',
                                                                                    'Normal')) mtl_type,
                                                                        d.mtl_group,
                                                                        d.parts_group,
                                                                        d.mtl_parts,
                                                                        d.part_type,
                                                                        d.part_no,
                                                                        d.part_desc,
                                                                        d.part_level,
                                                                        d.english_name,
                                                                        d.maker_source,
                                                                        d.maker_part_no,
                                                                        d.part_spec,
                                                                        c.SPEC_DEF,
                                                                        d.release_date,
                                                                        d.usage,
                                                                        d.part_unit,
                                                                        d.eol_status eol_status,
                                                                        d.uni_spec_status uni_spec_status,
                                                                        d.remark,
                                                                        d.modify_type,
                                                                        d.sn,
                                                                        d.parent_sn,
                                                                        decode(nvl(f.cnt, 0), 0, 'N', 'Y') as file_status,
                                                                        d.phase_id,
                                                                        i.ref_product partnumber_top,
                                                                        nvl(d.part_no, d.part_level) partnumber_child,
                                                                        d.partnumber_parent
                                                            from gpo.rsi_h_product_detail d
                                                            join gpo.rsi_h_product_info i
                                                            on d.rsi_no = i.rsi_no
                                                            left join gpo.rsi_c_mtl_special_parts s
                                                            on s.mtl_parts = d.mtl_parts
                                                            and s.active = 'Y'
                                                            left join (select distinct mtl_parts, spec_def
                                                                        from gpo.rsi_c_mtl_spec_def
                                                                        where active = 'Y') c
                                                            on d.mtl_parts = c.mtl_parts
                                                            left join (SELECT Biz_Id, count(1) AS cnt
                                                                        FROM GPO.RSI_H_FILELIST
                                                                        GROUP BY Biz_Id) f
                                                            on f.BIZ_ID = d.RSI_NO || '_' || d.SN
                                                            where nvl(d.mtl_type,
                                                                    decode(s.active, 'Y', 'Special', 'Normal')) <>
                                                                'Special'
                                                            and (d.modify_type is null or d.modify_type <> 'D')
                                                            and nvl(d.mtl_group, '-') not like 'MOH%'
                                                            and d.rsi_no = :rsi_no) d
                                                    start with partnumber_top = partnumber_parent
                                                connect by nocycle prior partnumber_top = partnumber_top
                                                        and prior sn = parent_sn
                                                    order siblings by part_level desc) a) b ");
                if (phase_id == "40")
                {
                    sqlText.Append(@"    LEFT JOIN (SELECT DISTINCT D.MTL_GROUP, --一般申請權限
                                                                D.MTL_PARTS,
                                                                D.PART_TYPE,
                                                                D.PARTNUMBER_PARENT,
                                                                D.PART_NO,
                                                                D.PART_SPEC,
                                                                D.USAGE,
                                                                D.PRICE,
                                                                D.PRICE_PM,
                                                                D.MOQ,
                                                                D.MOCKUP,
                                                                D.TOOLING,
                                                                D.FPCA_PCBA,
                                                                D.SN,
                                                                nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                                                nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,
                                                                NVL(D.SOURCE, '') AS SOURCE,
                                                                NVL(D.CURRENCY, '') AS CURRENCY,
                                                                NVL(D.RATE, 0) AS RATE,
                                                                NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                                                D.REMARK_PUR,
                                                                'Y' ISModify,
                                                                NVL(D.ISAPPROVED, 'N') AS ISAPPROVED,
                                                                NVL(D.ISCALCULATE, 'N') AS ISCALCULATE
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D
                                                JOIN GPO.RSI_H_AUTHORITY H
                                                    ON D.BU = H.BU
                                                AND D.MTL_PARTS = H.MTL_PARTS
                                                and nvl(h.rsi_no, d.rsi_no) = d.rsi_no
                                                and nvl(h.sn, d.sn) = d.sn
                                                WHERE D.RSI_NO = :rsi_no
                                                AND D.MTL_TYPE = 'Normal'
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                AND exists
                                                (select 1
                                                        from rsi_h_product_detail t
                                                        where (t.sn = d.parent_sn and
                                                            nvl(t.modify_type, '-') <> 'D')
                                                            OR d.parent_sn = 0
                                                            OR d.parent_sn IS NULL)
                                                AND H.EMP_NO = :emp_no
                                                AND H.ACTIVE = 'Y'
                                                AND H.START_DATE <= SYSDATE
                                                AND H.END_DATE >= SYSDATE
                                                UNION --若新增Material Group/Material Parts預設人員
                                                SELECT DISTINCT D.MTL_GROUP,
                                                                D.MTL_PARTS,
                                                                D.PART_TYPE,
                                                                D.PARTNUMBER_PARENT,
                                                                D.PART_NO,
                                                                D.PART_SPEC,
                                                                D.USAGE,
                                                                D.PRICE,
                                                                D.PRICE_PM,
                                                                D.MOQ,
                                                                D.MOCKUP,
                                                                D.TOOLING,
                                                                D.FPCA_PCBA,
                                                                D.SN,
                                                                nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                                                nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,
                                                                NVL(D.SOURCE, '') AS SOURCE,
                                                                NVL(D.CURRENCY, '') AS CURRENCY,
                                                                NVL(D.RATE, 0) AS RATE,
                                                                NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                                                D.REMARK_PUR,
                                                                'Y' ISModify,
                                                                NVL(D.ISAPPROVED, 'N') AS ISAPPROVED,
                                                                NVL(D.ISCALCULATE, 'N') AS ISCALCULATE
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D
                                                JOIN GPO.RSI_H_AUTHORITY H
                                                    ON (D.BU = H.BU AND D.PART_TYPE = H.MTL_PARTS)
                                                AND H.TYPE = 'DEFAULT'
                                                WHERE (1 = 1)
                                                AND D.MTL_PARTS = 'RD DEFINE'
                                                AND D.RSI_NO = :rsi_no
                                                AND D.MTL_TYPE = 'Normal'
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                AND exists (select 1
                                                        from rsi_h_product_detail t
                                                        where (t.sn = d.parent_sn and
                                                            nvl(t.modify_type, '-') <> 'D')
                                                            OR d.parent_sn = 0
                                                            OR d.parent_sn IS NULL)
                                                AND H.EMP_NO = :emp_no
                                                AND H.ACTIVE = 'Y'
                                                AND H.START_DATE <= SYSDATE
                                                AND H.END_DATE >= SYSDATE
                                                AND NOT exists --RD DEFINE已轉簽 
                                                (select 1
                                                        from GPO.RSI_H_AUTHORITY au
                                                        where au.type = 'ASSIGN'
                                                        and au.rsi_no = d.rsi_no
                                                        and au.sn = d.sn
                                                        and au.START_DATE <= SYSDATE
                                                        AND au.END_DATE >= SYSDATE
                                                        and au.active = 'Y'
                                                        and au.emp_no <> h.emp_no)
                                                ) P
                                    ON P.PART_TYPE = :part_type
                                    AND b.SN = P.SN
                                    order by seq
                ");
                }

                if (phase_id == "45")
                {
                    sqlText.Append(@" LEFT JOIN(
                                                --若Product Sourcer權限人員
                                                SELECT DISTINCT D.MTL_GROUP, 
                                                D.PARTS_GROUP,
                                                D.MTL_PARTS, 
                                                D.PART_TYPE, 
                                                D.PART_NO, 
                                                D.PART_SPEC,
                                                D.USAGE, 
                                                D.PRICE, 
                                                D.PRICE_PM, 
                                                D.MOQ, 
                                                D.MOCKUP, 
                                                D.TOOLING,
                                                D.FPCA_PCBA,
                                                nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                                nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,
                                                NVL(D.SOURCE, '') AS SOURCE,
                                                NVL(D.CURRENCY, '') AS CURRENCY,
                                                NVL(D.RATE, 0) AS RATE,
                                                NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                                D.REMARK_PUR,
                                                'Y' ISModify,
                                                NVL(D.ISASSIGNER, 'N') AS ISASSIGNER,
                                                NVL(D.ISAPPROVED, 'N') AS ISAPPROVED,
                                                NVL(D.ISCALCULATE, 'N') AS ISCALCULATE, D.SN
                                                FROM GPO.RSI_H_PRODUCT_DETAIL D
                                                JOIN GPO.RSI_H_PS_AUTHORITY H ON(D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE)
                                                WHERE(1 = 1)
                                                AND D.RSI_NO = :rsi_no
                                                AND D.MTL_TYPE = 'Normal'
                                                AND(D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                                                AND(D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                AND exists(select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                             OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                                AND H.EMP_NO = :emp_no
                                                AND H.ACTIVE = 'Y'
                                                AND H.START_DATE <= SYSDATE
                                                AND H.END_DATE >= SYSDATE
                                                AND D.PART_TYPE = :part_type
                                                ) P ON b.SN = P.SN order by seq
                               ");
                }

                if (phase_id == "50" || phase_id == "60")
                {
                    sqlText.Append(@" LEFT JOIN (SELECT DISTINCT D.MTL_GROUP, --一般申請權限
                                        D.PARTS_GROUP,
                                        D.MTL_PARTS, 
                                        D.PART_TYPE, 
                                        D.PARTNUMBER_PARENT,
                                        D.PART_NO, 
                                        D.PART_SPEC,
                                        D.USAGE, 
                                        D.SN,
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE, null) AS PRICE, 
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE_PM, null) AS PRICE_PM, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOQ, null) AS MOQ, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOCKUP, null) AS MOCKUP, 
                                        DECODE(D.ISCALCULATE, 'Y', D.TOOLING, null) AS TOOLING, 
                                        DECODE(D.ISCALCULATE, 'Y', D.FPCA_PCBA, null) AS FPCA_PCBA, 
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE, null) AS SourcerAmount,         
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE_PM, null) AS PMAmount,   
                                        NVL(D.SOURCE, '') AS SOURCE,  
                                        NVL(D.CURRENCY, '') AS CURRENCY,  
                                        NVL(D.RATE, 0) AS RATE,
                                        NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                        D.REMARK_PUR,
                                        'Y' ISModify,
                                        NVL(D.ISAPPROVED, 'N') AS ISAPPROVED,
                                        NVL(D.ISCALCULATE, 'N') AS ISCALCULATE
                                        FROM GPO.RSI_H_PRODUCT_DETAIL D
                                        JOIN GPO.RSI_H_PS_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE)  
                                        WHERE D.RSI_NO = :rsi_no
                                        AND D.MTL_TYPE = 'Normal'
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')   
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                        AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                    OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                        AND H.EMP_NO IN (--以 登入主管的工號為主，捉出目前 RSI_H_PS_AUTHORITY 有權限的員工
                                                                            SELECT DISTINCT B.EMP_NO 
                                                                              FROM au_dw.auo_person_reportline A
                                                                             INNER JOIN GPO.RSI_H_PS_AUTHORITY B ON A.EMP_NO = B.EMP_NO
                                                                             WHERE (TRIM(A.BOSS_EMP_NO) = :emp_no OR B.EMP_NO = :emp_no) --EMP_NO
                                                                               AND A.ACTIVE = 'Y'
                                                                               AND B.ACTIVE = 'Y'
                                                                               AND SYSDATE >= B.START_DATE  
                                                                               AND B.END_DATE >= SYSDATE)
                                        AND H.ACTIVE = 'Y'
                                        AND H.START_DATE <= SYSDATE
                                        AND H.END_DATE >= SYSDATE
                                        AND D.PART_TYPE = :part_type
                                        UNION
                                        SELECT DISTINCT D.MTL_GROUP, --若新增Material Group/Material Parts預設指派人員
                                        D.PARTS_GROUP,
                                        D.MTL_PARTS, 
                                        D.PART_TYPE, 
                                        D.PARTNUMBER_PARENT,
                                        D.PART_NO, 
                                        D.PART_SPEC,
                                        D.USAGE,
                                        D.SN,
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE, null) AS PRICE, 
                                        DECODE(D.ISCALCULATE, 'Y', D.PRICE_PM, null) AS PRICE_PM,      
                                        DECODE(D.ISCALCULATE, 'Y', D.MOQ, null) AS MOQ, 
                                        DECODE(D.ISCALCULATE, 'Y', D.MOCKUP, null) AS MOCKUP, 
                                        DECODE(D.ISCALCULATE, 'Y', D.TOOLING, null) AS TOOLING,
                                        DECODE(D.ISCALCULATE, 'Y', D.FPCA_PCBA, null) AS FPCA_PCBA, 
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE, null) AS SourcerAmount,         
                                        DECODE(D.ISCALCULATE, 'Y', D.USAGE * D.PRICE_PM, null) AS PMAmount,          
                                        NVL(D.SOURCE, '') AS SOURCE,  
                                        NVL(D.CURRENCY, '') AS CURRENCY,  
                                        NVL(D.RATE, 0) AS RATE,
                                        NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                        D.REMARK_PUR,
                                        'Y' ISModify,
                                        NVL(D.ISAPPROVED, 'N') AS ISAPPROVED,
                                        NVL(D.ISCALCULATE, 'N') AS ISCALCULATE
                                        FROM GPO.RSI_H_PRODUCT_DETAIL D
                                        JOIN GPO.RSI_H_PS_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE) AND H.TYPE = 'DEFAULT'
                                        WHERE (1 = 1)
                                        AND D.MTL_PARTS = 'RD DEFINE'
                                        AND D.RSI_NO = :rsi_no
                                        AND D.MTL_TYPE = 'Normal' 
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')   
                                        AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                        AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                    OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                        AND H.EMP_NO IN (
                                                                          --以 登入主管的工號為主，捉出目前 RSI_H_PS_AUTHORITY 有權限的員工
                                                                            SELECT DISTINCT B.EMP_NO 
                                                                              FROM au_dw.auo_person_reportline A
                                                                             INNER JOIN GPO.RSI_H_PS_AUTHORITY B ON A.EMP_NO = B.EMP_NO
                                                                             WHERE (TRIM(A.BOSS_EMP_NO) = :emp_no OR B.EMP_NO = :emp_no)
                                                                               AND A.ACTIVE = 'Y'
                                                                               AND B.ACTIVE = 'Y'
                                                                               AND SYSDATE >= B.START_DATE  
                                                                               AND B.END_DATE >= SYSDATE
                                                                              )
                                        AND H.ACTIVE = 'Y'
                                        AND H.START_DATE <= SYSDATE
                                        AND H.END_DATE >= SYSDATE 
                                        AND D.PART_TYPE = :part_type
                                       ) P 
                                       ON P.SN = b.SN  order by seq
                    ");
                }
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                models = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetRDReviewNormalParts", "取得Normal Parts資訊");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return models;
        }

        public static IList<H_Product_DetailEntity> GetRDReviewNormalpartsBySN(string rsi_no, string sn)
        {
            IList<H_Product_DetailEntity> models = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select d.*, LPAD(' ', (Level) * 4) || d.part_no as DisplayPartNo, Level bom_level from(
                                                        select distinct d.rsi_no,
                                                        nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                                        d.mtl_group,
                                                        d.mtl_parts,
                                                        d.part_type, 
                                                        d.part_no,
                                                        d.part_desc,
                                                        d.part_level,
                                                        d.english_name,
                                                        d.maker_source,
                                                        d.maker_part_no,
                                                        d.part_spec,
                                                        c.SPEC_DEF,
                                                        d.release_date,
                                                        d.usage,
                                                        d.part_unit,
                                                        d.eol_status eol_status,
                                                        d.uni_spec_status uni_spec_status,
                                                        d.remark,
                                                        d.modify_type,
                                                        d.sn,
                                                        d.parent_sn,
                                                        decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                                        d.phase_id,
                                                        i.ref_product partnumber_top,
                                                        d.part_no partnumber_child, 
                                                        d.partnumber_parent
                                                        from gpo.rsi_h_product_detail d
                                                        join gpo.rsi_h_product_info i on d.rsi_no = i.rsi_no
                                                        left join gpo.rsi_c_mtl_special_parts s on s.mtl_parts = d.mtl_parts and s.active = 'Y'
                                                        left join (select distinct mtl_parts, spec_def
                                                                     from gpo.rsi_c_mtl_spec_def
                                                                    where active = 'Y') c
                                                          on d.mtl_parts = c.mtl_parts
                                                        LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                          ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                                        where nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) <> 'Special' 
                                                        and (d.modify_type is null or d.modify_type <> 'D')
                                                        and (d.modify_type is null or d.modify_type <> 'S')
                                                        and d.rsi_no = :rsi_no)d
                                                         start with sn = :sn
                                                    connect by nocycle prior sn = parent_sn
                                                    --connect by nocycle prior partnumber_child = partnumber_parent
                                                    order siblings by part_no desc ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Varchar2, sn, ParameterDirection.Input));
                models = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetRDReviewNormalpartsBySN", "取得Normal Parts資訊");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return models;
        }

        public static IList<H_Product_DetailEntity> GetRDReviewSpecialParts(string rsi_no)
        {
            IList<H_Product_DetailEntity> model = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select *
                                    from (
                                          --Special 組合
                                          select distinct d.rsi_no,
                                                           nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                                           d.mtl_group,
                                                           d.mtl_parts,
                                                           d.part_type part_type,
                                                           d.part_no as part_no,
                                                           nvl(d.part_no,substr(d.part_level,1,2)) as displaypartno,
                                                           d.partnumber_parent,
                                                           d.part_desc,
                                                           d.part_level,
                                                           d.english_name,
                                                           d.maker_source,
                                                           d.maker_part_no,
                                                           d.part_spec,
                                                           c.SPEC_DEF,
                                                           d.release_date,
                                                           d.usage,
                                                           d.part_unit,
                                                           d.eol_status,
                                                           d.uni_spec_status,
                                                           d.remark,
                                                           d.modify_type,
                                                           d.sn,
                                                           decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                                           nvl(d.group_id, to_char(systimestamp, 'yyyymmddhh24missff6')) as group_id,
                                                           d.GROUP_DESC,
                                                           d.GROUP_NAME,
                                                           d.phase_id
                                            FROM RSI_H_PRODUCT_DETAIL d
                                            LEFT JOIN RSI_C_MTL_SPECIAL_PARTS s
                                              ON s.mtl_parts = d.mtl_parts
                                            LEFT JOIN (SELECT DISTINCT MTL_PARTS, SPEC_DEF
                                                         from RSI_C_MTL_SPEC_DEF
                                                        WHERE ACTIVE = 'Y') c
                                              ON d.mtl_parts = c.mtl_parts
                                            LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                              ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                           WHERE d.rsi_no = :rsi_no
                                             AND (d.MODIFY_TYPE IS NULL OR d.MODIFY_TYPE <> 'D')
                                             AND nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) = 'Special'
                                             and nvl(d.mtl_group,'-') not like 'MOH%'
                                     ) a
                                     order by group_id asc, nvl(partnumber_parent, '-') desc, mtl_group, mtl_parts
                ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));

                model = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetRDReviewSpecialParts", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }

        public static bool RDReviewSpecialModefyTypeD(string rsi_no, string group_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                //sqlText.Append(@"update gpo.rsi_h_product_detail d 
                //                                set d.modify_type = 'D'
                //                                where d.rsi_no = :rsi_no
                //                                and d.group_id = :group_id
                //                                and d.mtl_type = 'Special'
                //                                --and d.modify_type = 'A' ");

                sqlText.Append(@"delete from gpo.rsi_h_product_detail d
                                  where d.rsi_no = :rsi_no
                                    and d.group_id = :group_id
                                    and d.mtl_type = 'Special'");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RDReviewSpecialModefyTypeD", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool RDReviewNormalModifyTypeSToEmpty(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update gpo.rsi_h_product_detail d 
                                                set d.modify_type = ''
                                                where d.rsi_no = :rsi_no
                                                and d.mtl_type = 'Normal'
                                                and d.modify_type = 'S' ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));

                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RDReviewNormalModifyTypeSToEmpty", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool RDReviewNormalModifyTypeSToEmptyWithSpecialDelete(string rsi_no, string part_no, string parent_part_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update gpo.rsi_h_product_detail d 
                                    set d.modify_type = ''
                                  where d.rsi_no = :rsi_no
                                    and part_no = :part_no
                                    and partnumber_parent = :parent_part_no
                                    and d.mtl_type = 'Normal'
                                    and d.modify_type = 'S'
                                    and not exists (    --若其他產品組合有該料號也不能更新
                                                   select distinct d1.part_no
                                                     from gpo.rsi_h_product_detail d1
                                                    where d1.rsi_no = d.rsi_no
                                                      and d1.mtl_type = 'Special'
                                                      and d1.modify_type <> 'D'
                                                   )
                                ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_no", OracleDbType.Varchar2, part_no, ParameterDirection.Input));
                param.Add(new OracleParameter("parent_part_no", OracleDbType.Varchar2, parent_part_no, ParameterDirection.Input));

                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RDReviewNormalModifyTypeSToEmptyWithSpecialDelete", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool RDReviewNormalModifyTypeEmptyToS(string rsi_no, string group_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update gpo.rsi_h_product_detail d
                                                set d.modify_type = 'S'
                                                where d.rsi_no = :rsi_no
                                                and d.mtl_type = 'Normal'
                                                and nvl(d.modify_type, '-') <> 'D'
                                                and (nvl(part_no, part_spec), partnumber_parent) in
                                                    (select nvl(part_no, part_spec) as part_no, partnumber_parent
                                                     from gpo.rsi_h_product_detail t
                                                     where t.rsi_no = d.rsi_no
                                                     and t.group_id = :group_id
                                                     and mtl_type = 'Special'
                                                     and modify_type <> 'D') ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));

                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RDReviewNormalModifyTypeEmptyToS", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool RDReviewChangeCallPackage(string partno, string partlevel, string plant)
        {
            try
            {
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("p_lm_user", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("p_partno", OracleDbType.Varchar2, partno, ParameterDirection.Input));
                param.Add(new OracleParameter("p_partno_mfg", OracleDbType.Varchar2, plant, ParameterDirection.Input));
                param.Add(new OracleParameter("p_partlevel", OracleDbType.Varchar2, partlevel, ParameterDirection.Input));
                return SqlExcute.ExecuteStoredProcedure("gpo.rsi_main_pkg.replace_ref_bom_data", param.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<H_Product_DetailEntity> GetRDReviewChangeOtherTableData(string rsi_no, string partno)
        {
            IList<H_Product_DetailEntity> models = new List<H_Product_DetailEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();

                //sqlText.Append(@" select :rsi_no rsi_no, 'Normal' mtl_type, '' mtl_group, '' mtl_parts, '' part_type, b.partnumber_child as part_no, b.part_desc,
                //                         b.part_level, b.english_name, '' maker_source, '' maker_part_no, b.release_date, b.part_quantity as usage,
                //                         b.part_unit, '' eol_status, '' uni_spec_status, 'A' modify_type, 
                //                         b.partnumber_top, b.partnumber_child, b.partnumber_parent, '0' bom_isleaf, b.lm_user, b.partnumber_child as DisplayPartNo, 0 bom_level
                //                    from gpo.rsi_h_bom_all_tmp b
                //                   where lm_user = :emp_no
                //                     and bom_level = 0

                sqlText.Append(@"   select :rsi_no rsi_no, 'Normal' mtl_type, '' mtl_group, '' parts_group, '' mtl_parts, '' part_type, b.part_no, b.part_description as part_desc,
                                         b.part_kind_key as part_level, b.english_name, '' maker_source, '' maker_part_no, b.release_date, 1 as usage,
                                         b.unit as part_unit, '' eol_status, '' uni_spec_status, 'A' modify_type, 
                                         b.part_no as partnumber_top, b.part_no as partnumber_child, b.part_no as partnumber_parent, '0' bom_isleaf, :emp_no as lm_user, b.part_no as DisplayPartNo, 0 bom_level
                                    from rsi_r_part_info b
                                   where b.part_no = :partno
                                    union all
                                        select * 
                                        from(
                                            select d.*, LPAD(' ', (Level-1) * 4) || d.part_no as DisplayPartNo, Level bom_level
                                                from (
                                                    select distinct :rsi_no rsi_no,
                                                    'Normal' mtl_type,
                                                    d.mtl_group,
                                                    d.parts_group,
                                                    d.mtl_parts,
                                                    b.part_type, 
                                                    d.partnumber_child as part_no,
                                                    d.part_desc,
                                                    d.part_level,
                                                    d.english_name,
                                                    '' maker_source,
                                                    d.maker_part_no,
                                                    d.release_date,
                                                    d.part_quantity as usage,
                                                    d.part_unit,
                                                    nvl(p.phase_out_flag, 'N') eol_status,
                                                    nvl(u.active, 'N') uni_spec_status,
                                                    'A' modify_type,
                                                    :partno partnumber_top,
                                                    d.partnumber_child, 
                                                    d.partnumber_parent,
                                                    d.bom_isleaf,
                                                    d.lm_user
                                                    from gpo.rsi_h_bom_all_tmp d
                                                    left join gpo.rsi_c_mtl_special_parts s on s.mtl_parts = d.mtl_parts and s.active = 'Y'
                                                    left join gpo.rsi_r_mtl_phase_out p
                                                      on d.partnumber_child = p.item_no
                                                    left join (select * from gpo.rsi_c_unique_spec where active='Y') u
                                                      on d.partnumber_child = u.part_no     
                                                    left join (
                                                        select distinct d.mtl_group, d.mtl_parts, d.part_type 
                                                          from gpo.rsi_h_product_info i
                                                          join gpo.c_pms_bgbu_mapping m on m.bu = i.bu
                                                          join (select * from rsi_c_parts_type_def where active='Y') d on d.bg = m.bg 
                                                         where rsi_no = :rsi_no
                                                        ) b on d.mtl_group = b.mtl_group and d.mtl_parts = b.mtl_parts
                                                    where d.lm_user = :emp_no
                                                    ) d
                                                where (d.bom_isleaf = 0
                                                   or (d.bom_isleaf = 1 and mtl_group is not null))
                                                start with d.partnumber_top = d.partnumber_parent
                                                connect by nocycle prior d.partnumber_top = d.partnumber_top
                                                               and prior d.partnumber_child = d.partnumber_parent
                                                order siblings by d.part_no desc 
                                        ) a ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("partno", OracleDbType.Varchar2, partno, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                models = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetRDReviewChangeOtherTableData", "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return models;
        }

        public static bool UpdateSpecialPartsName(string rsi_no, string group_id, string group_name)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE GPO.RSI_H_PRODUCT_DETAIL SET 
                                                GROUP_NAME = :group_name
                                                WHERE RSI_NO = :rsi_no AND GROUP_ID = :group_id ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("group_name", OracleDbType.Varchar2, group_name, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateSpecialPartsName", String.Empty);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool UpdateSpecialPartsDesc(string rsi_no, string group_id, string group_desc)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"UPDATE GPO.RSI_H_PRODUCT_DETAIL SET 
                                                GROUP_DESC = :group_desc
                                                WHERE RSI_NO = :rsi_no AND GROUP_ID = :group_id ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("group_desc", OracleDbType.Varchar2, group_desc, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                return SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateSpecialPartsDesc", String.Empty);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static DataTable GetMaterialGroupMaterialParts(string rsi_no, string part_level, string english_name, string parent_part_level, string sn)
        {
            DataTable dt = new DataTable();
            try
            {
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("p_rsino", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_partlevel", OracleDbType.Varchar2, part_level, ParameterDirection.Input));
                param.Add(new OracleParameter("p_englishname", OracleDbType.Varchar2, english_name, ParameterDirection.Input));
                param.Add(new OracleParameter("p_parentlevel", OracleDbType.Varchar2, parent_part_level, ParameterDirection.Input));
                param.Add(new OracleParameter("p_sn", OracleDbType.Varchar2, sn, ParameterDirection.Input));
                param.Add(new OracleParameter("out_rs", OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output));
                dt = SqlExcute.ExecuteStoredProcedureGetDataTable("rsi_main_pkg.query_mtl_group", param.ToArray());
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }
        }

        public static String GetUniSpecStatus(string part_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select nvl(active, 'Y') as uni_spec_status from RSI_C_UNIQUE_SPEC where active = 'Y' and part_no = :part_no ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("part_no", OracleDbType.Varchar2, part_no, ParameterDirection.Input));
                var result = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetUniSpecStatus", String.Empty);
                return result.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        public static String GetEolStatus(string part_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                //sqlText.Append(@"select nvl(phase_out_flag, 'Y') as eol_status from AU_DW.VSPO_DW_PHASE_OUT_MAPPING where item_no = :part_no ");
                sqlText.Append(@"select nvl(phase_out_flag, 'Y') as eol_status from GPO.RSI_R_MTL_PHASE_OUT where item_no = :part_no ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("part_no", OracleDbType.Varchar2, part_no, ParameterDirection.Input));
                var result = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetEolStatus", String.Empty);
                return result.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        public static void UpdateParentPartNo(string rsi_no, List<string> resultSN, string parent_no)
        {
            resultSN.Remove("0");
            try
            {
                #region Step1 update bom_all最Top階資料
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update gpo.rsi_h_bom_all 
                                                          set ismodify = 'Y'
                                                          where rsi_no = :rsi_no
                                                           and bom_level = 0 ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateParentPartNo", String.Empty);
                #endregion

                #region Step2 update parent no資料
                sqlText = new StringBuilder();
                sqlText.AppendFormat(@"update rsi_h_product_detail
                                                          set partnumber_parent = substr(partnumber_parent, 0, 2)
                                                          where rsi_no = :rsi_no
                                                          and partnumber_parent in
                                                          (select distinct partnumber_parent
                                                             from GPO.RSI_H_PRODUCT_DETAIL t
                                                            where rsi_no = :rsi_no
                                                              and sn in ('{0}')
                                                              and partnumber_parent <> :parent_no) ", String.Join("','", resultSN));
                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("parent_no", OracleDbType.Varchar2, parent_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateParentPartNo", String.Empty);
                #endregion

                #region Step3 update part no料號
                sqlText = new StringBuilder();
                sqlText.AppendFormat(@"update rsi_h_product_detail
                                          set part_no = substr(part_no, 0, 2),
                                              modify_type = 'U',
                                              eol_status = null,
                                              uni_spec_status = null
                                          where rsi_no = :rsi_no
                                            and nvl(modify_type, '-') <> 'D'
                                            and sn in ('{0}') ", String.Join("','", resultSN));
                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateParentPartNo", String.Empty);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteMOH(string rsi_no, string mtl_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"delete from rsi_h_product_detail where rsi_no = :rsi_no and mtl_group like 'MOH%' and mtl_type = :mtl_type ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("mtl_type", OracleDbType.Varchar2, mtl_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "DeleteMOH", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InsertMOHForNormal(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"insert into rsi_h_product_detail
                                              (rsi_no, mtl_group, mtl_parts, part_type, created_date, created_by, mtl_type, bu, parts_group, usage, sn)   
                                               select a.*, RSI_H_PROD_DETAIL_sn_seq.NEXTVAL from
                                               (select i.rsi_no, a.mtl_group, a.mtl_parts, b.part_type, sysdate, 'SYSTEM', 'Normal', i.bu, a.parts_group, 1 as usage
                                               from rsi_h_product_info i
                                               join gpo.c_pms_bgbu_mapping m on m.bu = i.bu
                                               join gpo.rsi_c_mtl_parts a on a.active = 'Y'
                                               join gpo.rsi_c_parts_type_def b on b.bg = m.bg and a.mtl_group = b.mtl_group and a.mtl_parts = b.mtl_parts and b.active = 'Y'
                                               where a.mtl_group like 'MOH%'
                                               and decode(b.bu, '', 1, instr(i.bu, b.bu)) > 0
                                               and i.rsi_no = :rsi_no
                                               and not exists (select distinct d.mtl_group, d.mtl_parts
                                                                       from gpo.rsi_h_product_detail d
                                                                       where d.rsi_no = :rsi_no
                                                                       and d.mtl_type = 'Normal'
                                                                       and d.mtl_group = a.mtl_group
                                                                       and d.mtl_parts = a.mtl_parts)
                                               order by i.rsi_no, a.mtl_group, a.mtl_parts) a ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "InsertMOHForNormal", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InsertMOHForSpecial(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"insert into rsi_h_product_detail
                                              (rsi_no, mtl_group, mtl_parts, part_type, created_date, created_by, mtl_type, bu, parts_group, usage, group_id, group_desc, group_name, sn)     
                                               select a.*, 'A'||RSI_H_PROD_DETAIL_sn_seq.NEXTVAL from
                                               (select i.rsi_no, a.mtl_group, a.mtl_parts, b.part_type, sysdate, 'SYSTEM', 'Special', i.bu, a.parts_group, 1 as usage, c.group_id, c.group_desc, c.group_name
                                               from rsi_h_product_info i
                                               join gpo.c_pms_bgbu_mapping m on m.bu = i.bu
                                               join gpo.rsi_c_mtl_parts a on a.active = 'Y'
                                               join gpo.rsi_c_parts_type_def b on b.bg = m.bg and a.mtl_group = b.mtl_group and a.mtl_parts = b.mtl_parts and b.active = 'Y'
                                               join (select distinct rsi_no, group_id, group_desc, group_name
                                                          from gpo.rsi_h_product_detail 
                                                         where rsi_no = :rsi_no
                                                           and mtl_type = 'Special'
                                                           and (modify_type is null or modify_type <> 'D')) c on i.rsi_no = c.rsi_no
                                               where a.mtl_group like 'MOH%'
                                               and decode(b.bu, '', 1, instr(i.bu, b.bu)) > 0
                                               and i.rsi_no = :rsi_no
                                               and not exists (select distinct d.mtl_group, d.mtl_parts
                                                                       from gpo.rsi_h_product_detail d
                                                                       where d.rsi_no = :rsi_no
                                                                       and d.mtl_type = 'Special'
                                                                       and d.mtl_group = a.mtl_group
                                                                       and d.mtl_parts = a.mtl_parts)
                                               order by i.rsi_no, c.group_id, a.mtl_group, a.mtl_parts) a ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "InsertMOHForSpecial", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateChooseToSpecialStatus(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL a
                                            set a.isapproved = 'Y'
                                            where NVL(a.mtl_type, '-') = 'Normal' AND NVL(a.modify_type, '-') = 'S'
                                            and a.rsi_no = :rsi_no ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateChooseToSpecialStatus", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<H_Product_DetailEntity> GetProductDetailForExportExcelSpecialData(string rsi_no, string group_id, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select b.rsi_no, 'Normal' mtl_type, '' mtl_group, '' parts_group, '' mtl_parts, '' part_type, b.partnumber_child as part_no, b.part_desc,
                                               b.part_level, b.english_name, '' maker_source, '' maker_part_no, '' part_spce, '' spec_def, b.release_date,
                                               b.part_quantity as usage, b.part_unit, '' eol_status, '' uni_spec_status, '' remark, '' modify_type, 0 sn, '' file_status, 
                                               '' phase_id,'' group_id, '' group_name, '' group_desc, 
                                               b.partnumber_top, b.partnumber_child, b.partnumber_parent,
                                               0 PRICE, 0 PRICE_PM, 0 Moq, 0 Mockup, 0 Tooling, 0 FPCA_PCBA, 0 SourcerAmount, 0 PMAmount, '' CURRENCY, 0 RATE, 
                                               '' SOURCE_NO, '' SOURCE, 'N' ISModify, '' REMARK_PUR,  
                                               'N' ISASSIGNER, 'N' ISCALCULATE, b.partnumber_child as DisplayPartNo, 0 bom_level
                                               from gpo.rsi_h_bom_all b
                                               where rsi_no = :rsi_no   ---要放:rsi_no
                                               and bom_level = 0
                                               union all
                                               select *
                                               from (
                                                   select d.*, LPAD(' ', (Level) * 4) || d.part_no as DisplayPartNo, Level bom_level
                                                   from (
                                                       select distinct d.rsi_no,
                                                       nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) mtl_type,
                                                       d.mtl_group,
                                                       d.parts_group,
                                                       d.mtl_parts,
                                                       d.part_type, 
                                                       d.part_no,
                                                       d.part_desc,
                                                       d.part_level,
                                                       d.english_name,
                                                       d.maker_source,
                                                       d.maker_part_no,
                                                       d.part_spec,
                                                       c.SPEC_DEF,
                                                       d.release_date,
                                                       d.usage,
                                                       d.part_unit,
                                                       d.eol_status eol_status,
                                                       d.uni_spec_status uni_spec_status,
                                                       d.remark,
                                                       d.modify_type,
                                                       d.sn,
                                                       decode(nvl(f.cnt,0),0,'N','Y') as file_status,
                                                       d.phase_id,
                                                       d.group_id,
                                                       d.group_name,
                                                       d.group_desc,
                                                       i.ref_product partnumber_top,
                                                       d.part_no partnumber_child, 
                                                       d.partnumber_parent,
                                                       P.PRICE, 
                                                       P.PRICE_PM, 
                                                       P.Moq, 
                                                       P.Mockup, 
                                                       P.Tooling,
                                                       P.FPCA_PCBA,
                                                       --P.MPPRICE3, 
                                                       P.SourcerAmount, 
                                                       P.PMAmount, 
                                                       P.CURRENCY, 
                                                       P.RATE, 
                                                       P.SOURCE_NO, 
                                                       P.SOURCE,
                                                       NVL(P.ISModify, 'N') ISModify,
                                                       P.REMARK_PUR,  
                                                       P.ISASSIGNER, 
                                                       P.ISCALCULATE
                                                       from gpo.rsi_h_product_detail d
                                                       join gpo.rsi_h_product_info i on d.rsi_no = i.rsi_no
                                                       left join gpo.rsi_c_mtl_special_parts s on s.mtl_parts = d.mtl_parts and s.active = 'Y'
                                                       left join (select distinct mtl_parts, spec_def
                                                                    from gpo.rsi_c_mtl_spec_def
                                                                   where active = 'Y') c
                                                         on d.mtl_parts = c.mtl_parts
                                                       LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) f
                                                         ON f.BIZ_ID = d.RSI_NO||'_'||d.SN
                                                       LEFT JOIN (SELECT D.MTL_GROUP,    --一般申請權限
                                                                         D.MTL_PARTS, 
                                                                         D.PART_TYPE, 
                                                                         D.PART_NO, 
                                                                         D.PART_SPEC,
                                                                         D.USAGE, 
                                                                         NVL(D.PRICE, 0) AS PRICE, 
                                                                         NVL(D.PRICE_PM, 0) AS PRICE_PM, 
                                                                         NVL(D.Moq, 0) AS Moq, 
                                                                         NVL(D.Mockup, 0) AS Mockup, 
                                                                         NVL(D.Tooling, 0) AS Tooling,
                                                                         NVL(D.FPCA_PCBA, 0) AS FPCA_PCBA,
                                                                         --NVL(D.MPPRICE3, 0) AS MPPRICE3, 
                                                                         nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                                                         nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,             
                                                                         NVL(D.SOURCE, '') AS SOURCE,  
                                                                         NVL(D.CURRENCY, '') AS CURRENCY,  
                                                                         NVL(D.RATE, 0) AS RATE,
                                                                         NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                                                         D.REMARK_PUR,
                                                                         'Y' ISModify,
                                                                         NVL(D.ISASSIGNER, 'N') AS ISASSIGNER,
                                                                         DECODE(D.ISCALCULATE, 'N', 'N', 'Y') AS ISCALCULATE
                                                                         FROM GPO.RSI_H_PRODUCT_DETAIL D
                                                                         JOIN GPO.RSI_H_AUTHORITY H ON (D.BU = H.BU AND D.MTL_PARTS = H.MTL_PARTS)
                                                                         WHERE D.RSI_NO = :rsi_no  ---要放:rsi_no
                                                                         AND D.MTL_TYPE = 'Normal' 
                                                                         AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')   
                                                                         AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                                         AND H.EMP_NO = :emp_no      ---要放:emp_no
                                                                         AND H.ACTIVE = 'Y'
                                                                         AND H.START_DATE <= SYSDATE
                                                                         AND H.END_DATE >= SYSDATE
                                                                         UNION   --若新增Material Group/Material Parts預設人員
                                                                         SELECT D.MTL_GROUP, 
                                                                         D.MTL_PARTS, 
                                                                         D.PART_TYPE, 
                                                                         D.PART_NO, 
                                                                         D.PART_SPEC,
                                                                         D.USAGE, 
                                                                         NVL(D.PRICE, 0) AS PRICE, 
                                                                         NVL(D.PRICE_PM, 0) AS PRICE_PM, 
                                                                         NVL(D.Moq, 0) AS Moq, 
                                                                         NVL(D.Mockup, 0) AS Mockup, 
                                                                         NVL(D.Tooling, 0) AS Tooling,
                                                                         NVL(D.FPCA_PCBA, 0) AS FPCA_PCBA,
                                                                         --NVL(D.MPPRICE3, 0) AS MPPRICE3, 
                                                                         nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                                                         nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,            
                                                                         NVL(D.SOURCE, '') AS SOURCE,  
                                                                         NVL(D.CURRENCY, '') AS CURRENCY,  
                                                                         NVL(D.RATE, 0) AS RATE,
                                                                         NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                                                         D.REMARK_PUR,
                                                                         'Y' ISModify,
                                                                         NVL(D.ISASSIGNER, 'N') AS ISASSIGNER,
                                                                         DECODE(D.ISCALCULATE, 'N', 'N', 'Y') AS ISCALCULATE
                                                                         FROM GPO.RSI_H_PRODUCT_DETAIL D
                                                                         JOIN GPO.RSI_H_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.MTL_PARTS) AND H.TYPE = 'DEFAULT'
                                                                         WHERE (1 = 1)
                                                                         --AND D.MODIFY_TYPE = 'NEW' 
                                                                         AND D.MTL_PARTS = 'RD DEFINE'
                                                                         AND D.RSI_NO = :rsi_no  ---要放:rsi_no 
                                                                         AND D.MTL_TYPE = 'Normal' 
                                                                         AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D') 
                                                                         AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                                                         AND H.EMP_NO = :emp_no   ---要放:emp_no
                                                                         AND H.ACTIVE = 'Y'
                                                                         AND H.START_DATE <= SYSDATE
                                                                         AND H.END_DATE >= SYSDATE
                                                                         AND D.mtl_group ='RD DEFINE'
                                                                   ) P ON D.MTL_GROUP = P.MTL_GROUP AND D.MTL_PARTS = P.MTL_PARTS AND D.PART_TYPE = P.PART_TYPE AND NVL(D.PART_NO, D.PART_SPEC) = NVL(P.PART_NO, P.PART_SPEC)
                                                       where d.rsi_no = :rsi_no             ---要放:rsi_no
                                                         and (d.modify_type is null or d.modify_type <> 'D')
                                                         and (d.group_id is null or d.group_id = :group_id )       ---要放:group_id
                                                   ) d
                                                   start with partnumber_top = partnumber_parent
                                                   connect by nocycle prior partnumber_top = partnumber_top
                                                                  and prior partnumber_child = partnumber_parent
                                                   order siblings by part_level desc
                                               ) a ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("group_id", OracleDbType.Varchar2, group_id, ParameterDirection.Input));
                return SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "GetProductDetailForExportExcelSpecialData", String.Empty);
            }
            catch (Exception ex)
            {
                return new List<H_Product_DetailEntity>();
            }
        }

        public static IList<H_Product_DetailEntity> ProductReview_GetNormalParts(string rsi_no, string part_type, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT D.RSI_NO, D.MTL_TYPE, D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, D.PART_TYPE, NVL(D.PART_NO,D.PART_LEVEL) as PART_NO, D.PART_DESC, D.PART_LEVEL, D.ENGLISH_NAME, D.MAKER_SOURCE, D.MAKER_PART_NO, D.PART_SPEC, D.RELEASE_DATE, D.USAGE, D.EOL_STATUS, D.UNI_SPEC_STATUS, D.REMARK, 
                                P.PRICE, P.PRICE_PM, P.MOQ, P.MOCKUP, P.TOOLING, P.SourcerAmount, P.PMAmount, P.CURRENCY, P.RATE, P.SOURCE_NO, P.SOURCE, D.MODIFY_TYPE, D.BU, NVL(P.ISModify, 'N') ISModify, D.PHASE_ID, P.REMARK_PUR, D.GROUP_DESC,  D.SN, D.PARENT_SN, decode(nvl(f.cnt,0),0,'N','Y') as FILE_STATUS,
                                D.UPDATED_DATE, D.UPDATED_BY, E.EMP_NAME, 
                                decode(substr(D.PARTS_GROUP,1,3),'MOH','Y',P.ISASSIGNER) ISASSIGNER,
                                P.ISCALCULATE, D.REMARK_PM, D.PARTNUMBER_PARENT, D.FPCA_PCBA,
                                nvl2(a.emp_no,a.emp_no||'/'||a.emp_name,null) as reassign_from, nvl2(b.emp_no,b.emp_no||'/'||b.emp_name,null) as reassign_to, au.created_date as reassign_date, 
                                D.ISAPPROVED,
                                nvl(d.sourcer_owner, s.owner) as sourcer_owner_id,
                                nvl(e1.eng_name, s.owner_name) as sourcer_owner
                                FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                LEFT JOIN (
                                --若Product Sourcer權限人員
                                SELECT DISTINCT D.MTL_GROUP, 
                                D.PARTS_GROUP,
                                D.MTL_PARTS, 
                                D.PART_TYPE, 
                                D.PART_NO, 
                                D.PART_SPEC,
                                D.USAGE, 
                                D.PRICE, 
                                D.PRICE_PM, 
                                D.MOQ, 
                                D.MOCKUP, 
                                D.TOOLING,
                                D.FPCA_PCBA,
                                nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,          
                                NVL(D.SOURCE, '') AS SOURCE,  
                                NVL(D.CURRENCY, '') AS CURRENCY,  
                                NVL(D.RATE, 0) AS RATE,
                                NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                D.REMARK_PUR,
                                'Y' ISModify,
                                NVL(D.ISASSIGNER, 'N') AS ISASSIGNER,
                                NVL(D.ISCALCULATE, 'N') AS ISCALCULATE, D.SN
                                FROM GPO.RSI_H_PRODUCT_DETAIL D
                                JOIN GPO.RSI_H_PS_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE)
                                WHERE (1 = 1)
                                AND D.RSI_NO = :rsi_no
                                AND D.MTL_TYPE = 'Normal' 
                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D') 
                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                AND NVL(D.MTL_PARTS,'-') <> '-'  --節點不需要認領
                                AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                AND H.EMP_NO = :emp_no  --'1108633'
                                AND H.ACTIVE = 'Y'
                                AND H.START_DATE <= SYSDATE
                                AND H.END_DATE >= SYSDATE
                                AND D.PART_TYPE = :part_type
                                ) P ON D.SN = P.SN
                                LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) F
                                ON F.BIZ_ID = D.RSI_NO||'_'||D.SN
                                LEFT JOIN GFIN.EMP_DATA_ALL E ON E.EMP_NO = D.UPDATED_BY
                                LEFT JOIN GFIN.EMP_DATA_ALL E1
                                ON E1.EMP_NO = D.Sourcer_Owner
                                LEFT JOIN (select * from GPO.RSI_H_AUTHORITY
                                            where type='ASSIGN' and active='Y' and START_DATE <= SYSDATE AND END_DATE >= SYSDATE) AU 
                                on AU.Rsi_No = D.Rsi_No and au.sn = d.sn
                                left join gfin.emp_data_all a on a.emp_no = AU.created_by
                                left join gfin.emp_data_all b on b.emp_no = AU.emp_no
                                left join gpo.rsi_item_owner_v s
                                on s.rsi_no = d.rsi_no
                                and s.sn = d.sn
                                WHERE D.RSI_NO = :rsi_no
                                AND D.MTL_TYPE = 'Normal' 
                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                ORDER BY DECODE(ISModify, 'Y', 0, 1), PART_TYPE, NVL(ISAPPROVED,'-'), DECODE(substr(MTL_GROUP,1,3), 'MOH', 99, 0), D.MTL_PARTS, NVL(D.PART_NO, D.PART_LEVEL), SN ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                return SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "ProductReview_GetNormalParts", String.Empty);
            }
            catch (Exception ex)
            {
                return new List<H_Product_DetailEntity>();
            }
        }

        public static IList<H_Product_DetailEntity> ProductReview_GetSpecialParts(string rsi_no, string part_type, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT D.RSI_NO, D.Group_ID, D.MTL_TYPE, D.MTL_GROUP, D.PARTS_GROUP, D.MTL_PARTS, D.PART_TYPE, NVL(D.PART_NO,D.PART_LEVEL) as PART_NO, D.PART_DESC, D.PART_LEVEL, D.ENGLISH_NAME, D.MAKER_SOURCE, D.MAKER_PART_NO, D.PART_SPEC, D.RELEASE_DATE, D.USAGE, D.EOL_STATUS, D.UNI_SPEC_STATUS, D.REMARK, 
                                    P.PRICE, P.PRICE_PM, P.MOQ, P.MOCKUP, P.TOOLING, P.SourcerAmount, P.PMAmount, P.CURRENCY, P.RATE, P.SOURCE_NO, P.SOURCE, D.MODIFY_TYPE, D.BU, NVL(P.ISModify, 'N') ISModify, D.PHASE_ID, P.REMARK_PUR, D.GROUP_DESC,  D.SN, D.PARENT_SN, D.GROUP_NAME, decode(nvl(f.cnt,0),0,'N','Y') as FILE_STATUS,
                                    D.UPDATED_DATE, D.UPDATED_BY, E.EMP_NAME, 
                                    decode(substr(D.PARTS_GROUP,1,3),'MOH','Y',P.ISASSIGNER) ISASSIGNER,
                                    P.ISCALCULATE, D.REMARK_PM, D.PARTNUMBER_PARENT, D.FPCA_PCBA,
                                    nvl2(a.emp_no,a.emp_no||'/'||a.emp_name,null) as reassign_from, nvl2(b.emp_no,b.emp_no||'/'||b.emp_name,null) as reassign_to, au.created_date as reassign_date, 
                                    D.ISAPPROVED,
                                    nvl(d.sourcer_owner, s.owner) as sourcer_owner_id,
                                    nvl(e1.eng_name, s.owner_name) as sourcer_owner
                                    FROM GPO.RSI_H_PRODUCT_DETAIL D 
                                    LEFT JOIN (
                                    --若Product Sourcer權限人員
                                    SELECT DISTINCT D.MTL_GROUP, 
                                    D.PARTS_GROUP,
                                    D.MTL_PARTS, 
                                    D.PART_TYPE, 
                                    D.PART_NO, 
                                    D.PART_SPEC,
                                    D.USAGE, 
                                    D.PRICE, 
                                    D.PRICE_PM, 
                                    D.MOQ, 
                                    D.MOCKUP, 
                                    D.TOOLING,
                                    D.FPCA_PCBA,
                                    nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                                    nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,          
                                    NVL(D.SOURCE, '') AS SOURCE,  
                                    NVL(D.CURRENCY, '') AS CURRENCY,  
                                    NVL(D.RATE, 0) AS RATE,
                                    NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                                    D.REMARK_PUR,
                                    'Y' ISModify,
                                    NVL(D.ISASSIGNER, 'N') AS ISASSIGNER,
                                    NVL(D.ISCALCULATE, 'N') AS ISCALCULATE, D.SN
                                    FROM GPO.RSI_H_PRODUCT_DETAIL D
                                    JOIN GPO.RSI_H_PS_AUTHORITY H ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE)
                                    WHERE (1 = 1)
                                    AND D.RSI_NO = :rsi_no
                                    AND D.MTL_TYPE = 'Special' 
                                    AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D') 
                                    AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                    AND NVL(D.MTL_PARTS,'-') <> '-'  --節點不需要認領
                                    AND exists (select 1 from rsi_h_product_detail t where (t.sn = d.parent_sn and nvl(t.modify_type, '-') <> 'D')
                                                 OR d.parent_sn = 0 OR d.parent_sn IS NULL)
                                    AND H.EMP_NO = :emp_no
                                    AND H.ACTIVE = 'Y'
                                    AND H.START_DATE <= SYSDATE
                                    AND H.END_DATE >= SYSDATE
                                    AND D.PART_TYPE = :part_type
                                    ) P ON D.SN = P.SN
                                    LEFT JOIN (SELECT Biz_Id,count(1) AS cnt FROM GPO.RSI_H_FILELIST GROUP BY Biz_Id) F
                                    ON F.BIZ_ID = D.RSI_NO||'_'||D.SN
                                    LEFT JOIN GFIN.EMP_DATA_ALL E ON E.EMP_NO = D.UPDATED_BY
                                    LEFT JOIN GFIN.EMP_DATA_ALL E1
                                    ON E1.EMP_NO = D.Sourcer_Owner
                                    LEFT JOIN (select * from GPO.RSI_H_AUTHORITY
                                               where type='ASSIGN' and active='Y' and START_DATE <= SYSDATE AND END_DATE >= SYSDATE) AU 
                                    on AU.Rsi_No = D.Rsi_No and au.sn = d.sn
                                    left join gfin.emp_data_all a on a.emp_no = AU.created_by
                                    left join gfin.emp_data_all b on b.emp_no = AU.emp_no
                                    left join gpo.rsi_item_owner_v s
                                    on s.rsi_no = d.rsi_no
                                    and s.sn = d.sn
                                    WHERE D.RSI_NO = :rsi_no
                                    AND D.MTL_TYPE = 'Special' 
                                    AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                                    AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                                    ORDER BY RSI_NO, GROUP_ID, DECODE(ISModify, 'Y', 0, 1), NVL(ISAPPROVED,'-'), DECODE(substr(MTL_GROUP,1,3), 'MOH', 99, 0), MTL_PARTS, NVL(D.PART_NO, D.PART_LEVEL) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                return SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "ProductReview_GetSpecialParts", String.Empty);
            }
            catch (Exception ex)
            {
                return new List<H_Product_DetailEntity>();
            }
        }

        public static IList<H_Product_PriceTrend> GetPriceTrend(Decimal rsi_no, Decimal sn)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select * from RSI_H_Price_Trend where rsi_no = :rsi_no and sn = :sn  order by sn");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Decimal, sn, ParameterDirection.Input));
                return SqlExcute.GetOraObjList<H_Product_PriceTrend>(sqlText, param.ToArray(), "GetPriceTrend", String.Empty);

            }
            catch (Exception ex)
            {
                return new List<H_Product_PriceTrend>();
            }
        }

        public static void UpdatePriceTrend(string rsi_no, string sn, string id, string price)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"DELETE FROM GPO.RSI_H_PRICE_TREND where rsi_no = :rsi_no and sn = :sn and id = :id ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Decimal, sn, ParameterDirection.Input));
                param.Add(new OracleParameter("id", OracleDbType.Decimal, id, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "SetPriceTrend", String.Empty);

                sqlText.Clear();
                param.Clear();

                if (!string.IsNullOrEmpty(price))
                {
                    sqlText.Append(@"INSERT INTO GPO.RSI_H_PRICE_TREND(RSI_NO, SN, ID, PRICE, UPDATED_DATE, UPDATED_BY) VALUES (:rsi_no, :sn, :id, :price, SYSDATE, :emp_no) ");
                    param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("sn", OracleDbType.Decimal, sn, ParameterDirection.Input));
                    param.Add(new OracleParameter("id", OracleDbType.Decimal, id, ParameterDirection.Input));
                    param.Add(new OracleParameter("price", OracleDbType.Decimal, price, ParameterDirection.Input));
                    param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                    SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "SetPriceTrend", String.Empty);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateParentSNForChange(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL a
                                                set a.parent_sn = (select sn from RSI_H_PRODUCT_DETAIL b where b.rsi_no = a.rsi_no and nvl(b.part_no, b.part_level) = a.partnumber_parent and b.modify_type not in ('D','S'))
                                                where a.rsi_no= :rsi_no
                                                and a.modify_type not in ('D','S')
                                                and a.parent_sn is null
                                                and exists ( select 1 from RSI_H_PRODUCT_DETAIL b where b.rsi_no = a.rsi_no and nvl(b.part_no, b.part_level) = a.partnumber_parent) ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateParentSNForChange", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void UpdateParentSNbyBOM(string rsi_no, string old_sn, string new_sn)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL a
                                            set a.parent_sn = :new_sn
                                            where a.rsi_no = :rsi_no 
                                            and a.parent_sn = :old_sn ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("new_sn", OracleDbType.Decimal, new_sn, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("old_sn", OracleDbType.Decimal, old_sn, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateParentSNbyBOM", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetProductDetailRSINOSN(string rsi_no, string part_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select sn from RSI_H_PRODUCT_DETAIL
                                                where rsi_no = :rsi_no
                                                and part_no = :part_no
                                                and (modify_type is null or modify_type <> 'D') ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_no", OracleDbType.Varchar2, part_no, ParameterDirection.Input));
                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetProductDetailRSINOSN", String.Empty);
                string result = String.Empty;
                if (dt.Rows.Count > 0)
                    result = dt.Rows[0][0].ToString();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal GetSNByProductDetail()
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select RSI_H_PROD_DETAIL_sn_seq.nextval from dual ");
                DataTable dt = SqlExcute.GetOraDateTable(sqlText, "GetProductDetailRSINOSN", String.Empty);
                string result = String.Empty;
                if (dt.Rows.Count > 0)
                    result = dt.Rows[0][0].ToString();
                return Convert.ToDecimal(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ModalEdit_Save(H_Product_DetailEntity model)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL a
                                                set a.MTL_GROUP = :mtl_group,
                                                     a.PARTS_GROUP = :parts_group,
                                                     a.MTL_PARTS = :mtl_parts  
                                                where a.SN = :sn");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("mtl_group", OracleDbType.NVarchar2, model.MTL_GROUP, ParameterDirection.Input));
                param.Add(new OracleParameter("parts_group", OracleDbType.NVarchar2, model.PARTS_GROUP, ParameterDirection.Input));
                param.Add(new OracleParameter("mtl_parts", OracleDbType.NVarchar2, model.MTL_PARTS, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Decimal, model.SN, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ModelEdit_Save", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InsertLogForReturnSubmit(string rsi_no, string part_type, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"insert into RSI_H_DETAIL_APP_LOG(rsi_no, sn, form_no, phase_id, app_serial, app_actor, app_status, app_content, created_by, created_date, begin_date, end_date)
                                    select fh.rsi_no, null, fh.form_no, '10', fa.app_serial,:emp_no, 'A', 'RD Resend to Sourcer',:emp_no, sysdate, min(fa.begin_date), sysdate
                                    from RSI_H_FORM_HEADER fh
                                    join RSI_H_FORM_APPROVE fa
                                    on fa.form_no = fh.form_no
                                    where fh.rsi_no = :rsi_no and fh.part_type= :part_type  --從querystring的rsi_no、part_type取得
                                    and fa.phase_id = '40'  --固定值
                                    group by fh.rsi_no,fh.form_no,fa.app_serial ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "InsertLogForReturnSubmit");
            }
            catch (Exception ex)
            {

            }
        }

        public static DataTable GetSheet1Data(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select b.partnumber_parent as parent_level,
                                        '' part_type,
                                        decode(ismodify, 'Y', substr(b.partnumber_child, 0, 2), b.partnumber_child) as part_no,
                                        b.part_level,
                                        b.english_name,
                                        b.part_desc as part_spec,
                                        b.part_quantity as usage,
                                        b.part_unit,
                                        '' remark,
                                        null as created_by,
                                        null as created_date,
                                        'N' as ismodify
                                 from gpo.rsi_h_bom_all b
                                 where rsi_no = :rsi_no
                                 and bom_level = 0
                                 union all
                                 select partnumber_parent as parent_level,
                                        part_type,
                                        nvl(part_no_ori, part_no) as part_no,
                                        part_level,
                                        english_name,
                                        part_spec,
                                        usage,
                                        unit,
                                        remark,
                                        created_by,
                                        created_date,
                                        decode(part_type,:part_type,'Y','N') as ismodify
                                 from (select d.*,
                                              LPAD(' ', (Level) * 4) || nvl(d.part_no, d.part_level) as DisplayPartNo,
                                              Level bom_level, CONNECT_BY_ISLEAF as isleaf
                                       from (select distinct i.ref_product partnumber_top,
                                                             nvl(r.part_no_ori, d.partnumber_parent) as partnumber_parent,
                                                             d.parent_sn,
                                                             d.partnumber_parent as parent_level,
                                                             d.part_type,
                                                             d.part_no,
                                                             d.part_no_ori,
                                                             d.sn,
                                                             d.part_level,
                                                             d.english_name,
                                                             d.part_spec,
                                                             d.usage,
                                                             d.part_unit as unit,
                                                             d.remark,
                                                             nvl(e.eng_name,d.created_by) as created_by,
                                                             d.created_date
                                            from gpo.rsi_h_product_detail d
                                            join gpo.rsi_h_product_info i on d.rsi_no = i.rsi_no
                                            left join gpo.rsi_h_product_detail r
                                            on r.sn = d.parent_sn
                                            and nvl(r.mtl_type,'-') <> 'Special' 
                                            and nvl(r.modify_type, '-') <> 'D' 
                                            left join gfin.emp_data_all e on d.created_by = e.emp_no
                                            --left join gpo.rsi_c_mtl_special_parts s on s.mtl_parts = d.mtl_parts and s.active = 'Y'
                                            left join (select distinct mtl_parts, spec_def
                                                       from gpo.rsi_c_mtl_spec_def
                                                       where active = 'Y') c on d.mtl_parts = c.mtl_parts
                                            --left join gpo.rsi_h_filelist f on f.biz_id = d.rsi_no || '_' || d.sn
                                            --where nvl(d.mtl_type, decode(s.active, 'Y', 'Special', 'Normal')) <> 'Special'
                                            where nvl(d.mtl_type,'-') <> 'Special'
                                            and (d.modify_type is null or d.modify_type <> 'D')
                                            and nvl(d.mtl_group, '-') not like 'MOH%'
                                            and d.rsi_no = :rsi_no) d
                                       start with partnumber_top = partnumber_parent
                                       connect by nocycle prior partnumber_top = partnumber_top
                                       and prior sn = parent_sn
                                       order siblings by part_level desc) a ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "H_Product_DetailEntityDAL.GetSheet1Data", "RD Download Excel Sheet1 Data");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetSheet2Data(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select b.parent_level,
                                        b.part_type,
                                        null           as part_no,
                                        b.part_level   as part_level,
                                        b.english_name,
                                        b.default_spec as part_spec,
                                        null           as usage,
                                        null           as unit,
                                        null           as remark
                                 from rsi_h_product_info i
                                 join gpo.rsi_c_tmp_rule a on a.bu = i.bu
                                 join gpo.rsi_h_tmp_def b on a.id = b.id
                                 where a.active = 'Y'
                                 and a.bu = i.bu
                                 and a.code = nvl(i.product_combination_name, 'Default')
                                 and i.rsi_no = :rsi_no  
                                 and b.part_type = :part_type ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "H_Product_DetailEntityDAL.GetSheet2Data", "RD Download Excel Sheet2 Data");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetSheet3Data()
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct part_kind_key as part_level, english_name from RSI_R_english_name where part_type in ('ACD','EE','OM','Product')  order by 1,2 ");
                DataTable result = SqlExcute.GetOraDateTable(sqlText, "GetSheet3Data", "");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteDetailForUpload(string rsi_no, string part_type, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL 
                                 set modify_type= 'D',UPDATED_DATE = SYSDATE,UPDATED_BY = :emp_no
                                 where rsi_no = :rsi_no and part_type= :part_type ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Decimal, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "Delete Detail Data with RD Upload BOM Data");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteDetailTempForUpload(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"delete from RSI_H_RD_UPLOAD_TMP
                                 where rsi_no = :rsi_no
                                 and part_type = :part_type ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "DeleteDetailTempForUpload", "Delete Detail Temp Data with RD Upload BOM Data");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InsertDetailTempForUpload(string rsi_no, H_Product_DetailTree data, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"insert into RSI_H_RD_UPLOAD_TMP
                                 (RSI_NO, MTL_GROUP, PARTS_GROUP, MTL_PARTS, PART_TYPE, PARENT_LEVEL, PART_NO, PART_LEVEL, ENGLISH_NAME, PART_SPEC, USAGE, UNIT, REMARK, CREATED_BY, CREATED_DATE)
                                 values
                                 (:rsi_no, '', '', '', :part_type, :parent_level, :part_no, :part_level, :english_name, :part_spec, :usage, :unit, :remark, :emp_no, sysdate) ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, data.part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("parent_level", OracleDbType.NVarchar2, data.parent_level, ParameterDirection.Input));
                param.Add(new OracleParameter("part_no", OracleDbType.NVarchar2, data.part_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_level", OracleDbType.NVarchar2, data.part_level, ParameterDirection.Input));
                param.Add(new OracleParameter("english_name", OracleDbType.NVarchar2, data.english_name, ParameterDirection.Input));
                param.Add(new OracleParameter("part_spec", OracleDbType.NVarchar2, data.part_spec, ParameterDirection.Input));
                param.Add(new OracleParameter("usage", OracleDbType.Decimal, data.usage, ParameterDirection.Input));
                param.Add(new OracleParameter("unit", OracleDbType.NVarchar2, data.unit, ParameterDirection.Input));
                param.Add(new OracleParameter("remark", OracleDbType.NVarchar2, data.remark, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "insertDetailTempForUpload", "Insert Detail Temp Data with RD Upload BOM Data");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Ins_rd_upload_bom(string rsi_no, string part_type, string emp_no)
        {
            try
            {
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("p_rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("p_emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExecuteStoredProcedure("gpo.rsi_main_pkg.ins_rd_upload_bom", param.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateDetailByFileManagement(string rsi_no, string sn, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL d
                                 set d.updated_date = sysdate, d.updated_by = :emp_no
                                 where d.rsi_no = :rsi_no
                                 and d.sn = :sn
                                 and nvl(d.modify_type,'-')<>'D' ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Decimal, sn, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "UpdateDetailByFileManagement");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateRDDefineOwner(string rsi_no, string part_type)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL d
                                    set d.sourcer_owner = (select h.emp_no from rsi_h_authority h 
                                                        where h.bu = d.bu 
                                                            and h.mtl_parts = d.part_type
                                                            and h.type='DEFAULT' 
                                                            and h.active = 'Y'
                                                            and h.start_date <= SYSDATE
                                                            and h.end_date >= SYSDATE
                                                            and rownum = 1)
                                where d.rsi_no= :rsi_no 
                                AND NVL(d.modify_type,'-')<>'D' 
                                and d.mtl_parts = 'RD DEFINE'
                                and d.part_type = :part_type ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateRDDefineOwner", "Update方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_AUTHORITY h
                                 set h.active = 'N', h.end_date = sysdate, h.flower_num = 'Process Return'
                                 where h.type = 'ASSIGN' 
                                 and h.rsi_no = :rsi_no 
                                 and h.active = 'Y'
                                 and exists (select 1 from RSI_H_PRODUCT_DETAIL d where d.rsi_no = h.rsi_no and d.sn = h.sn and d.part_type = :part_type) ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_RSI_H_AUTHORITY", "Update方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static DataTable PartNoApprovedForSubmit(string rsi_no, string part_type, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select c.part_type,decode(c.mtl_type,'Normal','Normal Parts','Special Parts') as mtl_type,c.mtl_parts,count(1) as cnt
                                 from rsi_h_product_detail c
                                 join rsi_item_owner_v v
                                 on c.rsi_no = v.rsi_no
                                 and c.sn = v.sn
                                 where nvl(c.modify_type, '-') <> 'D'
                                 and nvl(c.isassigner,'-')<>'Y'
                                 and c.rsi_no = :rsi_no
                                 and c.part_type = :part_type
                                 and (c.sourcer_owner = :emp_no or (nvl(c.sourcer_owner,v.owner)=:emp_no))
                                 group by c.rsi_no, c.part_type, c.mtl_type, c.mtl_parts
                                 order by c.mtl_type, c.mtl_parts");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "PartNoApprovedForSubmit");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<H_Product_DetailEntity> ProductValuation(string rsi_no, string part_type, string phase_id, string sn, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select P.ISCALCULATE,
       b.DisplayPartNo as part_no,
       bom_level,
       b.mtl_group,
       b.parts_group,
       b.mtl_parts,
       b.english_name,
       b.part_spec,
       b.usage,
       b.part_unit,
       P.PRICE_PM,
       P.PMAmount,
       P.ISAPPROVED,
       P.SN
  from (select a.*, ROWNUM as seq
          from (select d.*,
                       LPAD(' ', (Level) * 4) || nvl(d.part_no, d.part_level) as DisplayPartNo,
                       Level bom_level
                  from (select distinct d.rsi_no,
                                        nvl(d.mtl_type,
                                            decode(s.active,
                                                   'Y',
                                                   'Special',
                                                   'Normal')) mtl_type,
                                        d.mtl_group,
                                        d.parts_group,
                                        d.mtl_parts,
                                        d.part_type,
                                        d.part_no,
                                        d.part_desc,
                                        d.part_level,
                                        d.english_name,
                                        d.maker_source,
                                        d.maker_part_no,
                                        d.part_spec,
                                        c.SPEC_DEF,
                                        d.release_date,
                                        d.usage,
                                        d.part_unit,
                                        d.eol_status eol_status,
                                        d.uni_spec_status uni_spec_status,
                                        d.remark,
                                        d.modify_type,
                                        d.sn,
                                        d.parent_sn,
                                        decode(nvl(f.cnt, 0), 0, 'N', 'Y') as file_status,
                                        d.phase_id,
                                        i.ref_product partnumber_top,
                                        nvl(d.part_no, d.part_level) partnumber_child,
                                        d.partnumber_parent
                          from gpo.rsi_h_product_detail d
                          join gpo.rsi_h_product_info i
                            on d.rsi_no = i.rsi_no
                          left join gpo.rsi_c_mtl_special_parts s
                            on s.mtl_parts = d.mtl_parts
                           and s.active = 'Y'
                          left join (select distinct mtl_parts, spec_def
                                      from gpo.rsi_c_mtl_spec_def
                                     where active = 'Y') c
                            on d.mtl_parts = c.mtl_parts
                          left join (SELECT Biz_Id, count(1) AS cnt
                                      FROM GPO.RSI_H_FILELIST
                                     GROUP BY Biz_Id) f
                            on f.BIZ_ID = d.RSI_NO || '_' || d.SN
                         where nvl(d.mtl_type,
                                   decode(s.active, 'Y', 'Special', 'Normal')) <>
                               'Special'
                           and (d.modify_type is null or d.modify_type <> 'D')
                           and nvl(d.mtl_group, '-') not like 'MOH%'
                           and d.rsi_no = :rsi_no) d
                --start with partnumber_top = partnumber_parent
                 start with sn = :sn
                connect by nocycle prior partnumber_top = partnumber_top
                       and prior sn = parent_sn
                 order siblings by part_level desc) a) b
  LEFT JOIN (
             --若Product Sourcer權限人員
             SELECT DISTINCT D.MTL_GROUP,
                              D.PARTS_GROUP,
                              D.MTL_PARTS,
                              D.PART_TYPE,
                              D.PART_NO,
                              D.PART_SPEC,
                              D.USAGE,
                              D.PRICE,
                              D.PRICE_PM,
                              D.MOQ,
                              D.MOCKUP,
                              D.TOOLING,
                              D.FPCA_PCBA,
                              nvl2(D.PRICE, D.USAGE * D.PRICE, null) AS SourcerAmount,
                              nvl2(D.PRICE_PM, D.USAGE * D.PRICE_PM, null) AS PMAmount,
                              NVL(D.SOURCE, '') AS SOURCE,
                              NVL(D.CURRENCY, '') AS CURRENCY,
                              NVL(D.RATE, 0) AS RATE,
                              NVL(D.SOURCE_NO, '') AS SOURCE_NO,
                              D.REMARK_PUR,
                              'Y' ISModify,
                              NVL(D.ISASSIGNER, 'N') AS ISASSIGNER,
                              NVL(D.ISAPPROVED, 'N') AS ISAPPROVED,
                              NVL(D.ISCALCULATE, 'N') AS ISCALCULATE,
                              D.SN
               FROM GPO.RSI_H_PRODUCT_DETAIL D
               JOIN GPO.RSI_H_PS_AUTHORITY H
                 ON (D.BU = H.BU AND D.PART_TYPE = H.PART_TYPE)
              WHERE (1 = 1)
                AND D.RSI_NO = :rsi_no
                AND D.MTL_TYPE = 'Normal'
                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'S')
                AND exists (select 1
                       from rsi_h_product_detail t
                      where (t.sn = d.parent_sn and
                            nvl(t.modify_type, '-') <> 'D')
                         OR d.parent_sn = 0
                         OR d.parent_sn IS NULL)
                AND H.EMP_NO = :emp_no
                AND H.ACTIVE = 'Y'
                AND H.START_DATE <= SYSDATE
                AND H.END_DATE >= SYSDATE
                AND D.PART_TYPE = :part_type) P
    ON b.SN = P.SN
 order by seq
");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.NVarchar2, sn, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraObjList< H_Product_DetailEntity>(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "ProductValuation");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ProductValuationSave(H_Product_DetailEntity model)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update GPO.RSI_H_PRODUCT_DETAIL SET 
                                 ISCALCULATE = :iscalculate
                                 WHERE RSI_NO = :rsi_no 
                                 AND SN = :sn");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("iscalculate", OracleDbType.NVarchar2, model.ISCALCULATE, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, model.RSI_NO, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.NVarchar2, model.SN, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "ProductValuationSave");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<string> GetPartNoMfg(string part_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                //sqlText.Append(@"select part_no_mfg, max(partnumber_top) || '-' || part_no_mfg as val from (
                //                    select distinct t.partnumber_top, t.partnumber_top_mfg, t.partnumber_parent as part_no, t.partnumber_parent_mfg as part_no_mfg 
                //                    from BOMCOST.H_MCA_PARTBOM_T2 t
                //                    where t.partnumber_parent=:part_no
                //                    order by t.partnumber_parent_mfg, t.partnumber_top
                //                 )
                //                 group by part_no_mfg ");

                sqlText.Append(@"select part_no_mfg, max(partnumber_top || '-' || partnumber_top_mfg) as val from (
                                    select distinct t.partnumber_top, t.partnumber_top_mfg, t.part_no, t.part_no_mfg
                                    from RSI_H_PARTBOM_MFG t
                                    where t.part_no =:part_no
                                    order by t.part_no_mfg, t.partnumber_top)
                                 group by part_no_mfg ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("part_no", OracleDbType.NVarchar2, part_no, ParameterDirection.Input));
                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "GetPartNoMfg");
                if(dt.Rows.Count > 0)
                    return dt.AsEnumerable().Select(p => p["VAL"].ToString()).ToList();
                return new List<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Gen_ref_bom_data(string partnumber_top, string partnumber_top_mfg, string part_no, string emp_no)
        {
            try
            {
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("p_lm_user", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_partno_top", OracleDbType.Varchar2, partnumber_top, ParameterDirection.Input));
                param.Add(new OracleParameter("p_partno_top_mfg", OracleDbType.Varchar2, partnumber_top_mfg, ParameterDirection.Input));
                param.Add(new OracleParameter("p_partno", OracleDbType.NVarchar2, part_no, ParameterDirection.Input));
                SqlExcute.ExecuteStoredProcedure("RSI_MAIN_PKG.gen_ref_bom_data", param.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<H_Product_DetailEntity> GetNewOtherTableData(string rsi_no, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct :rsi_no rsi_no,
                                'Normal' mtl_type,
                                d.mtl_group,
                                d.parts_group,
                                d.mtl_parts,
                                b.part_type,
                                d.partnumber_child as part_no,
                                d.part_desc,
                                d.part_level,
                                d.english_name,
                                '' maker_source,
                                d.maker_part_no,
                                d.release_date,
                                d.part_quantity as usage,
                                d.part_unit,
                                nvl(p.phase_out_flag, 'N') eol_status,
                                nvl(u.active, 'N') uni_spec_status,
                                'A' modify_type,
                                d.partnumber_top,
                                d.partnumber_child,
                                d.partnumber_parent,
                                to_number(d.bom_level) bom_level,
                                d.bom_isleaf,
                                d.lm_user
                  from gpo.rsi_h_bom_gen d
                  left join gpo.rsi_c_mtl_special_parts s
                    on s.mtl_parts = d.mtl_parts
                   and s.active = 'Y'
                  left join gpo.rsi_r_mtl_phase_out p
                    on d.partnumber_child = p.item_no
                  left join (select * from gpo.rsi_c_unique_spec where active = 'Y') u
                    on d.partnumber_child = u.part_no
                  left join (select distinct d.mtl_group, d.mtl_parts, d.part_type
                               from gpo.rsi_h_product_info i
                               join gpo.c_pms_bgbu_mapping m
                                 on m.bu = i.bu
                               join (select * from rsi_c_parts_type_def where active = 'Y') d
                                 on d.bg = m.bg
                              where rsi_no = :rsi_no) b
                    on d.mtl_group = b.mtl_group
                   and d.mtl_parts = b.mtl_parts
                 where d.lm_user = :emp_no
                   and (d.bom_isleaf = 0 or (d.bom_isleaf = 1 and d.mtl_group is not null))
                   and d.bom_level <> '1'
                 ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                var result = SqlExcute.GetOraObjList<H_Product_DetailEntity>(sqlText, param.ToArray(), "H_Product_DetailEntityDAL", "GetNewOtherTableData");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion
}