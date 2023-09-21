using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Configuration;
using RSI.Models.Utility;

namespace RSI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //初始化log4net，加载配置
            log4net.Config.XmlConfigurator.Configure();

            #region 取得DB Connection String
            if (ConfigurationManager.AppSettings["USE_DBCS"].ToString() == "Y") //使用DBCS获取连接字串
            {
                #region 取得從DBCS之Connection String
                using (DBCS_SERVICE.WCF_ConnectionInfoClient oDBCS = new DBCS_SERVICE.WCF_ConnectionInfoClient())
                {
                    DBCS_SERVICE.DBConnectionString oConnectionstring = oDBCS.GetDBConnection(ConfigurationManager.AppSettings["DBCS_Default"].ToString());
                    DBCS.Default.ConnectionString = oConnectionstring.ConnectionString;
                    DBCS.Default.Provider = oConnectionstring.ProviderName;
                    
                    if (string.IsNullOrEmpty(DBCS.Default.ConnectionString))
                    {
                        //若是取得Null，需記錄
                        LogHelper.GetLogger("Global.asax").Error("Get Connection String is null from DBCS");
                    }
                }
                #endregion 取得從DBCS之Connection String
            }
            else
            {   //從web.config中讀取連接字串 【套用时修改】
                DBCS.Default.ConnectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
                DBCS.Default.Provider = ConfigurationManager.ConnectionStrings["default"].ProviderName;
            }
            #endregion 取得DB Connection String
        }
    }
}
