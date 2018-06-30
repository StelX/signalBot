using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;

namespace signalBot
{
    public class Service
    {

        protected string Domain { get; set; }
        protected string ApiVersion { get; set; }

        public Service(string domain, string api_version="")
        {
            this.Domain = domain;
            this.ApiVersion = api_version;
        }

        protected Service() { }

        protected string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value)));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        protected string BuildJSON(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            var entries = new List<string>();
            foreach (var item in param)
                entries.Add(string.Format("\"{0}\":\"{1}\"", item.Key, item.Value));

            return "{" + string.Join(",", entries) + "}";
        }

        public string Query(string function, Dictionary<string, string> param = null, string method = "GET", bool json = false)
        {
            string paramData = json ? BuildJSON(param) : BuildQueryData(param);
            string url = ApiVersion + function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
            string postData = (method != "GET") ? paramData : "";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Domain + url);
            Console.WriteLine(url.ToString());
            webRequest.Method = method;

            try
            {
                if (postData != "")
                {
                    webRequest.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
                    var data = Encoding.UTF8.GetBytes(postData);
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                using (Stream str = webResponse.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                    {
                        Console.WriteLine("Empty request");
                        //throw;
                    }


                    if (response != null)
                    {
                        //StreamReader StreamReader = new StreamReader(response.GetResponseStream());

                        Console.WriteLine("Code: " + ((HttpWebResponse)response).StatusCode
                            + "\nDescription: " + ((HttpWebResponse)response).StatusDescription
                            + "\nMessage: " + wex.Message);
                        //throw;
                    }

                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Unknown exception" + ex.ToString());
                Console.ReadKey();

                return null;
            }
        }
        
//        public Dictionary<string, string> GetData(string function, Dictionary<string, string> param = null, string method="GET")
//        {
//            List<string> result;

//            var dic = new Dictionary<string, string>();
//;           var json_data = Query(function: function, method: method, param: param);
//            var list_of_objects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(json_data);
//            Dictionary<string, object> objs_dic = new Dictionary<string, object>();
//            var i = 0;

            
//            //foreach (var key in param.Keys)
//            //{

//            //}

//            foreach (var obj in list_of_objects)
//            {

//                objs_dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());
//            }

//            List<string> keys_list = new List<string>(objs_dic.Keys);
//            foreach (var key in keys_list)
//            {
//                result = 
//            }

//            return dic;
             
//        }
        
    }
}
