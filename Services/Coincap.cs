using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace signalBot
{
    sealed class Coincap:Service
    {
        public Coincap()
        {
            this.Domain = "http://coincap.io";
            this.ApiVersion = "";
        }

        public string GetVolume(string symbol)
        {
            string volume = ""; // volume 24h usd
            var json_data = Query($"/page/{symbol.ToUpper()}");
            var list_of_objects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(json_data);
            Dictionary<string, object> objs_dic = new Dictionary<string, object>();

            foreach (var obj in list_of_objects)
            {
                objs_dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());
                volume = objs_dic["volume"].ToString();
            }
            return volume;
        }

        public string GetCap(string symbol)
        {
            string market_cap = "";
            var json_data = Query($"/page/{symbol.ToUpper()}");
            var list_of_objects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(json_data);
            Dictionary<string, object> objs_dic = new Dictionary<string, object>();

            foreach (var obj in list_of_objects)
            {
                objs_dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());
                market_cap = objs_dic["market_cap"].ToString();
            }
            return market_cap;
        }

        public ValueTuple<string, string> GetVolAndCap(string symbol)
        {
            var market_cap = "";
            var volume = "";
            var json_data = Query($"/page/{symbol.ToUpper()}");
            var list_of_objects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(json_data);
            Dictionary<string, object> objs_dic = new Dictionary<string, object>();

            foreach (var obj in list_of_objects)
            {
                objs_dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());
                market_cap = objs_dic["market_cap"].ToString();
                volume = objs_dic["volume"].ToString();
            }
            return (market_cap, volume);
        }
    }
}
