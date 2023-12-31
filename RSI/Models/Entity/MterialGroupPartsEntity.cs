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
    public class MterialGroupPartsEntity
    {
        public string BG { get; set; }
        public string BU { get; set; }
        public string MTL_GROUP { get; set; }
        public string MTL_PARTS { get; set; }
        public string SPEC_DEF { get; set; }
    }

    public static class MterialGroupPartsDAL
    {
        public static IList<MterialGroupPartsEntity> GetMterialGroupParts(string bu, string part_type)
        {
            IList<MterialGroupPartsEntity> models = new List<MterialGroupPartsEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select * 
                                               from gpo.rsi_c_parts_type_def p
                                               join gpo.c_pms_bgbu_mapping m on p.bg = m.bg and m.active = 'Y' and decode(p.bu, '', 1, instr(p.bu, m.bu)) > 0
                                               left join gpo.rsi_c_mtl_spec_def s on p.mtl_parts = s.mtl_parts and s.active = 'Y'              
                                               where p.active = 'Y'
                                               and m.bu = :bu
                                               and p.part_type = decode(p.mtl_group, 'RD DEFINE', :part_type, p.part_type) 
                                               order by decode(p.mtl_group, 'RD DEFINE', 0, 1), p.mtl_group, p.mtl_parts ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.Varchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                models = SqlExcute.GetOraObjList<MterialGroupPartsEntity>(sqlText, param.ToArray(), "GetMterialGroupParts", String.Empty);
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return models;
        }

        public static IList<string> GetUnit()
        {
            IList<string> models = new List<string>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct upper(unit) unit
                                               from rsi_r_part_info
                                               order by unit ");
                var dt = SqlExcute.GetOraDateTable(sqlText, "GetUnit", String.Empty);
                foreach (DataRow row in dt.Rows)
                {
                    models.Add(row[0].ToString());
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return models;
        }
    }
}