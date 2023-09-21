using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;

namespace RSI.Models.Utility
{
    public static class ExtendHelper
    {
        /// <summary>
        /// 将DataTable转换为IList<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcDt"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this DataTable srcDt) where T : class, new()
        {
            List<PropertyInfo> perList = new List<PropertyInfo>();
            Type t = typeof(T);
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (srcDt.Columns.IndexOf(p.Name) != -1) perList.Add(p); });

            List<T> resList = new List<T>();
            foreach (DataRow row in srcDt.Rows)
            {
                T ob = new T();
                perList.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                resList.Add(ob);
            }
            return resList;
        }

        /// <summary>
        /// 将DataTable转换为IList<T>,解决table栏位名中包含“_",model属性名中因无“_"导致与栏位名不匹配的解决方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcDt"></param>
        /// <param name="underscore">匹配属性名时，是否去除下划线</param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this DataTable srcDt, bool underscore = false) where T : class, new()
        {
            DataColumn[] cols = new DataColumn[srcDt.Columns.Count];
            srcDt.Columns.CopyTo(cols, 0);
            if (underscore)
                Array.ForEach<DataColumn>(cols, p => { p.ColumnName = p.ColumnName.Replace("_", ""); });

            List<PropertyInfo> perList = new List<PropertyInfo>();
            Type t = typeof(T);
            Array.ForEach<PropertyInfo>(t.GetProperties(), p =>
            {
                foreach (DataColumn col in cols)
                {
                    if (col.ColumnName.ToLower() == p.Name.ToLower())
                    {
                        perList.Add(p);
                        break;
                    }
                }
            });

            List<T> resList = new List<T>();
            foreach (DataRow row in srcDt.Rows)
            {
                T ob = new T();
                perList.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                resList.Add(ob);
            }
            return resList;
        }
    }
}