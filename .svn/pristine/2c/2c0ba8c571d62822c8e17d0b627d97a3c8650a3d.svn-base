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
    public static class RSI_ConfigEntityDAL
    {
        public static int GetPMPriceConfig(string bu)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT ATTRIBUTE2 FROM GPO.RSI_C_PARAMETER 
                                               WHERE FUNCTION = 'MPPrice' AND ATTRIBUTE1 = :bu ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));

                var dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetPMPriceConfig", "獲取PMPriceConfig資訊");
                int result = Convert.ToInt32(dt.Rows[0][0]);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetExportExcelConfig(string bu, string part_type, string phase_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                //sqlText.Append(@"select * 
                //                               from gpo.rsi_c_parameter
                //                               where function = 'ExportExcel_Authority'
                //                               and decode(:BU, upper(attribute1), 1, instr(attribute1, 'ALL')) > 0    
                //                               and decode(:Part_Type, upper(attribute2), 1, INSTR(attribute2, 'ALL')) > 0
                //                               and attribute3 = :Phase_ID
                //                               order by attribute3 ");

                sqlText.Append(@"select * 
                                 from gpo.rsi_c_parameter
                                 where function = 'ExportExcel_Authority'
                                 and decode(:BU, upper(attribute1), 1, instr(attribute1, 'ALL')) > 0  ");
                if (!string.IsNullOrEmpty(part_type))
                {
                    sqlText.Append(@" and decode(:Part_Type, upper(attribute2), 1, INSTR(attribute2, 'ALL')) > 0 ");
                }

                sqlText.Append(@" and attribute3 = :Phase_ID
                                  order by attribute3");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));
                if (!string.IsNullOrEmpty(part_type))
                    param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetExportExcelConfig", String.Empty);
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public static DataTable GetExportExcelForSourcerEmpNo(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select part_type, product_sourcer from RSI_H_FORM_HEADER where rsi_no= :rsi_no and part_type = :part_type ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntity", "GetExportExcelForSourcerEmpNo");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetExprotExcelConfigForConfrim(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select part_type,price_group from RSI_H_FORM_HEADER where rsi_no= :rsi_no ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntity", "GetExprotExcelConfigForConfrim");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int GetProductReassignConfig(string bu, string part_type, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select count(*) as count
                                                from gpo.rsi_h_ps_authority
                                                where bu = :bu  --base page
                                                and part_type = :part_type --base page
                                                and emp_no = :emp_no --emp_no
                                                and type = 'FORM'
                                                and active = 'Y'
                                                and start_date <= sysdate
                                                and end_date >= sysdate ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));

                var dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetProductReassignConfig", String.Empty);
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetPriceTrend(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select distinct i.rsi_no, i.bu, a.id, a.seq, a.description 
                                                from gpo.rsi_h_product_info i
                                                join gpo.RSI_C_Price_Trend a on i.bu = a.bu and i.quto_type = a.type and a.active = 'Y'
                                                where i.rsi_no = :rsi_no
                                                order by a.seq ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetPriceTrend", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int GetAuthorityForProductSoucer(string rsi_no, string part_type, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select count(1)
                                    from rsi_h_product_info a
                                    left join RSI_H_PS_AUTHORITY b
                                    on a.bu = b.bu
                                    where b.start_date <= sysdate
                                    and b.end_date >= sysdate
                                    and b.active='Y'
                                    and a.rsi_no = :rsi_no
                                    and b.part_type = :part_type
                                    and b.emp_no = :emp_no ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetAuthorityForProductSoucer", String.Empty);
                int result = 0;
                if (dt.Rows.Count > 0)
                    result = Convert.ToInt32(dt.Rows[0][0]);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetRMSUser(string emp_no, string boss_level)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct N''||a.boss_emp_no as boss_emp_no
                                from au_dw.auo_person_reportline a
                                where a.emp_no= :emp_no
                                and a.boss_level>= :boss_level
                                union
                                select :emp_no as boss_emp_no from dual ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("boss_level", OracleDbType.NVarchar2, boss_level, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetEmployeeBoss", String.Empty);
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public static void ExportLog(string function, string page_name, string rsi_no, string part_type, string form_no, string rms_authority, string rms_userlist)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"insert into RSI_H_FUNC_LOG 
                                (attribute1, attribute2, attribute3, attribute4, attribute5, attribute6, attribute7, created_by, created_date)
                                values (:function, :page_name, :rsi_no, :part_type, :form_no, :rms_authority, :rms_userlist, :emp_no, sysdate) ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("function", OracleDbType.NVarchar2, function, ParameterDirection.Input));
                param.Add(new OracleParameter("page_name", OracleDbType.NVarchar2, page_name, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("form_no", OracleDbType.NVarchar2, form_no, ParameterDirection.Input));
                param.Add(new OracleParameter("rms_authority", OracleDbType.NVarchar2, rms_authority, ParameterDirection.Input));
                param.Add(new OracleParameter("rms_userlist", OracleDbType.NVarchar2, rms_userlist, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "ExportLog");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Upload_Flag(string phase_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select attribute3 from RSI_C_PARAMETER
                                 where function='BULK_UPLOAD' and attribute2 = :phase_id  ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                var dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "Upload_Flag");
                if (dt.Rows.Count > 0)
                {
                    var attribute3 = dt.Rows[0][0].ToString();
                    return attribute3;
                }

                return "N";
            }
            catch (Exception ex)
            {
                return "N";
            }
        }

        public static DataTable GetSigningNotify(string attribute1, string phase_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select t.* from RSI_C_PARAMETER t
                                 where t.function='SIGNING_NOTICE' and t.attribute1= :attribute1 and t.attribute3= :phase_id ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("attribute1", OracleDbType.NVarchar2, attribute1, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetSigningNotice");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetSigningNotice(string bu, string attribute1, string phase_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select distinct t.attribute1,t.attribute2,t.attribute3,t.attribute4,t.attribute5,t.attribute6,t.attribute7
                                 from RSI_C_PARAMETER t
                                 join RSI_C_FLOW_DEF d
                                 on t.attribute3 = decode(:attribute1, 'RESEND', d.phase_id, 'APPROVE', d.next_phase_id, d.last_phase_id)
                                 where t.function = 'SIGNING_NOTICE' 
                                 and t.attribute1 = :attribute1 
                                 and d.phase_id = :phase_id
                                 and d.bu = :bu 
                                 and t.attribute2 = 'Y' ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("attribute1", OracleDbType.NVarchar2, attribute1, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetSigningNotice");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetCCMail(string attr3, string attr4, string attr5, string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select a.phase, a.phase_id, a.app_assigner,a.e_mail as mailto, decode( :attr4, 'Y', listagg(a.boss_email,';') WITHIN GROUP (ORDER BY a.seq desc), null) AS mailcc   
                                 from (
                                 select fd.phase,fa.phase_id,fa.app_assigner,ee.e_mail,r.seq,e.e_mail as boss_email
                                 from RSI_H_FORM_HEADER fh
                                 join RSI_H_FORM_APPROVE fa
                                 on fh.form_no = fa.form_no
                                 join RSI_H_PRODUCT_INFO p
                                 on p.rsi_no = fh.rsi_no
                                 join (select distinct bu, phase_id, phase from RSI_C_FLOW_DEF) fd
                                 on fd.phase_id = fa.phase_id
                                 and fh.bu = fd.bu
                                 join rsi_to_do_list_v s
                                 on s.rsi_no = fh.rsi_no and s.form_no = fh.form_no and s.APP_ASSIGNER = fa.app_assigner
                                 and s.phase_id = fa.phase_id
                                 left join au_dw.auo_person_reportline r
                                 on r.emp_no = fa.app_assigner
                                 and r.seq <= :attr5
                                 and r.boss_level >= 20
                                 left join gfin.emp_data_all ee
                                 on ee.emp_no = fa.app_assigner
                                 left join gfin.emp_data_all e
                                 on e.emp_no = trim(r.boss_emp_no)
                                 where fh.rsi_no = :rsi_no and fh.part_type = :part_type
                                 and fa.app_status = 'U'
                                 and fa.phase_id = :attr3
                                 ) a
                                 GROUP BY a.phase, a.phase_id, a.app_assigner, a.e_mail");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("attr3", OracleDbType.NVarchar2, attr3, ParameterDirection.Input));
                param.Add(new OracleParameter("attr4", OracleDbType.NVarchar2, attr4, ParameterDirection.Input));
                param.Add(new OracleParameter("attr5", OracleDbType.NVarchar2, attr5, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetCCMail");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetNotifyMail(string attr6, string attr7, string emp_no, string bu, string phase_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select decode( :attr6, 'Y', listagg(a.boss_email, ';') WITHIN GROUP(ORDER BY a.seq desc), null) AS notifyto, a.e_mail as notifycc, a.phase 
                                 from (
                                 select: emp_no, ee.e_mail, r.seq, e.e_mail as boss_email,d.phase
                                 from au_dw.auo_person_reportline r
                                 left join gfin.emp_data_all ee
                                 on ee.emp_no = :emp_no
                                 left join gfin.emp_data_all e
                                 on e.emp_no = trim(r.boss_emp_no)
                                 left join (select distinct bu,phase_id, phase from RSI_C_FLOW_DEF) d
                                 on d.bu = :bu
                                 and d.phase_id = :phase_id
                                 where r.emp_no = :emp_no
                                 and r.seq <= :attr7
                                 and r.boss_level >= 20
                                 ) a
                                 GROUP BY a.e_mail, a.phase");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("attr6", OracleDbType.NVarchar2, attr6, ParameterDirection.Input));
                param.Add(new OracleParameter("attr7", OracleDbType.NVarchar2, attr7, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetNotifyMail");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetCCMailForPhase40(string attr4, string attr5, string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select a.phase, a.phase_id, a.app_assigner, a.e_mail as mailto,null AS mailcc   
                                 from (
                                 select t.phase,t.phase_id,t.app_assigner,ee.e_mail
                                 from RSI_TO_DO_LIST_V t
                                 left join gfin.emp_data_all ee
                                 on ee.emp_no = t.APP_ASSIGNER
                                 where t.rsi_no = :rsi_no and t.part_type = :part_type
                                 and t.RETURN_PHASE_ID = '40'
                                 ) a
                                 GROUP BY a.phase, a.phase_id, a.app_assigner, a.e_mail ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("attr4", OracleDbType.NVarchar2, attr4, ParameterDirection.Input));
                param.Add(new OracleParameter("attr5", OracleDbType.NVarchar2, attr5, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetCCMail");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable GetMailToApprover(string rsi_no, string part_type)
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"select a.rsi_no, a.part_type, listagg(a.e_mail,';') WITHIN GROUP (ORDER BY a.sourcer) as mailto
                                 from (
                                 select distinct t.rsi_no, t.part_type,t.APP_ASSIGNER as sourcer, e.e_mail
                                 from RSI_DASHBOARD_LAYER1_SOURCER_V t
                                 left join gfin.emp_data_all e
                                 on e.emp_no = t.APP_ASSIGNER
                                 where t.rsi_no=:rsi_no
                                 and t.part_type=:part_type
                                 and t.approved='R') a
                                 GROUP BY a.rsi_no, a.part_type ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetMailToApprover");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetMailCCApprover(string rsi_no, string part_type)
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"select a.rsi_no, a.part_type, listagg(a.e_mail,';') WITHIN GROUP (ORDER BY a.ps) as mailcc
                                 from (
                                 select distinct t.rsi_no, t.part_type,t.PRODUCTSOURCER as ps, e.e_mail
                                 from RSI_DASHBOARD_LAYER1_SOURCER_V t
                                 left join gfin.emp_data_all e
                                 on e.emp_no = t.PRODUCTSOURCER
                                 where t.rsi_no=:rsi_no
                                 and t.part_type=:part_type
                                 and t.approved='R') a
                                 GROUP BY a.rsi_no, a.part_type ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetMailCCApprover");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetMailWithdraw(string rsi_no, string part_type, string bu, string phase_id)
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"select a.phase, listagg(a.e_mail,';') WITHIN GROUP (ORDER BY a.app_assigner) as mailto
                                from (
                                select distinct h.rsi_no, h.part_type, t.app_assigner, e.e_mail, d.phase
                                from RSI_H_FORM_HEADER h 
                                join RSI_H_FORM_APPROVE t
                                on h.form_no = t.form_no
                                left join gfin.emp_data_all e
                                on e.emp_no = t.app_assigner
                                left join (select distinct bu,phase_id, phase from RSI_C_FLOW_DEF) d
                                on d.bu = :bu
                                and d.phase_id = :phase_id
                                where h.rsi_no = :rsi_no
                                and h.part_type = :part_type
                                and t.app_status = 'C'
                                and t.app_serial = (select max(app_serial)-1 from  RSI_H_FORM_APPROVE where form_no= h.form_no)
                                ) a
                                GROUP BY a.phase ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetMailWithdraw");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetMail(string emp_no)
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"select e.e_mail as email
                                 from gfin.emp_data_all e
                                 where e.emp_no = :emp_no ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetMail");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LoginLog(string emp_no)
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"INSERT INTO rsi_h_func_log(attribute1,created_by,created_date)
                                 VALUES('Login',:emp_no,SYSDATE) ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "LoginLog");
            }
            catch (Exception ex)
            {

            }
        }

        public static DataTable GetMCAConrol()
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"select * from RSI_C_PARAMETER t
                                 where t.function='MAC_CONTROL'
                                 and t.attribute1='Y' ");
                return SqlExcute.GetOraDateTable(sqlText, "RSI_ConfigEntityDAL", "GetMCAConrol");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetBulletin()
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"select t.*,dbms_lob.substr(t.note) as sys_note from RSI_C_PARAMETER t
                                 where t.function='BULLETIN' ");
                return SqlExcute.GetOraDateTable(sqlText, "RSI_ConfigEntityDAL", "GetBulletin");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetVersion()
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"select attribute1 from RSI_C_PARAMETER where function='VERSION_CONTROL' ");
                DataTable dt = SqlExcute.GetOraDateTable(sqlText, "RSI_ConfigEntityDAL", "GetVersion");
                return dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable GetRMSConrol()
        {
            try
            {
                var sqlText = new StringBuilder();
                sqlText.Append(@"select * from RSI_C_PARAMETER t
                                 where t.function='RMS_CONTROL'
                                 and t.attribute1='Y' ");
                return SqlExcute.GetOraDateTable(sqlText, "RSI_ConfigEntityDAL", "GetRMSConrol");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetSourcerPhase(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select * from RSI_H_FORM_APPROVE
                                  where phase_id = '40'
                                    and app_status = 'U'
                                    and (form_no, app_serial) in (select form_no, max(app_serial) from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type group by form_no) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.NVarchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                return SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetSourcerPhase", String.Empty);
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public static List<EmployeeModel> GetMaterialSourcerReassignBindding(string bu, string mtl_parts)
        {
            var result = new List<EmployeeModel>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT DISTINCT E.EMP_NO, E.ENG_NAME AS EMP_NAME
                                 FROM GPO.RSI_H_AUTHORITY H
                                 join GFIN.EMP_DATA_ALL E
                                 on H.EMP_NO = E.EMP_NO
                                 where H.BU = :bu
                                 and H.Mtl_Parts = :mtl_parts
                                 and H.START_DATE <= SYSDATE
                                 AND H.END_DATE >= SYSDATE
                                 AND H.ACTIVE = 'Y' 
                                 AND E.ACTIVE = 'Y'
                                 and h.type = 'FORM' ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.NVarchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("mtl_parts", OracleDbType.NVarchar2, mtl_parts, ParameterDirection.Input));
                result = SqlExcute.GetOraObjList<EmployeeModel>(sqlText, param.ToArray(), "RSI_ConfigEntityDAL", "GetMaterialSourcerReassignBindding").ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}