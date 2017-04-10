using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Web.Services.Protocols;
using System.Web.Services;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;

using System.Configuration;
using System.Collections.Specialized;
using System.Runtime.InteropServices.ComTypes;
using System.Web.UI.WebControls;
using static TuleapHelper.RequestSender;
using CheckBox = System.Windows.Forms.CheckBox;
using TuleapHelper;

namespace TuleapHelper
{
    class HttpLevel
    {
        public static byte callCounter = 0;

        public static string HttpGet(string url, bool useToken=true)
        {
            HttpWebRequest req = WebRequest.Create(CommonVariables.tuleapHost + url) as HttpWebRequest;

            if (useToken)
            {
                req.Headers.Add("X-Auth-Token: " + CommonVariables.globalUserInfo.token);
                req.Headers.Add("X-Auth-UserId: " + CommonVariables.globalUserInfo.user_id);
            }
            string result = null;

            try
            {
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(resp.GetResponseStream());
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    if (((ex as WebException).Response as HttpWebResponse).StatusCode == HttpStatusCode.Unauthorized)
                    {
                        callCounter++;
                        if (callCounter == 10)
                        {
                            callCounter = 0;
                            return null;
                        }
                        if (TuleapLevel.Authorize(CommonVariables.tuleapHost, CommonVariables.user, CommonVariables.pass) == HttpStatusCode.OK)
                            result = HttpGet(url, useToken);
                    }
                    //string s = string.Format($"{((ex as WebException).Response as HttpWebResponse).StatusCode}\r\n{ex.Data}\r\n{ex.TargetSite}\r\n{ex.HResult}\r\n{ex.Message}\r\n{ex.Source}\r\n{ex.StackTrace}\r\n{ex.InnerException}");
                    ////textBox1.Text += s;
                    //CommonFunctions.Log(s);
                }
            }
            callCounter = 0;
            return result;
        }


        public static string HttpGetJSON(string url, string json, bool useToken)
        {
            string query = "query=" + System.Web.HttpUtility.UrlEncode(json);
            string resultUrl = CommonVariables.tuleapHost + url + "?" + query;
            //resultUrl = System.Web.HttpUtility.UrlEncode(query);
            HttpWebRequest req = WebRequest.Create(resultUrl) as HttpWebRequest;
            if (useToken)
            {
                req.Headers.Add("X-Auth-Token: " + CommonVariables.globalUserInfo.token);
                req.Headers.Add("X-Auth-UserId: " + CommonVariables.globalUserInfo.user_id);
            }
            string result = null;
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }
            return result;
        }


        public static string HttpPostJSON(string url, string json, bool useToken, out System.Net.HttpStatusCode statusCode)
        {
            HttpWebRequest req = WebRequest.Create(new Uri(CommonVariables.tuleapHost + url)) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/json";  //req.ContentType = "application/x-www-form-urlencoded";

            if (useToken)
            {
                req.Headers.Add("X-Auth-Token: " + CommonVariables.globalUserInfo.token);
                req.Headers.Add("X-Auth-UserId: " + CommonVariables.globalUserInfo.user_id);
            }

            using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
            {
                sw.Write(json);
            }

            // Pick up the response:
            string result = null;
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                statusCode = resp.StatusCode;
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }

            return result;
        }

        static string HttpPost(string url, string[] paramName, string[] paramVal, bool useToken)
        {
            HttpWebRequest req = WebRequest.Create(new Uri(CommonVariables.tuleapHost + url)) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            if (useToken)
            {
                req.Headers.Add("X-Auth-Token: " + CommonVariables.globalUserInfo.token);
                req.Headers.Add("X-Auth-UserId: " + CommonVariables.globalUserInfo.user_id);
            }

            StringBuilder paramz = new StringBuilder();
            for (int i = 0; i < paramName.Length; i++)
            {
                paramz.Append(paramName[i]);
                paramz.Append("=");
                paramz.Append(System.Uri.EscapeUriString(paramVal[i]));
                paramz.Append("&");
            }

            // Encode the parameters as form data:
            byte[] formData = UTF8Encoding.UTF8.GetBytes(paramz.ToString());
            req.ContentLength = formData.Length;

            // Send the request:
            using (Stream post = req.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }

            // Pick up the response:
            string result = null;
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }

            return result;
        }

        public static string HttpDeleteJSON(string url, string json, bool useToken, out System.Net.HttpStatusCode responseCode)
        {
            HttpWebRequest req = WebRequest.Create(new Uri(CommonVariables.tuleapHost + url)) as HttpWebRequest;
            req.Method = "DELETE";
            req.ContentType = "application/json";  //req.ContentType = "application/x-www-form-urlencoded";

            if (useToken)
            {
                req.Headers.Add("X-Auth-Token: " + CommonVariables.globalUserInfo.token);
                req.Headers.Add("X-Auth-UserId: " + CommonVariables.globalUserInfo.user_id);
            }

            using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
            {
                sw.Write(json);
            }

            // Pick up the response:
            string result = null;
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                responseCode = resp.StatusCode;
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }

            return result;
        }

    }
}
