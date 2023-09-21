using System;
using System.IO;
using System.Net;
using System.Text;

namespace RSI.Models.Utility
{
    public class WebApiHelper
    {
        #region 公共参数
        static string webApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebApiUrl"];
        #endregion

        #region PostMethod
        public static string PostMethod(string postDataStr, string url)
        {
            url = webApiUrl + url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, System.Text.Encoding.UTF8);
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        #endregion

        #region GetMethod
        public static string GetMethod(string url)
        {
            url = webApiUrl + url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "text/html;charset=UTF-8"; 
            request.ContentType = "application/json";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        #endregion

        #region 异步调用Web API
        static byte[] postData = null;
        public void AsyncPostMethod(string postDataStr, string url)
        {
            url = webApiUrl + url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.KeepAlive = true;
            request.Timeout = 300000;
            request.ContentType = "application/json";
            postData = Encoding.UTF8.GetBytes(postDataStr);
            request.BeginGetRequestStream(new AsyncCallback(RequestStreamCallBack), request);
        }

        public static void RequestStreamCallBack(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            Stream reqStream = request.EndGetRequestStream(result);
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            request.BeginGetResponse(new AsyncCallback(ResponseCallBack), request);
        }

        public static void ResponseCallBack(IAsyncResult result)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)result.AsyncState;
                HttpWebResponse response = (HttpWebResponse)req.EndGetResponse(result);

                using (Stream sw = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(sw))
                    {
                        string xmls = reader.ReadToEnd();
                    }
                }
                if (response != null) response.Close();
            }
            catch (Exception ex)
            {
                LogHelper.GetLogger("WebApiHelper").Error(ex.Message, ex);
            }
        }
        #endregion



    }
}
