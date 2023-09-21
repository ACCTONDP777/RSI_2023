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
    public class FileManagementEntity
    {
        public string rsi_no { get; set; }
        public string sn { get; set; }
        public int phase_id { get; set; }
        public string remark { get; set; }
        public HttpPostedFileBase file { get; set; }
    }

    public class FileListEntity
    {
        public string SYS_ID { get; set; }
        public string BIZ_ID { get; set; }
        public string FILE_ID { get; set; }
        public string FILE_NAME { get; set; }
        public decimal FILE_SIZE { get; set; }
        public string REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
    #endregion

    #region DAL
    public static class FileManagementDAL
    {
        public static IList<FileListEntity> GetFilelist(string biz_id)
        {
            IList<FileListEntity> fileList = new List<FileListEntity>();
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"Select * from RSI_H_FILELIST where biz_id = :biz_id order by created_date desc ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("biz_id", OracleDbType.NVarchar2, biz_id, ParameterDirection.Input));

                fileList = SqlExcute.GetOraObjList<FileListEntity>(sqlText, param.ToArray(), "GetFilelist", "GetFilelist方法");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
            return fileList;
        }

        public static void SetFilelist(string sys_id, string biz_id, string file_id, string file_name, string file_size, string remark, string emp_no)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"
                   insert into RSI_H_FILELIST (SYS_ID, BIZ_ID, FILE_ID, FILE_NAME, FILE_SIZE, REMARK, CREATED_BY, CREATED_DATE) 
                   values (:sys_id, :biz_id, :file_id, :file_name, :file_size, :remark, :emp_no, SYSDATE)");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("sys_id", OracleDbType.Varchar2, sys_id, ParameterDirection.Input));
                param.Add(new OracleParameter("biz_id", OracleDbType.Varchar2, biz_id, ParameterDirection.Input));
                param.Add(new OracleParameter("file_id", OracleDbType.Varchar2, file_id, ParameterDirection.Input));
                param.Add(new OracleParameter("file_name", OracleDbType.Varchar2, file_name, ParameterDirection.Input));
                param.Add(new OracleParameter("file_size", OracleDbType.Decimal, file_size, ParameterDirection.Input));
                param.Add(new OracleParameter("remark", OracleDbType.Varchar2, remark, ParameterDirection.Input));
                param.Add(new OracleParameter("emp_no", OracleDbType.Varchar2, emp_no, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "SetFilelist", "SetFilelist方法");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
        }

        public static void UpdateFileActive(string file_id)
        {
            try
            {
                StringBuilder sqlText = new StringBuilder();
                sqlText.Append(@"delete from RSI_H_FILELIST where file_id = :file_id ");

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter("file_id", OracleDbType.Varchar2, file_id, ParameterDirection.Input));

                SqlExcute.ExcuteOraSql(sqlText, param.ToArray(), "UpdateFileActive", "UpdateFileActive方法");
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger("GetWIPInfo").Error(ex);
                throw ex;
            }
        }
    }
    #endregion
}