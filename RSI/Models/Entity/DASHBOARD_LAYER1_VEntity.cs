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
    public class DASHBOARD_LAYER1_VEntity
    {
        public decimal RSI_NO { get; set; }
        public decimal FORM_NO { get; set; }
        public string PART_TYPE { get; set; }
        public string PROJECT_NAME { get; set; }
        public string REVIEWER { get; set; }
        public DateTime BEGIN_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public string STATUS { get; set; }
        public decimal HELDDAYS { get; set; }
        public string PHASE_ID { get; set; }
        public string APP_ASSIGNER { get; set; }
        public string APP_ACTOR { get; set; }
        public string FORM_STATUS { get; set; }
        public string NOW_PHASE { get; set; }
        public string RETURN_PHASE_ID { get; set; }
        public string CHECK_RETURN { get; set; }
        public string PRODUCTSOURCER { get; set;  }
        public string MTL_PARTS { get; set; }
        public string APPROVED { get; set; }
    }
    #endregion

    #region DAL
    public class DASHBOARD_LAYER1_VEntityDAL
    {
        public static IList<DASHBOARD_LAYER1_VEntity> GetDashBoardLayer1(string rsi_no, string phase_id, string form_status, string part_type)
        {
            IList<DASHBOARD_LAYER1_VEntity> dashboardLayer1s = new List<DASHBOARD_LAYER1_VEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM
                                                (
                                                SELECT * FROM RSI_DASHBOARD_LAYER1_V 
                                                 WHERE RSI_NO = :rsi_no
                                                   AND STATUS = :form_status 
                                                   AND PHASE_ID = :phase_id
                                                   AND PART_TYPE = :part_type ");

                if (phase_id.Equals("40"))
                {
                    sqlText.Append(@"               UNION
                                                    SELECT * FROM RSI_DASHBOARD_LAYER1_V 
                                                     WHERE RSI_NO = :rsi_no
                                                       AND STATUS = :form_status
                                                       AND PHASE_ID = '45'     --固定不用傳參數
                                                       AND NOW_PHASE = :phase_id
                                                       AND (APP_ASSIGNER = :emp_no OR PRODUCTSOURCER = :emp_no) --:emp_no
                                                       AND PART_TYPE = :part_type ");
                }                
                sqlText.Append(@"                ) T
                                                ORDER BY T.RSI_NO, T.PHASE_ID DESC, DECODE(T.PART_TYPE, 'ACD', 0, 'EE', 1, 'OM', 2, 'PACKING', 3, 4) ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_status", OracleDbType.NVarchar2, form_status, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));     
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));

                dashboardLayer1s = SqlExcute.GetOraObjList<DASHBOARD_LAYER1_VEntity>(sqlText, param.ToArray(), "GetDashBoardLayer1", "獲取DashBoardLayer1資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dashboardLayer1s;
            }
            return dashboardLayer1s;
        }

        public static IList<DASHBOARD_LAYER1_VEntity> GetDashBoardLater1Sourcer(string rsi_no, string phase_id, string form_status, string part_type)
        {
            IList<DASHBOARD_LAYER1_VEntity> dashboardLayer1s = new List<DASHBOARD_LAYER1_VEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"SELECT * FROM
                                                (
                                                SELECT rsi_no, form_no, part_type, mtl_parts, reviewer, owner_status as approved, begin_date, end_date, 
                                                       status, form_status, helddays, phase_id, app_assigner, now_phase, return_phase_id, 
                                                       check_return, sourcer_reject, productsourcer 
                                                  FROM RSI_DASHBOARD_LAYER1_SOURCER_V 
                                                 WHERE RSI_NO = :rsi_no
                                                   --AND FORM_STATUS = :form_status 
                                                   AND PHASE_ID = :phase_id
                                                   AND PART_TYPE = :part_type
                                                   AND nvl(mtl_parts,'-')<>'-'
                                                UNION  --串Product Sourcer簽核狀況
                                                    SELECT rsi_no, form_no, part_type, '-', reviewer, DECODE(status, 'A', 'Y', 'N') AS approved, begin_date, end_date, 
                                                           status, form_status, helddays, phase_id, app_assigner, now_phase, return_phase_id, 
                                                           check_return, sourcer_reject, productsourcer  
                                                      FROM RSI_DASHBOARD_LAYER1_V 
                                                     WHERE RSI_NO = :rsi_no
                                                       --AND STATUS = :form_status  --多人同時簽核,簽核與否都要顯示
                                                       AND PHASE_ID = '45'     --固定不用傳參數
                                                       --AND NOW_PHASE = :phase_id
                                                       --AND (APP_ASSIGNER = :emp_no OR PRODUCTSOURCER = :emp_no) --:emp_no  --只有Product Sourcer才可以看到Product Sourcer簽核資訊
                                                       AND PART_TYPE = :part_type
                                             ) T
                                                ORDER BY T.STATUS DESC, T.MTL_PARTS ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                param.Add(new OracleParameter("form_status", OracleDbType.NVarchar2, form_status, ParameterDirection.Input));
                param.Add(new OracleParameter("phase_id", OracleDbType.NVarchar2, phase_id, ParameterDirection.Input));
                param.Add(new OracleParameter("part_type", OracleDbType.NVarchar2, part_type, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));

                dashboardLayer1s = SqlExcute.GetOraObjList<DASHBOARD_LAYER1_VEntity>(sqlText, param.ToArray(), "GetDashBoardLater1Sourcer", "獲取DashBoardLayer1資訊");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dashboardLayer1s;
            }
            return dashboardLayer1s;
        }

        public static bool GetViewIconShow(List<DASHBOARD_LAYER1_VEntity> model, string phase_id, string rsi_no)
        {
            bool result = true;
            try
            {
                result = model.Where(p => p.APP_ASSIGNER == Employee.EmpNO || p.APP_ACTOR == Employee.EmpNO).Any();

                if (result)
                    return result;

                DataTable dt = GetViewDataTable(rsi_no, phase_id);
                 result = model.Where(p => dt.AsEnumerable().Select(x => x.Field<string>("PART_TYPE")).Contains(p.PART_TYPE)).Any();
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return false;
            }
            return result;
        }

        public static DataTable GetViewDataTable(string rsi_no, string phase_id)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                if (phase_id.Equals("10") || phase_id.Equals("20") || phase_id.Equals("25") || phase_id.Equals("30") || phase_id.Equals("70"))
                {
                    sqlText.Append(@"select distinct trim(a.boss_emp_no) as boss_emp_no, a.boss_level, a.boss_title, c.rsi_no, c.form_no, c.part_type, b.app_assigner, b.app_actor
                                                    from au_dw.auo_person_reportline a
                                                    join RSI_H_FORM_APPROVE b on (a.emp_no = b.app_assigner or a.emp_no = b.app_actor) and (b.phase_id <= '30' or b.phase_id = '70')
                                                    join RSI_H_FORM_HEADER c on b.form_no = c.form_no
                                                    where (trim(a.boss_emp_no) = :emp_no) -- EMP_NO
                                                    -- or nvl(b.app_actor, b.app_assigner) = :emp_no)
                                                    and a.active = 'Y'
                                                    and c.rsi_no = :rsi_no  --RSI_NO -- DashBoard傳入
                                                    and b.phase_id = :phase_id
                                                  order by c.rsi_no, c.form_no, c.part_type ");

                    var param = new List<OracleParameter>();
                    param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                    param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("phase_id", OracleDbType.Decimal, phase_id, ParameterDirection.Input));

                    dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetViewIconShow", "phase_id=10、20、25、30、70");
                }
                else
                {
                    sqlText.Append(@"select distinct trim(a.boss_emp_no) as boss_emp_no, a.boss_level, a.boss_title, h.rsi_no, h.form_no, h.part_type, d.app_assigner, c.app_actor 
                                                    from au_dw.auo_person_reportline a
                                                    join RSI_H_AUTHORITY b on a.emp_no = b.emp_no
                                                    join RSI_H_DETAIL_APP_LOG c on a.emp_no = c.app_actor
                                                    join RSI_H_FORM_HEADER h on c.rsi_no = h.rsi_no and c.form_no = h.form_no 
                                                    join RSI_H_FORM_APPROVE d on c.form_no = d.form_no
                                                    where trim(a.boss_emp_no) = :emp_no    -- EMP_NO
                                                    and a.active = 'Y'
                                                    and b.active = 'Y'
                                                    and sysdate >= b.start_date  
                                                    and b.end_date > =sysdate
                                                    and h.rsi_no = :rsi_no  --RSI_NO -- DashBoard傳入
                                                    and d.phase_id = :phase_id
                                                  order by h.rsi_no, h.form_no, h.part_type ");
                    var param = new List<OracleParameter>();
                    param.Add(new OracleParameter("emp_no", OracleDbType.NVarchar2, Employee.EmpNO, ParameterDirection.Input));
                    param.Add(new OracleParameter("rsi_no", OracleDbType.Decimal, rsi_no, ParameterDirection.Input));
                    param.Add(new OracleParameter("phase_id", OracleDbType.Decimal, phase_id, ParameterDirection.Input));

                    dt = SqlExcute.GetOraDateTable(sqlText, param.ToArray(), "GetViewIconShow", "phase_id=40、50、60");
                }
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                return dt;
            }
            return dt;
        }
    }
    #endregion
}