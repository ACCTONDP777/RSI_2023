using Newtonsoft.Json;
using RSI.Models.Entity;
using RSI.Models.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace RSI.Service
{
    /// <summary>
    ///RSIWebService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class RSIWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public bool CreateRSIProcess(string str)
        {
            try
            {
                //單筆資料
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                RSIWebServiceEntity model = json_serializer.Deserialize<RSIWebServiceEntity>(str);

                if (model.RdEvaluationFinish.Equals("N") && RSIWebServiceManager.Check_Product_Info_Status(model))     //MS RFQ技術評估分批拋送至RSI，若平行簽核之前已經啟動過Part Type，只要update狀態即可
                {
                    RSIWebServiceManager.InsertLog("WebService", "CreateRSIProcess", "UPD:"+model.RFQ_NO.Trim()+"/"+ model.RFQ_VER.Trim(), str);
                    //RSI_Status = ContinueRSI
                    RSIWebServiceManager.Update_Product_Info_Status(model);
                }
                else
                {
                    RSIWebServiceManager.InsertLog("WebService", "CreateRSIProcess", "INS:" + model.RFQ_NO.Trim() + "/" + model.RFQ_VER.Trim(), str);
                    if (!RSIWebServiceManager.Check_Product_Info_Exists(model))  //避免RFQ重複拋轉
                        RSIWebServiceManager.InsertH_Product_Info(model);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [WebMethod] 
        public bool CancelRSIProcess(string str)
        {
            try
            {
                //單筆資料
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                RSIWebServiceEntity model = json_serializer.Deserialize<RSIWebServiceEntity>(str);
                RSIWebServiceManager.InsertLog("WebService", "CancelRSIProcess", model.RFQ_NO.Trim() + "/" + model.RFQ_VER.Trim(), str);
                string header_flag = model.ACTION.Equals("TERMINAL") ? "DE" : "RC";
                string approve_flag = model.ACTION.Equals("TERMINAL") ? "D" : "C";
                RSIWebServiceManager.Cancel_Product_Info(model, header_flag, approve_flag);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [WebMethod]
        public bool ReassignRDMember(string str)
        {
            try
            {
                //單筆資料
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                RSIWebServiceEntity model = json_serializer.Deserialize<RSIWebServiceEntity>(str);
                RSIWebServiceManager.Reassign_RD_Member(model);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCurrentHandler(string bu, string project_name)
        {
            var result = RSIWebServiceManager.GetCurrentHandler(bu, project_name);
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}
