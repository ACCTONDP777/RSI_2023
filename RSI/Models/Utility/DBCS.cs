using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSI.Models.Utility
{
    public class DBCS
    {
        public static ConnectionInfo Default = new ConnectionInfo();

        /// <summary>
        /// 預設DB連線Connection String
        /// </summary>
        public class ConnectionInfo
        {
            public string ConnectionString = string.Empty;
            public string Provider = string.Empty;
        }
    }
}
