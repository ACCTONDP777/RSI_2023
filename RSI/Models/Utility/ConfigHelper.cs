using System.Data;
using System.Configuration;
using System.IO;

namespace RSI.Models.Utility
{
    public class ConfigHelper
    {
        //static string CONFIG_FILE_PATH = ConfigurationManager.AppSettings["ConfigFilePath"];
        static string DEFAULT_LANGUAGE = ConfigurationManager.AppSettings["DefaultLanguage"].ToUpper();
        static string MYAUO_URL = ConfigurationManager.AppSettings["myAuoUrl"];

        /// <summary>
        /// 获取用户的语言种类
        /// </summary>
        public static string GetLangId(string userCapLanguage)
        {
            string langId = userCapLanguage.ToUpper(); //语系，优先从CAP中取，无效的话再取web.config中配置的默认语系
            if (!string.IsNullOrWhiteSpace(langId))
            {
                if (langId != "ZH-CN" && langId != "ZH-TW" && langId != "EN-US") //简中zh-CN，繁中zh-TW，英文en-US
                {
                    if (DEFAULT_LANGUAGE == "ZH-CN" || DEFAULT_LANGUAGE == "ZH-TW" || DEFAULT_LANGUAGE == "EN-US")
                    {
                        langId = DEFAULT_LANGUAGE;
                    }
                    else langId = "zh-TW";  //web.config中配置的默认语系不在支援范围内，强制设置为：繁中zh-TW
                }
            }
            return langId;
        }
    }
}
