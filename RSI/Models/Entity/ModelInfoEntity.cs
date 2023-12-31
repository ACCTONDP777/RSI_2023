﻿using Oracle.ManagedDataAccess.Client;
using RSI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace RSI.Models.Entity
{
    public class ModelInfoEntity
    {
        public string PART_NO { get; set; }
        public string PART_MFG { get; set; }
    }

    public static class ModelInfoDAL
    {
        public static IList<ModelInfoEntity> GetModelInfo(string part_no)
        {
            IList<ModelInfoEntity> result = new List<ModelInfoEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct partnumber_top as part_no, partnumber_top_mfg as part_mfg from rsi_h_partbom_mfg where part_no like: part_no and partnumber_top_mfg is not null order by 1, 2 ");

                //sqlText.Append(@"select distinct partnumber_top as part_no, partnumber_top_mfg as part_mfg from bomcost.h_mca_partbom_t2 where partnumber_top like '53.32M01.X02%' and partnumber_top_mfg is not null order by 1, 2 ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("part_no", OracleDbType.Varchar2, String.Format("{0}%", part_no), ParameterDirection.Input));
                result = SqlExcute.GetOraObjList<ModelInfoEntity>(sqlText, param.ToArray(), "GetModelInfo", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static DataTable GetPartNo(string query)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select * from rsi_r_part_info where part_no like :query ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("query", OracleDbType.Varchar2, String.Format("{0}%", query), ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetPartNo", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetQueryConditions(string query)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct i.rsi_no, i.project_name , e.emp_no, e.eng_name, i.rsi_no || '/' || i.project_name || '/' || h.part_type || '/' || e.eng_name as queryresult
                                                from gpo.rsi_h_product_info i
                                                join gpo.rsi_h_form_header h on i.rsi_no = h.rsi_no and i.bu = h.bu
                                                join gfin.emp_data_all e on h.rd = e.emp_no
                                               where (1 = 1)
                                               and ((:query is null or i.rsi_no like :querylike) 
                                               OR (:query is null or UPPER(i.project_name) like UPPER(:querylike)) 
                                               OR (:query is null or UPPER(e.eng_name) like UPPER(:querylike))) 
                                               order by i.rsi_no  ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("query", OracleDbType.Varchar2, String.Format("{0}", query), ParameterDirection.Input));
                param.Add(new OracleParameter("querylike", OracleDbType.Varchar2, String.Format("{0}%", query), ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetQueryConditions", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetPartNoForUploadExcel(string query)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select count(1) from rsi_r_part_info where part_no = :query ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("query", OracleDbType.Varchar2, String.Format("{0}", query), ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "ModelInfoDAL", "GetPartKindKeyCount");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetPartKindKeyForUploadExcel(string query)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select count(1) from rsi_r_part_info where part_kind_key = :query ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("query", OracleDbType.Varchar2, String.Format("{0}", query), ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "ModelInfoDAL", "GetPartKindKeyCount");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetPartLevelEnglishForUploadExcel(string part_level, string english_name)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select count(1) from rsi_r_part_info where part_kind_key= :part_level and english_name=:english_name ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("part_level", OracleDbType.Varchar2, String.Format("{0}", part_level), ParameterDirection.Input));
                param.Add(new OracleParameter("english_name", OracleDbType.Varchar2, String.Format("{0}", english_name), ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "ModelInfoDAL", "GetPartLevelEnglishForUploadExcel");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetPartTypeForUploadExcel()
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct part_type from RSI_C_PARTS_TYPE_DEF where active='Y' ");
                return SqlExcute.GetOraDateTable(sqlText, "ModelInfoDAL", "GetPartTypeForUploadExcel");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetEnglishForUploadExcel(string rsi_no, string parent_level, string part_level)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct b.parent_level,
                                        b.part_level,
                                        b.english_name
                                 from rsi_h_product_info i
                                 join gpo.rsi_c_tmp_rule a on a.bu = i.bu
                                 join gpo.rsi_h_tmp_def b on a.id = b.id
                                 where a.active = 'Y'
                                 and a.bu = i.bu
                                 and a.code = nvl(i.product_combination_name, 'Default')
                                 and i.rsi_no = :rsi_no
                                 and b.active = 'Y'
                                 and b.parent_level=:parent_level
                                 and b.part_level=:part_level
                                 order by b.parent_level,b.part_level,b.english_name ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, String.Format("{0}", rsi_no), ParameterDirection.Input));
                param.Add(new OracleParameter("parent_level", OracleDbType.Varchar2, String.Format("{0}", parent_level), ParameterDirection.Input));
                param.Add(new OracleParameter("part_level", OracleDbType.Varchar2, String.Format("{0}", part_level), ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "ModelInfoDAL", "GetEnglishForUploadExcel");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}