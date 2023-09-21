using Com.Auo.HttpModule20.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RSI.Models.Entity
{
    public static class Validate
    {
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str">解密前文字</param>
        /// <returns>解密後文字</returns>
        public static string DecryptValue(string str)
        {
            if (String.IsNullOrEmpty(str))
                return String.Empty;
            str = str.Substring(12, str.Length - 12);
            return SecurityHelper.Decrypt(str, false);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str">加密前文字</param>
        /// <returns>加密後文字</returns>
        public static string EncryptValue(string str)
        {
            return SecurityHelper.Encrypt(str, false);
        }
    }
}