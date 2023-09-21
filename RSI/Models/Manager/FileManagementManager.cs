using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class FileManagementManager
    {
        public static IList<FileListEntity> GetFilelist(string biz_id)
        {
            return FileManagementDAL.GetFilelist(biz_id);
        }
        public static void SetFilelist(string sys_id, string biz_id, string file_id, string file_name, string file_size, string remark, string emp_no)
        {
            FileManagementDAL.SetFilelist(sys_id, biz_id, file_id, file_name, file_size, remark, emp_no);
        }

        public static void UpdateFileActive(string file_id)
        {
            FileManagementDAL.UpdateFileActive(file_id);
        }
    }
}