using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RSI.Models.Utility
{
    public static class XMLHelper
    {
        public static DataTable GetDataSetFromXML(string returnXML)
        {
            if (returnXML.StartsWith("<Error>"))
            {
                throw new Exception(returnXML);
            }
            DataSet ds = new DataSet();
            using (System.IO.StringReader sr = new System.IO.StringReader(returnXML))
            {
                ds.ReadXml(sr);
            }
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return new DataTable();
            }
        }

    }
}