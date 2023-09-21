using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Auo.EAP;

namespace RSI.Models.Utility
{
    /*
         * EAP 使用說明：http://itdn/WebForms/TreeMenu.aspx?contentItemId=134d252e-be3a-4573-9bb0-30543efdbb36 
         * EAP Client dll ( Com.Auo.EAP.dll ) 提供 2種方式使用 EAP:
         * 1 EAP PluginLite (EAP Module)，以Plugin的方式實現類似 Asp.net forms驗證 的功能，不需 hard code，在 Web.config 和 EAP Platform 中設定即可實現對 Page 訪問的權限驗證
         * 2 EAPClient，提供 Method ，通過調用 Method 返回驗證結果
         * 這些都可以在以上 《EAP 使用說明》 中看到相關說明資料。
         * 
         * EAPClient.Check()方法有 3個重載：
         * 1 public static bool Check(string authFunction, string empNo, string domain)
         * 2 public static bool Check(Page page, string authFunction)
         * 3 public static bool Check(string company, string bu, string system, string systemPassword, string authFunction, string empNo, string domain)
         * 
         * 【 通常情況下請使用 重載1 】，重載1 可以從 Web.config 的 <EAP /> 配置節 中讀取 company, bu, system, systemPassword
         *  
         * 重載2 在方法內取得當前登錄人的工號跟域，然後調用重載1進行驗證權限
         *  
         * 重載3 需要傳入 company, bu, system, systemPassword 參數，通常用於對多個 system 進行權限驗證 的情況，如 Portal Menu，
         * 在 Portal Menu 中，system 根據 Menu配置表（Platform_Menu_C）中配置的 auth_system 傳入（可以為每個Menu Item 配置對應的 auth_system, auth_function），
         * 而 system 對應的 systemPassword 則從 <appSettings /> 配置節中讀取（如下示例，見方法 public static bool Check(string system, string authFunction, string empNo, string domain)）
         * 這部分詳細內容請參考 Diamond 和 PortalMenu 說明。       
         */
    public class EAP
    {
        public static bool Check(string authFunction, string empNo, string domain)
        {
            return EAPClient.Check(authFunction, empNo, domain);  // 從 Web.config <EAP /> 配置節 中讀取 company, bu, system, systemPassword
        }

        public static void Logout()
        {
            EAPClient.Logout();  // 清空緩存在 Cache 中的權限資料，如 FunctionList
        }
    }
}
