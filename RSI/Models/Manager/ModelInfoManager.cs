using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class ModelInfoManager
    {
        public static IList<ModelInfoEntity> GetModelInfo(string part_no)
        {
            return ModelInfoDAL.GetModelInfo(part_no);
        }

        public static DataTable GetPartNo(string query)
        {
            return ModelInfoDAL.GetPartNo(query);
        }

        public static DataTable GetQueryConditions(string query)
        {
            return ModelInfoDAL.GetQueryConditions(query);
        }

        public static DataTable GetPartNoForUploadExcel(string query)
        {
            return ModelInfoDAL.GetPartNoForUploadExcel(query);
        }

        public static DataTable GetPartKindKeyForUploadExcel(string query)
        {
            return ModelInfoDAL.GetPartKindKeyForUploadExcel(query);
        }

        public static DataTable GetPartLevelEnglishForUploadExcel(string part_level, string english_name)
        {
            return ModelInfoDAL.GetPartLevelEnglishForUploadExcel(part_level, english_name);
        }

        public static DataTable GetPartTypeForUploadExcel()
        {
            return ModelInfoDAL.GetPartTypeForUploadExcel();
        }

        public static DataTable GetEnglishForUploadExcel(string rsi_no, string parent_level, string part_level)
        {
            return ModelInfoDAL.GetEnglishForUploadExcel(rsi_no, parent_level, part_level);
        }
    }
}