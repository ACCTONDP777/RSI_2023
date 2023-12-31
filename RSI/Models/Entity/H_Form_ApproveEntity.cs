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
    #region DTO
    public static class H_Form_ApproveEntity
    {
    }
    #endregion

    #region DAL
    public static class H_Form_ApproveEntityDAL
    {
        public static bool Approve(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE a
                    set a.app_status = 'A', a.end_date= sysdate, 
                    a.app_assigner = decode(a.app_assigner, NULL, :emp_no, a.app_assigner),
                    a.app_actor= :emp_no, a.app_content= :p_comment
                    where a.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                    and a.phase_id = :phase_id 
                    and a.app_status = 'U' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
               
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve", "Update方法");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
            return result;
        }

        public static bool insert_Approve(string emp_no, string comment, string rsi_no, string part_type, string phase_id, string tomanager)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();

                if (phase_id.Equals("50") && tomanager.Equals("N"))
                {
                    sqlText.Append(@"
                                    insert into RSI_H_FORM_APPROVE(form_no, phase_id, app_serial, app_status, begin_Date)
                                    select fh.form_no, fd.next_phase_id, max(fa.app_serial)+1, 'U', sysdate 
                                    from RSI_H_FORM_HEADER fh, RSI_H_FORM_APPROVE fa, RSI_C_FLOW_DEF fd
                                    where fh.rsi_no = :rsi_no and fh.part_type = :part_type
                                    and fh.form_no = fa.form_no
                                    and fh.bu = fd.bu
                                    and fd.phase_id = :phase_id
                                    group by fh.form_no, fd.next_phase_id
                                    order by fd.next_phase_id
                                ");
                }
                else
                {
                    sqlText.Append(@"
                                    insert into RSI_H_FORM_APPROVE(form_no, phase_id, app_serial, app_status, begin_Date)
                                    select fh.form_no, fd.next_phase_id, max(fa.app_serial)+1, 'U', sysdate 
                                    from RSI_H_FORM_HEADER fh, RSI_H_FORM_APPROVE fa, RSI_C_FLOW_DEF fd
                                    where fh.rsi_no = :rsi_no and fh.part_type = :part_type
                                    and fh.form_no = fa.form_no
                                    and fa.phase_id = fd.phase_id
                                    and fh.bu = fd.bu
                                    and fa.phase_id = :phase_id
                                    group by fh.form_no, fd.next_phase_id
                                    order by fd.next_phase_id
                                ");
                }

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id.Equals("50") && tomanager.Equals("N") ? "60" : phase_id, ParameterDirection.Input));
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "insert_Approve", "Insert方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static int count_Approve(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@" select count(*) from gpo.rsi_h_form_approve
                                                where (form_no, app_serial) in (select form_no, max(app_serial)
                                                                              from gpo.rsi_h_form_header
                                                                             where rsi_no = :rsi_no
                                                                             and part_type = :part_type group by form_no)
                                                and app_status = 'U' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                var result = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "count_Approve", "Update方法");
                return Convert.ToInt32(result.Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert_Sourcer_Log(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    insert into RSI_H_DETAIL_APP_LOG(rsi_no, sn, form_no, phase_id, app_serial, app_actor, app_status, app_content, created_by, created_date)
                    select pd.rsi_no, pd.sn, fh.form_no, :phase_id, fa.app_serial,:emp_no, 'A', :p_comment, :emp_no, sysdate   --從querystring的phase_id取得
                    from RSI_H_PRODUCT_DETAIL pd
                    join RSI_H_FORM_HEADER fh
                    on pd.rsi_no = fh.rsi_no and pd.part_type = fh.part_type
                    join RSI_H_FORM_APPROVE fa
                    on fa.form_no = fh.form_no
                    where pd.rsi_no = :rsi_no and pd.part_type= :part_type  --從querystring的rsi_no、part_type取得
                    and nvl(pd.modify_type,'-')<>'D'
                    and fa.app_status = 'U'
                    and nvl(pd.isassigner,'-') = 'Y'
                    and fa.phase_id = :phase_id ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Insert_Sourcer_Log", "Insert RSI_H_DETAIL_LOG方法");

                //正常Approve不需要update phase_id,Reject/Return才要。
                //sqlText = new StringBuilder();
                //sqlText.Append(@"
                //    update RSI_H_PRODUCT_DETAIL pd
                //    set pd.phase_id = (select fd.next_phase_id from RSI_C_FLOW_DEF fd where pd.bu = fd.bu and fd.phase_id = :phase_id)  --從querystring的phase_id取得
                //    where pd.rsi_no = :rsi_no and pd.part_type= :part_type  --從querystring的rsi_no、part_type取得
                //    and exists (select 1 from RSI_H_FORM_HEADER fh,RSI_H_AUTHORITY ha
                //    where pd.bu = ha.bu and pd.mtl_parts = ha.mtl_parts
                //    and ha.emp_no =:emp_no)");

                //param = new List<OracleParameter>();
                //param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                //param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                //param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                //param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                //result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_Sourcer", "Update RSI_H_PRODUCT_DETAIL方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static bool Approve_SourcerForRecordeSN(string emp_no, string comment, string rsi_no, string part_type, string phase_id, string sn)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    insert into RSI_H_DETAIL_APP_LOG(rsi_no, sn, form_no, phase_id, app_serial, app_actor, app_status, app_content, created_by, created_date)
                    select distinct pd.rsi_no, pd.sn, fh.form_no, :phase_id, fa.app_serial,:emp_no, 'A', :p_comment, :emp_no, sysdate   --從querystring的phase_id取得
                    from RSI_H_PRODUCT_DETAIL pd
                    join RSI_H_FORM_HEADER fh
                    on pd.rsi_no = fh.rsi_no and pd.part_type = fh.part_type
                    join RSI_H_FORM_APPROVE fa
                    on fa.form_no = fh.form_no
                    where pd.rsi_no = :rsi_no and pd.part_type= :part_type  --從querystring的rsi_no、part_type取得
                    and nvl(pd.modify_type,'-')<>'D'
                    and fa.app_status = 'U'
                    and nvl(pd.isassigner,'-') = 'Y'
                    and fa.phase_id = :phase_id
                    and pd.sn = :sn ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Varchar2, sn, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_Sourcer", "Insert Approve_SourcerForRecordeSN方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static bool Approve_SourcerForRecordeNoSN(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    insert into RSI_H_DETAIL_APP_LOG(rsi_no, sn, form_no, phase_id, app_serial, app_actor, app_status, app_content, created_by, created_date)
                    select distinct fh.rsi_no, null, fh.form_no, :phase_id, fa.app_serial,:emp_no, 'A', :p_comment, :emp_no, sysdate
                    from RSI_H_FORM_HEADER fh
                    join RSI_H_FORM_APPROVE fa
                    on fa.form_no = fh.form_no
                    where fh.rsi_no = :rsi_no 
                    and fh.part_type= :part_type 
                    and fa.app_status = 'U'
                    and fa.phase_id = :phase_id ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_Sourcer", "Insert Approve_SourcerForRecordeNoSN方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static void Approve_PMConfirm(string emp_no, string comment, string rsi_no, string phase_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE a
                    set a.app_status = 'A', a.end_date=sysdate, a.app_actor= :emp_no,a.app_content=:p_comment
                    where a.form_no IN (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no)  --從querystring的rsi_no、part_type取得
                    and a.phase_id = :phase_id  --從querystring的phase_id取得
                    and a.app_status = 'U' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_PMConfirm", "Update RSI_H_FORM_APPROVE方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_HEADER a
                    set a.form_status = 'AP', a.end_date = sysdate
                    where a.rsi_no = :rsi_no  --從querystring的rsi_no取得");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_PMConfirm", "Update RSI_H_FORM_HEADER方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_PRODUCT_INFO a
                    set a.RSI_STATUS = 'AP'
                    where a.rsi_no = :rsi_no --從querystring的rsi_no取得");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_PMConfirm", "Update RSI_H_PRODUCT_INFO方法");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Approve_SourcerConfirm(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE a
                    set a.app_status = 'A', a.end_date=sysdate, a.app_actor= :emp_no,a.app_content=:p_comment
                    where a.form_no IN (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no)  --從querystring的rsi_no、part_type取得
                    and a.phase_id = :phase_id  --從querystring的phase_id取得
                    and a.app_status = 'U' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_SourcerConfirm", "Update RSI_H_FORM_APPROVE方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    insert into RSI_H_FORM_APPROVE(form_no, phase_id, app_serial, app_status, app_assigner, app_actor, begin_Date, end_date, APP_CONTENT)
                    select fh.form_no, fd.next_phase_id, max(fa.app_serial)+1, 'A', fh.pm, fh.pm, sysdate, sysdate, 'Auto Complete by System'
                    from RSI_H_FORM_HEADER fh, RSI_H_FORM_APPROVE fa, RSI_C_FLOW_DEF fd
                    where fh.rsi_no = :rsi_no and fh.part_type = :part_type
                    and fh.form_no = fa.form_no
                    and fa.phase_id = fd.phase_id
                    and fh.bu = fd.bu
                    and fa.phase_id = :phase_id
                    group by fh.form_no, fd.next_phase_id, fh.pm
                    order by fd.next_phase_id ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_SourcerConfirm", "Insert RSI_H_FORM_APPROVE方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_HEADER a
                    set a.form_status = 'AP', a.end_date = sysdate
                    where a.rsi_no = :rsi_no  --從querystring的rsi_no取得");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_SourcerConfirm", "Update RSI_H_FORM_HEADER方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_PRODUCT_INFO a
                    set a.RSI_STATUS = 'AP'
                    where a.rsi_no = :rsi_no --從querystring的rsi_no取得");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_SourcerConfirm", "Update RSI_H_PRODUCT_INFO方法");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Reject(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE a
                    set a.app_status = 'R', a.end_date=sysdate,
                    a.app_actor= decode(a.phase_id, :phase_id, :emp_no, '') , 
                    a.app_content= decode(a.phase_id, :phase_id, :p_comment, '')
                    where a.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)  --從querystring的rsi_no、part_type取得
                    --and a.phase_id = :phase_id  --從querystring的phase_id取得  --註解:因為流程有同時簽核的問題，先把Phase_ID條件拿掉
                    and a.app_status = 'U' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reject", "Update方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                   insert into RSI_H_FORM_APPROVE(form_no, phase_id, app_serial, app_status, begin_Date)
                    select fh.form_no, fd.last_phase_id, max(fa.app_serial)+1, 'U', sysdate 
                    from RSI_H_FORM_HEADER fh, RSI_H_FORM_APPROVE fa, RSI_C_FLOW_DEF fd
                    where fh.rsi_no = :rsi_no and fh.part_type = :part_type
                    and fh.form_no = fa.form_no
                    and fa.phase_id = fd.phase_id
                    and fh.bu = fd.bu
                    and fa.phase_id = :phase_id
                    group by fh.form_no, fd.last_phase_id
                ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reject", "Insert方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static bool Reject_Sourcer(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE a
                    set a.app_status = 'R', a.end_date=sysdate,
                    a.app_actor= :emp_no, 
                    a.app_content= :p_comment
                    where a.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                    and a.phase_id = :phase_id
                    and a.app_status = 'U'
                    and a.app_assigner = :emp_no");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reject_Sourcer", "Update方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                   insert into RSI_H_FORM_APPROVE(form_no, phase_id, app_serial, app_status, begin_Date)
                    select fh.form_no, fd.last_phase_id, max(fa.app_serial)+1, 'U', sysdate 
                    from RSI_H_FORM_HEADER fh, RSI_H_FORM_APPROVE fa, RSI_C_FLOW_DEF fd
                    where fh.rsi_no = :rsi_no and fh.part_type = :part_type
                    and fh.form_no = fa.form_no
                    and fa.phase_id = fd.phase_id
                    and fh.bu = fd.bu
                    and fa.phase_id = :phase_id
                    group by fh.form_no, fd.last_phase_id
                ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reject_Sourcer", "Insert方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static bool Reject_Sourcer_Log(string emp_no, string comment, string rsi_no, string part_type, string phase_id, int[] snarr)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_PRODUCT_DETAIL pd
                    set pd.phase_id = (select min(fd.last_phase_id) from RSI_C_FLOW_DEF fd where pd.bu = fd.bu and fd.phase_id = :phase_id)  --考量sourcer主管站點退回的關卡不只一個
                        ,pd.price = 0
                        ,pd.price_pm = 0
                        ,pd.price_his_h = 0
                        ,pd.price_his_l = 0
                        ,pd.moq = 0    
                        ,pd.mockup = 0
                        ,pd.tooling = 0    
                  where pd.rsi_no = :rsi_no and pd.part_type= :part_type  --從querystring的rsi_no、part_type取得
                    and pd.sn in (:SN) --頁面上勾選Return的資料 ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                sqlText = sqlText.Replace(":SN", String.Join(",", snarr));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reject_Sourcer_Log", "Update方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                   insert into RSI_H_DETAIL_APP_LOG(rsi_no, sn, form_no, phase_id, app_serial, app_actor, app_status, app_content, created_by, created_date, begin_date, end_date)
                   select distinct pd.rsi_no, pd.sn, fh.form_no, :phase_id, fa.app_serial, :emp_no, 'R',:p_comment, :emp_no, sysdate, fa.begin_date, sysdate   --從querystring的phase_id取得
                   from RSI_H_PRODUCT_DETAIL pd
                   join RSI_H_FORM_HEADER fh
                   on pd.rsi_no = fh.rsi_no and pd.part_type = fh.part_type
                   join RSI_H_FORM_APPROVE fa
                   on fa.form_no = fh.form_no
                   where pd.rsi_no = :rsi_no and pd.part_type=:part_type  --從querystring的rsi_no、part_type取得
                   and nvl(pd.modify_type,'-')<>'D'
                   and fa.app_status = 'U'
                   --and pd.phase_id = (select distinct fd.last_phase_id from RSI_C_FLOW_DEF fd where pd.bu = fd.bu and fd.phase_id = :phase_id)  --用sn判定reject那幾筆即可
                   and fa.phase_id = :phase_id
                   and fa.app_assigner = :emp_no
                   and pd.sn in (:SN)
                ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                sqlText = sqlText.Replace(":SN", String.Join(",", snarr));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reject_Sourcer_Log", "Insert方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static void Reject_Return(string emp_no, string comment, string rsi_no, string part_type, string phase_id, string now_phase)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE a
                    set a.app_status = 'C', a.end_date=sysdate,
                    a.app_assigner = decode(a.app_assigner, NULL, :emp_no, a.app_assigner), 
                    a.app_actor= :emp_no, a.app_content= :p_comment
                    where a.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)  --從querystring的rsi_no、part_type取得
                    --and a.phase_id = :now_phase  --從querystring的phase_id取得
                    and a.app_status = 'U' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("now_phase", OracleDbType.Varchar2, now_phase, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reject", "Update方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                   insert into RSI_H_FORM_APPROVE(form_no, phase_id, app_serial, app_status, begin_Date)
                    select distinct fh.form_no, :phase_id, max(fa.app_serial)+1, 'U', sysdate 
                    from RSI_H_FORM_HEADER fh, RSI_H_FORM_APPROVE fa, RSI_C_FLOW_DEF fd
                    where fh.rsi_no = :rsi_no and fh.part_type = :part_type
                    and fh.form_no = fa.form_no
                    and fa.phase_id = fd.phase_id
                    and fh.bu = fd.bu
                    and (fa.phase_id = :now_phase or fa.phase_id = '45')  --考量表單無材料,只會簽Product Sourcer的狀況
                    group by fh.form_no, fd.last_phase_id
                ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("now_phase", OracleDbType.Varchar2, now_phase, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Reject", "Insert方法");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update_Assigner(string rsi_no, string part_type, string phase_id, string emp_no)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                var param = new List<OracleParameter>();
                if (phase_id.Equals("10"))
                {
                    sqlText = new StringBuilder();
                    sqlText.Append(@"
                        update RSI_H_FORM_HEADER fh
                        set fh.rd_boss = (select e.boss_no from gfin.emp_data_all e where e.emp_no = fh.rd),
                            fh.rd_manager = (select e.boss_no from gfin.emp_data_all e where e.emp_no = (select e.boss_no from gfin.emp_data_all e where e.emp_no = fh.rd))
                        where fh.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                        and fh.form_status = 'UA' ");
                    param = new List<OracleParameter>();
                    param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                    SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Assigner", "Update_phase_10");
                }

                if (phase_id.Equals("45"))  //更新Sourcer Boss & Sourcer Manager,若上兩層主管為處級，改抓部級。
                {
                    sqlText = new StringBuilder();
                    sqlText.Append(@"
                        UPDATE rsi_h_form_header fh
                           SET fh.sourcer         = :emp_no,
                               fh.sourcer_boss   =
                               (SELECT e.boss_no
                                  FROM gfin.emp_data_all e
                                  JOIN gfin.emp_data_all b
                                    ON e.boss_no = b.emp_no
                                 WHERE e.emp_no = :emp_no
                                   AND b.boss_level > '10'),
                                   fh.sourcer_manager = nvl((SELECT e.boss_no AS manager
                                                          FROM gfin.emp_data_all e
                                                          JOIN gfin.emp_data_all b
                                                            ON e.boss_no = b.emp_no
                                                            WHERE e.emp_no =
                                                               (SELECT e.boss_no
                                                                  FROM gfin.emp_data_all e
                                                                  JOIN gfin.emp_data_all b
                                                                    ON e.boss_no = b.emp_no
                                                                 WHERE e.emp_no = :emp_no
                                                                 AND b.boss_level > '10')
                                                           AND b.boss_level > '10'),
                                                        ((SELECT e.boss_no
                                                            FROM gfin.emp_data_all e
                                                            JOIN gfin.emp_data_all b
                                                              ON e.boss_no = b.emp_no
                                                           WHERE e.emp_no = :emp_no
                                                           AND b.boss_level > '10')))
                         WHERE fh.form_no = (SELECT form_no
                                               FROM rsi_h_form_header
                                              WHERE rsi_no = :rsi_no
                                                AND part_type = :part_type
                                             )
                           AND fh.form_status = 'UA' ");
                    param = new List<OracleParameter>();
                    param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                    SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Assigner", "Update_phase_40");
                }

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE fa
                    set fa.app_assigner = (
					--select case fa.phase_id when '10' then fh.rd when '20' then fh.rd_boss when '30' then fh.pm when '40' then fh.sourcer when '50' then fh.sourcer_boss when '60' then fh.sourcer_manager when '70' then fh.pm else null end from RSI_H_FORM_HEADER fh where fh.form_no = fa.form_no
					select case
					 when (fa.phase_id = '10') then fh.rd 
					 when (fa.phase_id = '20') then fh.rd_boss 
                     when (fa.phase_id = '25') then fh.rd_manager 
					 when (fa.phase_id = '30' and fh.bu not in ('TV','PD')) then fh.pm 
					 when (fa.phase_id = '30' and fh.bu in ('TV','PD')) then fh.pl_rd
					 --when (fa.phase_id = '30' and m.bg = 'MS') then fh.pm 
					 --when (fa.phase_id = '30' and m.bg = 'VS') then fh.pl_rd
					 --when (fa.phase_id = '40') then fh.sourcer 
                     when (fa.phase_id = '45') then fh.product_sourcer 
					 when (fa.phase_id = '50') then fh.sourcer_boss 
					 when (fa.phase_id = '60') then fh.sourcer_manager
					 when (fa.phase_id = '70') then fh.pm else null end 
					 from RSI_H_FORM_HEADER fh
					 join gpo.c_pms_bgbu_mapping m on fh.bu = m.bu and m.active = 'Y'
					 where fh.form_no = fa.form_no
					 )
                     where fa.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                     and fa.app_status='U' ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Assigner", "Update_Assigner方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static void Update_AssignerForSourcerReturn(string rsi_no, string part_type) {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                var param = new List<OracleParameter>();
                sqlText = new StringBuilder();
                sqlText.Append(@"
                        update RSI_H_FORM_HEADER fh
                        set fh.sourcer = '',
                        fh.sourcer_boss   = '',
                        fh.sourcer_manager = ''
                        where fh.form_no = (select form_no
                        from RSI_H_FORM_HEADER
                        where rsi_no = :rsi_no
                        and part_type = :part_type)
                        and fh.form_status = 'UA'  ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_AssignerForSourcerReturn", "SourcerReturn方法");

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE fa
                    set fa.app_assigner = (
					--select case fa.phase_id when '10' then fh.rd when '20' then fh.rd_boss when '30' then fh.pm when '40' then fh.sourcer when '50' then fh.sourcer_boss when '60' then fh.sourcer_manager when '70' then fh.pm else null end from RSI_H_FORM_HEADER fh where fh.form_no = fa.form_no
					select case
					 when (fa.phase_id = '10') then fh.rd 
					 when (fa.phase_id = '20') then fh.rd_boss 
                     when (fa.phase_id = '25') then fh.rd_manager 
					 when (fa.phase_id = '30' and fh.bu not in ('TV','PD')) then fh.pm 
					 when (fa.phase_id = '30' and fh.bu in ('TV','PD')) then fh.pl_rd
					 --when (fa.phase_id = '30' and m.bg = 'MS') then fh.pm 
					 --when (fa.phase_id = '30' and m.bg = 'VS') then fh.pl_rd
					 --when (fa.phase_id = '40') then fh.sourcer 
					 when (fa.phase_id = '50') then fh.sourcer_boss 
					 when (fa.phase_id = '60') then fh.sourcer_manager
					 when (fa.phase_id = '70') then fh.pm else null end 
					 from RSI_H_FORM_HEADER fh
					 join gpo.c_pms_bgbu_mapping m on fh.bu = m.bu and m.active = 'Y'
					 where fh.form_no = fa.form_no
					 )
                     where fa.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                     and fa.app_status='U' ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_AssignerForSourcerReturn", "SourcerReturn方法");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_AssignerForSourcer(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                var param = new List<OracleParameter>();
                sqlText = new StringBuilder();
                sqlText.Append(@"
                   update RSI_H_FORM_APPROVE fa
                    set fa.app_assigner=''
                    where fa.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)   --從querystring的rsi_no、part_type取得
                    and fa.app_status='U' ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_AssignerForSourcer", "Update_AssignerForSourcer方法");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_AssignerForReturn(string rsi_no, string part_type, string phase_id, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                var param = new List<OracleParameter>();
                if (phase_id.Equals("10"))
                {
                    sqlText = new StringBuilder();
                    sqlText.Append(@"
                        update RSI_H_FORM_HEADER fh
                        set fh.rd_boss = (select e.boss_no from gfin.emp_data_all e where e.emp_no = fh.rd),
                            fh.rd_manager = (select e.boss_no from gfin.emp_data_all e where e.emp_no = (select e.boss_no from gfin.emp_data_all e where e.emp_no = fh.rd))
                        where fh.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type) ");
                    param = new List<OracleParameter>();
                    param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));


                    SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Assigner", "Update_phase_10");
                }

                //清空sourcer
                if (phase_id.Equals("40"))  
                {
                    sqlText = new StringBuilder();
                    sqlText.Append(@"
                       update RSI_H_FORM_HEADER fh
                       set fh.sourcer = '', fh.sourcer_boss = '', fh.sourcer_manager = ''
                       where fh.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type) ");
                    param = new List<OracleParameter>();
                    param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                    SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Assigner", "Update_phase_40");
                }

                ////撤回者為sourcer,故update sourcer為emp_no,sourcer_boss/sourcer_manager為product sourcer主管不須異動
                //if (phase_id.Equals("50"))
                //{
                //    sqlText = new StringBuilder();
                //    sqlText.Append(@"
                //       update RSI_H_FORM_HEADER fh
                //       set fh.sourcer = :emp_no
                //       where fh.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type) ");
                //    param = new List<OracleParameter>();
                //    param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                //    param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                //    param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                //    SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Assigner", "Update_phase_50");
                //}

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_FORM_APPROVE fa
                    set fa.app_assigner= (select case fa.phase_id when '10' then fh.rd when '20' then fh.rd_boss  when '25' then fh.rd_manager when '30' then fh.pm when '50' then fh.sourcer_boss when '60' then fh.sourcer_manager when '70' then fh.pm else null end from RSI_H_FORM_HEADER fh where fh.form_no = fa.form_no)
                    where fa.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                    and fa.app_status = 'U' ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Assigner", "Update_Assigner方法");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update_Price_Group(string price_group, string rsi_no, string form_no)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                var param = new List<OracleParameter>();

                sqlText = new StringBuilder();
                sqlText.Append(@"
                    UPDATE GPO.RSI_H_FORM_HEADER SET PRICE_GROUP = :price_group WHERE RSI_NO = :rsi_no AND FORM_NO = :form_no ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("price_group", OracleDbType.Varchar2, price_group, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_no", OracleDbType.Varchar2, form_no, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve", "Update_Price_Group方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static void Return_Sourcer(string rsi_no, string part_type, string phase_id, string comment, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                    update RSI_H_PRODUCT_DETAIL pd
                    set pd.phase_id = :phase_id --從querystring的phase_id取得
                    where pd.rsi_no = :rsi_no and pd.part_type= :part_type  --從querystring的rsi_no、part_type取得 ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Return_Sourcer", "Update方法");

                //log檔無須串RSI_H_AUTHORITY,因為可能是RD按下撤回,但RD無設定材料權限
                sqlText = new StringBuilder();
                sqlText.Append(@"
                  insert into RSI_H_DETAIL_APP_LOG(rsi_no, sn, form_no, phase_id, app_serial, app_actor, app_status, app_content, created_by, created_date, begin_date, end_date)
                  select distinct pd.rsi_no, pd.sn, fh.form_no, :phase_id, fa.app_serial, :emp_no, 'C', :p_comment, :emp_no, sysdate, fa.begin_date, sysdate   --加入distinct是考量40&45同時簽核(fa.app_status='U')的狀況
                  from RSI_H_PRODUCT_DETAIL pd
                  join RSI_H_FORM_HEADER fh
                  on pd.rsi_no = fh.rsi_no and pd.part_type = fh.part_type
                  join RSI_H_FORM_APPROVE fa
                  on fa.form_no = fh.form_no
                  where pd.rsi_no = :rsi_no  --從querystring的rsi_no取得
                  and pd.part_type=:part_type 
                  and nvl(pd.modify_type,'-')<>'D' 
                  and fa.app_status = 'U'
                ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Return_Sourcer", "Insert方法");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ReassignForRDDefineAuth(string bu, string reassign, string mtl_part, string rsi_no, string part_type, string phase_id, string sn, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"INSERT INTO GPO.RSI_H_AUTHORITY
                                 (BU, MTL_PARTS, EMP_NO, START_DATE, END_DATE, ""TYPE"", ACTIVE, CREATED_BY, CREATED_DATE, UPDATED_BY, UPDATED_DATE, RSI_NO, SN) 
                                 VALUES
                                 (:bu, :mtl_part, :reassign, SYSDATE, TO_DATE('2049/12/31', 'yyyy/mm/dd'), 'ASSIGN', 'Y', :created_by, SYSDATE, :updated_by, SYSDATE, :rsi_no, :sn) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.Varchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("mtl_part", OracleDbType.Varchar2, mtl_part, ParameterDirection.Input));
                param.Add(new OracleParameter("reassign", OracleDbType.Varchar2, reassign, ParameterDirection.Input));
                param.Add(new OracleParameter("created_by", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("updated_by", OracleDbType.Varchar2, Employee.EmpNO, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Decimal, sn, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignForRDDefineAuth", "Setp 2");

                sqlText.Clear();
                param.Clear();

                sqlText.Append(@"update rsi_h_authority a
                                 set a.active = 'N', a.end_date = sysdate, a.updated_by = :emp_no, a.updated_date = sysdate
                                 where a.bu = :bu
                                 and a.mtl_parts = 'RD DEFINE'
                                 and a.emp_no = :emp_no
                                 and a.rsi_no = :rsi_no
                                 and a.sn = :sn ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.Varchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Decimal, sn, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignForRDDefineAuth", "Setp 3");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ReassignApprove(string bu, string reassign, string mtl_part, string rsi_no, string part_type, string phase_id, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"insert into gpo.rsi_h_form_approve(form_no, phase_id, app_serial, app_assigner, app_actor, app_status, begin_Date, end_date, app_content)
                                             select fa.form_no, fa.phase_id, fa.app_serial, :reassign, null, 'U', sysdate, null, null
                                             from gpo.rsi_h_form_approve fa
                                             where exists (SELECT * 
                                             FROM (SELECT a.*, ROW_NUMBER() OVER (PARTITION BY app_assigner ORDER BY begin_date desc) as sort
                                                   FROM RSI_H_FORM_APPROVE a
                                                   where a.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                                                   and a.phase_id = :phase_id and a.app_assigner = :emp_no) b
                                             WHERE b.sort = 1 and fa.rowid = b.rowid)
                                             and not exists (select 1 from rsi_h_form_approve a 
                                             where a.form_no = fa.form_no and a.app_status = 'U' and a.app_assigner = :reassign) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("reassign", OracleDbType.Varchar2, reassign, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignApprove", "Setp 1");

                sqlText.Clear();
                param.Clear();

                sqlText.Append(@"update gpo.rsi_h_form_approve fa
                                 set fa.app_status  = 'A',
                                     fa.app_actor   = :emp_no,
                                     fa.end_date    = sysdate
                                 where fa.form_no in (select form_no from gpo.rsi_h_form_header where rsi_no = :rsi_no and part_type = :part_type)
                                 and fa.phase_id ='40'
                                 and fa.app_status = 'U'
                                 and fa.app_assigner = :emp_no
                                 and exists (select 1 from RSI_DASHBOARD_LAYER1_SOURCER_V v 
                                 WHERE v.form_no = fa.form_no
                                 and v.emp_no = :emp_no
                                 and v.owner_status = 'Y' )");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignApprove", "Setp 2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ReassignProductSourcerMember(string reassign, string rsi_no, string part_type, string phase_id, string bu, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update gpo.rsi_h_form_header h 
                                                set h.product_sourcer = :reassign  --所選人員
                                                where h.rsi_no = :rsi_no             --參數
                                                and h.part_type = :part_type       --參數
                                                and h.form_status = 'UA' ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("reassign", OracleDbType.Varchar2, reassign, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignProductSourcerMember", "Setp 1");

                sqlText.Clear();
                param.Clear();

                sqlText.Append(@"insert into gpo.rsi_h_form_approve(form_no, phase_id, app_serial, app_assigner, app_actor, app_status, begin_Date, end_date, app_content)
                                             select fa.form_no, fa.phase_id, fa.app_serial, :reassign, null, 'U', sysdate, null, null
                                             from gpo.rsi_h_form_approve fa
                                             where fa.form_no in (select form_no from rsi_h_form_header where rsi_no = :rsi_no and part_type = :part_type)
                                             and fa.phase_id = :phase_id
                                             and fa.app_status = 'U'
                                             and fa.app_assigner = :emp_no ");
                param = new List<OracleParameter>();
                param.Add(new OracleParameter("reassign", OracleDbType.Varchar2, reassign, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignProductSourcerMember", "Setp 2");

                sqlText.Clear();
                param.Clear();

                sqlText.Append(@"update gpo.rsi_h_form_approve fa
                                                set fa.app_actor = :emp_no, fa.app_status = 'T', fa.end_date = sysdate, fa.app_content = 'Transferred from '||:emp_no||' to '||:reassign
                                                where fa.form_no in (
                                                    select form_no
                                                    from gpo.rsi_h_form_header
                                                    where rsi_no = :rsi_no     
                                                    and part_type = :part_type 
                                                )
                                                and fa.phase_id = :phase_id        
                                                and fa.app_status = 'U'
                                                and fa.app_assigner = :emp_no ");
                param = new List<OracleParameter>();
                param.Add(new OracleParameter("reassign", OracleDbType.Varchar2, reassign, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignProductSourcerMember", "Setp 3");

                sqlText.Clear();
                param.Clear();

                sqlText.Append(@"INSERT INTO GPO.RSI_H_PS_AUTHORITY(BU, PART_TYPE, EMP_NO, START_DATE, END_DATE, TYPE, ACTIVE, CREATED_BY, CREATED_DATE, UPDATED_BY, UPDATED_DATE, PARAMETER)
                                    select distinct a.bu, a.part_type, :reassign, sysdate, TO_DATE('2049/12/31', 'yyyy/mm/dd'), 'ASSIGN', 'Y', :emp_no, sysdate, :emp_no, sysdate, a.parameter
                                    from GPO.RSI_H_PS_AUTHORITY a
                                    where a.emp_no = :emp_no
                                    and a.bu = :bu
                                    and a.part_type = :part_type
                                    and a.active = 'Y' ");
                param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.Varchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("reassign", OracleDbType.Varchar2, reassign, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignProductSourcerMember", "Setp 4");

                sqlText.Clear();
                param.Clear();

                sqlText.Append(@"
                    update rsi_h_ps_authority a
                    set a.active = 'N', a.end_date = sysdate, a.updated_by = :emp_no, a.updated_date = sysdate
                    where a.bu = :bu
                    and a.part_type = :part_type
                    and a.type = 'ASSIGN'
                    and a.active = 'Y'
                    and a.emp_no = :emp_no ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("bu", OracleDbType.Varchar2, bu, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "ReassignProductSourcerMember", "Setp 5");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_IsApproved_Y(string rsi_no, string emp_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL a
                set a.isapproved = 'Y',
                    a.sourcer_owner = nvl(a.sourcer_owner, :emp_no)
                where exists (
                SELECT d.*
                FROM GPO.RSI_H_PRODUCT_DETAIL D
                JOIN GPO.RSI_H_AUTHORITY H
                   ON(D.BU = H.BU AND D.MTL_PARTS = H.MTL_PARTS)
                JOIN GPO.C_PMS_BGBU_MAPPING M
                   ON D.BU = M.BU             
                WHERE D.RSI_NO = :rsi_no --'2018000001'
                  AND(D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                  AND H.EMP_NO = :emp_no --'1108633'
                  AND H.ACTIVE = 'Y'
                  AND H.START_DATE <= SYSDATE
                  AND H.END_DATE >= SYSDATE
                  AND(D.PHASE_ID IS NULL OR D.PHASE_ID = '40')
                  AND D.PART_TYPE = :part_type --卡控Part_Type
                  AND NVL(D.MTL_GROUP, '-') <> 'MOH'
                  AND NVL(D.ISASSIGNER, '-') = 'Y' --已認領的料件
                  AND D.rsi_No = a.rsi_no
                  AND D.sn = a.sn
                UNION--若新增Material Group / Material Parts預設人員
                SELECT d.*
                FROM GPO.RSI_H_PRODUCT_DETAIL D
                JOIN GPO.RSI_H_AUTHORITY H
                   ON(D.BU = H.BU AND D.PART_TYPE = H.MTL_PARTS)
                  AND H.TYPE = 'DEFAULT'
                JOIN GPO.C_PMS_BGBU_MAPPING M
                   ON D.BU = M.BU
                WHERE D.RSI_NO = :rsi_no
                  AND(D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                  AND H.EMP_NO = :emp_no --'1108633'
                  AND H.ACTIVE = 'Y'
                  AND H.START_DATE <= SYSDATE
                  AND H.END_DATE >= SYSDATE
                  AND D.mtl_group = 'RD DEFINE'
                  AND(D.PHASE_ID IS NULL OR D.PHASE_ID = '40')
                  AND D.PART_TYPE = :part_type --卡控Part_Type
                  AND NVL(D.MTL_GROUP, '-') <> 'MOH'
                  AND NVL(D.ISASSIGNER, '-') = 'Y'--已認領的料件
                  AND D.rsi_No = a.rsi_no
                  AND D.sn = a.sn)
                  AND NVL(a.MODIFY_TYPE,'-')<>'D'
                ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_IsApproved_Y", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_IsApproved_Y_MOH(string rsi_no, string emp_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL d
                                 set d.isapproved = 'Y', d.sourcer_owner = nvl(d.sourcer_owner, :emp_no)
                                 where D.RSI_NO = :rsi_no
                                 AND D.PART_TYPE = :part_type
                                 AND (D.MODIFY_TYPE IS NULL OR D.MODIFY_TYPE <> 'D')
                                 AND (D.PHASE_ID IS NULL OR D.PHASE_ID = '40')
                                 AND D.MTL_GROUP = 'MOH' --Product Sourcer需維護MOH費用
                                 AND NVL(D.ISASSIGNER, '-') = 'Y' --已認領的料件
                                 AND exists (SELECT 1
                                        FROM GPO.RSI_H_PS_AUTHORITY H
                                       WHERE H.EMP_NO = :emp_no
                                         AND H.ACTIVE = 'Y'
                                         AND H.START_DATE <= SYSDATE
                                         AND H.END_DATE >= SYSDATE
                                         AND D.BU = H.BU
                                         AND D.PART_TYPE = H.Part_Type) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_IsApproved_Y_MOH", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_IsAssigner_IsApproved_N(string rsi_no, string part_type, string sn, string reassign)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL d
                                set d.isassigner = null, d.isapproved = null, d.iscalculate = null, d.sourcer_owner = :sourcer_owner
                                where d.RSI_NO = :rsi_no
                                AND d.PART_TYPE = :part_type
                                AND nvl(d.MODIFY_TYPE,'-') <> 'D'
                                AND d.sn = :sn ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("sourcer_owner", OracleDbType.Varchar2, reassign, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("sn", OracleDbType.Decimal, sn, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_IsAssigner_IsApproved_N", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_IsApproved_N_byPartType(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL d
                                set d.isapproved = null, d.isassigner = null, d.iscalculate = null, d.sourcer_owner = null
                                where d.RSI_NO = :rsi_no
                                AND d.PART_TYPE = :part_type
                                AND nvl(d.MODIFY_TYPE,'-') <> 'D' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_IsApproved_N_byparttype", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_Phaseid_byPartType(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_PRODUCT_DETAIL d
                                set d.phase_id = null
                                where d.RSI_NO = :rsi_no
                                AND d.PART_TYPE = :part_type
                                AND (d.MODIFY_TYPE IS NULL OR d.MODIFY_TYPE <> 'D') ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Phaseid_byPartType", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_Sourcer_Assigner(string rsi_no, string part_type)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                var param = new List<OracleParameter>();
                sqlText = new StringBuilder();
                sqlText.Append(@"INSERT INTO RSI_H_FORM_APPROVE
                                 select distinct a.form_no, a.phase_id, a.app_serial, r.sourcer as app_assigner, null as app_actor, a.app_status, a.begin_date, null as end_date, null as app_content
                                  from RSI_H_FORM_APPROVE a
                                  join RSI_ASSIGN_SOURCER_V r
                                    on a.form_no = r.FORM_NO
                                 where a.phase_id = 40
                                   and a.app_status = 'U'
                                   and r.rsi_no = :rsi_no and r.part_type = :part_type ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Sourcer_Assigner", "Update_Sourcer_Assigner方法Step1");

                sqlText.Clear();
                param.Clear();

                sqlText.Append(@"delete from RSI_H_FORM_APPROVE
                                 where form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                                 and phase_id=40
                                 and app_status='U'
                                 and app_assigner is null ");

                param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Sourcer_Assigner", "Update_Sourcer_Assigner方法Step2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Approve_Sourcer(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_FORM_APPROVE a
                                 set a.app_status = 'A', a.end_date = sysdate, 
                                 a.app_actor = :emp_no, a.app_content = :p_comment
                                 where exists (SELECT * FROM 
                                      (SELECT a.*, ROW_NUMBER() OVER(PARTITION BY app_assigner ORDER BY begin_date desc) as sort
                                       FROM RSI_H_FORM_APPROVE a
                                       where a.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                                       and a.phase_id = :phase_id and a.app_assigner = :emp_no) b
                                WHERE b.sort = 1 and a.rowid = b.rowid) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                param.Add(new OracleParameter("p_comment", OracleDbType.Varchar2, comment, ParameterDirection.Input));
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Approve_Sourcer", "Update方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static bool Update_Approve_byPass(string rsi_no, string part_type, string phase_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_FORM_APPROVE a
                                 set a.app_status = 'P', a.end_date= sysdate, a.app_actor= 'SYSTEM', a.app_content= 'Auto Complete by System'
                                 where a.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                                 and a.phase_id = :phase_id
                                 --and (a.phase_id = :phase_id or a.phase_id = '45')
                                 and a.app_status = 'U' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Approve_byPass", "Update方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        public static bool Update_Approve_byPass_forRej(string rsi_no, string part_type, string phase_id)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update RSI_H_FORM_APPROVE a
                                 set a.app_status = 'P', a.end_date= sysdate, a.app_actor= 'SYSTEM', a.app_content= 'Auto Complete by System'
                                 where a.form_no = (select form_no from RSI_H_FORM_HEADER where rsi_no = :rsi_no and part_type = :part_type)
                                 and a.phase_id in ('40','45')
                                 and a.app_status = 'U' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.Varchar2, phase_id, ParameterDirection.Input));

                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_Approve_byPass_forRej", "Update方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        //public static bool Check_Reassign_Status(string rsi_no, string part_type, string emp_no)
        //{
        //    Boolean flag = false;

        //    try
        //    {
        //        StringBuilder sqlText = new StringBuilder();

        //        sqlText.Append(@"select count(*) as count 
        //                         from rsi_to_do_list_v v 
        //                         join RSI_H_FORM_HEADER h
        //                         on v.RSI_NO = h.rsi_no and v.FORM_NO = h.form_no
        //                         WHERE v.rsi_no = :rsi_no
        //                         and v.PART_TYPE = :part_type
        //                         and v.app_assigner = :emp_no ");

        //        var param = new List<OracleParameter>();
        //        param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
        //        param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
        //        param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));

        //        DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "Check_Reassign_Status", "判斷材料是否已全數轉簽");

        //        var results = dt.AsEnumerable().Select(p => p.Field<decimal>("count")).ToList();
        //        int result = Convert.ToInt32(results.FirstOrDefault());

        //        if (result == 0)
        //            flag = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //LogHelper.GetLogger("GetWIPInfo").Error(ex);
        //        return flag;
        //    }

        //    return flag;
        //}

        public static bool RDCheckUpdate(string rsi_no, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"select * from rsi_h_product_detail
                                 where rsi_no = :rsi_no
                                 and (created_by = :emp_no or updated_by = :emp_no)
                                 and nvl(modify_type, '-') <> 'D' ");
                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));
                DataTable dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "H_Form_ApproveEntityDAL", "RDCheckUpdate");

                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update_mtl_group_forSpecial(string rsi_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"update rsi_h_product_detail a
                                 set (a.mtl_group,a.parts_group,a.part_type)=(select distinct mtl_group, parts_group, part_type from rsi_h_product_detail b where b.sn = a.parent_sn
                                 and b.rsi_no = a.rsi_no and nvl(b.modify_type,'-')<>'D')
                                 where a.rsi_no = :rsi_no
                                 and a.mtl_group='FOLLOW60'
                                 and nvl(a.modify_type,'-')<>'D' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input)); 
                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Update_mtl_group_forSpecial", String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert_b2b_status(string rsi_no, string part_type)
        {
            bool result = false;
            try
            {
                StringBuilder sqlText = new StringBuilder();

                sqlText.Append(@"insert into RSI_H_B2B_STATUS(RSI_NO, B2B_STATUS, B2B_PURPOSE, B2B_TYPE, CREATE_TIME)
                                 select i.rsi_no, 'WAIT', 'PARTIAL', h.part_type, sysdate
                                 from rsi_h_product_info i
                                 join rsi_h_form_header h 
                                 on h.rsi_no = i.rsi_no
                                 where i.rsi_no = :rsi_no
                                 and h.is_adp = 'Y' ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Varchar2, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.Varchar2, part_type, ParameterDirection.Input));
                result = SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "Insert_b2b_status", "Insert方法");
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }
    }

    #endregion
}