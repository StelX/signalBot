
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;


namespace signalBot
{
    public class OrderBookItem
    {
        public string Symbol { get; set; }
        public int Level { get; set; }
        public int BidSize { get; set; }
        public decimal BidPrice { get; set; }
        public int AskSize { get; set; }
        public decimal AskPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public sealed class BitMEXApi:ExchangeBase
    {
        private const string domain = "https://testnet.bitmex.com";
        private readonly string apiKey;
        private readonly string apiSecret;
        private readonly int rateLimit;
        
        public BitMEXApi(string bitmexKey = AppSettings.Bitmex_apiKey, string bitmexSecret = AppSettings.Bitmex_apiSecret, int rateLimit = 5000)
        {
            this.apiKey = bitmexKey;
            this.apiSecret = bitmexSecret;
            this.rateLimit = rateLimit;
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private long GetNonce()
        {
            DateTime yearBegin = new DateTime(1990, 1, 1);
            return DateTime.UtcNow.Ticks - yearBegin.Ticks;
        }

        public string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false)
        {
            string paramData = json ? BuildJSON(param) : BuildQueryData(param);
            string url = "/api/v1" + function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
            string postData = (method != "GET") ? paramData : "";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
            Console.WriteLine(url.ToString());
            webRequest.Method = method;

            if (auth)
            {
                string nonce = GetNonce().ToString();
                string message = method + url + nonce + postData;
                byte[] signatureBytes = hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
                string signatureString = ByteArrayToString(signatureBytes);

                webRequest.Headers.Add("api-nonce", nonce);
                webRequest.Headers.Add("api-key", apiKey);
                webRequest.Headers.Add("api-signature", signatureString);
            }

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

        //public List<OrderBookItem> GetOrderBook(string symbol, int depth)
        //{
        //    var param = new Dictionary<string, string>();
        //    param["symbol"] = symbol;
        //    param["depth"] = depth.ToString();
        //    string res = Query("GET", "/orderBook", param);
        //    return JsonSerializer.DeserializeFromString<List<OrderBookItem>>(res);
        //}

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        public string GetOrders()
        {
            var param = new Dictionary<string, string>();
            //param["symbol"] = "XBTUSD";
            //param["filter"] = "{\"open\":true}";
            //param["columns"] = "";
            //param["count"] = 100.ToString();
            //param["start"] = 0.ToString();
            //param["reverse"] = false.ToString();
            //param["startTime"] = "";
            //param["endTime"] = "";
            return Query("GET", "/order", param, true);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="symbol">
        /// Instrument symbol. Send a bare series (e.g. XBU) to get data for the nearest expiring contract in that series.
        /// You can also send a timeframe, e.g. XBU:monthly. Timeframes are daily, weekly, monthly, quarterly, and biquarterly.
        /// </param>
        /// <param name="filter">Generic table filter. Send JSON key/value pairs, such as {"key": "value"}. You can key on individual fields, and do more advanced querying on timestamps.</param>
        /// <param name="columns">Array of column names to fetch. If omitted, will return all columns.</param>
        /// <param name="count">Number of results to fetch.</param>
        /// <param name="start">Starting point for results.</param>
        /// <param name="reverse">If true, will sort results newest first.</param>
        /// <param name="startTime">Starting date filter for results.</param>
        /// <param name="endTime">Ending date filter for results.</param>
        /// <returns></returns>
        public string GetOrders(string symbol="{}", string filter="{}", string columns="{}", int count=100, int start=0, bool reverse=false, Nullable<DateTime> startTime =null, Nullable<DateTime> endTime=null)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["filter"] = filter;
            param["columns"] = columns;
            param["count"] = count.ToString();
            param["start"] = start.ToString();
            param["reverse"] = false.ToString();
            param["startTime"] = startTime.ToString();
            param["endTime"] = endTime.ToString();
            return Query("GET", "/order", param, true);
        }

        /// <summary>
        /// Create a new order.
        /// </summary>
        /// <param name="symbol">Instrument symbol. e.g. 'XBTUSD'.</param>
        /// <param name="side">Order side. Valid options: Buy, Sell.</param>
        /// <param name="price">Optional limit price for 'Limit', 'StopLimit', and 'LimitIfTouched' orders.</param>
        /// <param name="orderQty">Order quantity in units of the instrument (i.e. contracts).</param>
        /// <param name="ordType">
            /// Order type. Valid options: Market, Limit, Stop, StopLimit, MarketIfTouched, LimitIfTouched, MarketWithLeftOverAsLimit, Pegged.
        /// </param>
        /// <param name="stopPx">
            /// Optional trigger price for 'Stop', 'StopLimit', 'MarketIfTouched', and 'LimitIfTouched' orders. 
            /// Use a price below the current price for stop-sell orders and buy-if-touched orders. 
            /// Use execInst of 'MarkPrice' or 'LastPrice' to define the current price used for triggering.
        /// </param>
        /// <returns></returns>
        public string PostOrders(string symbol, string side = "Buy", double? price=null, double orderQty=1.0, string ordType="Market", double? stopPx=null)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol.ToUpper(); // XBTUSD
            param["side"] = side; // Buy | Sell
            param["price"] = price.ToString();
            param["orderQty"] = orderQty.ToString();
            param["ordType"] = ordType; // Valid options: Market, Limit, Stop, StopLimit, MarketIfTouched, LimitIfTouched, MarketWithLeftOverAsLimit, Pegged.
            param["stopPx"] = stopPx.ToString();
            return Query("POST", "/order", param, true);
        }


        /// <summary>
        /// Cancel all orders
        /// </summary>
        /// <returns></returns>
        public string DeleteOrders()
        {
            var param = new Dictionary<string, string>();
            return Query("DELETE", "/order/all", param, true, true);
        }

        /// <summary>
        /// Cancel all orders
        /// </summary>
        /// <param name="symbol">Optional symbol. If provided, only cancels orders for that symbol.</param>
        /// <param name="filter">Optional filter for cancellation. Use to only cancel some orders, e.g. {"side": "Buy"}.</param>
        /// <param name="text">Optional cancellation annotation. e.g. 'Spread Exceeded'</param>
        /// <returns></returns>
        public string DeleteOrders(string symbol = "{}", string filter = "{}", string text = "{}")
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["filter"] = filter;
            param["text"] = text;
            return Query("DELETE", "/order/all", param, true, true);
        }

        /// <summary>
        /// Delete order by ID
        /// </summary>
        /// <param name="orderID">Order ID(s).</param>
        /// <param name="text">Optional cancellation annotation. e.g. 'Spread Exceeded'.</param>
        /// <param name="clOrdID">Client Order ID(s). See POST /order.</param>
        /// <returns></returns>
        public string DeleteOrder(string orderID, string text= "cancel by ID", string clOrdID="{}")
        {
            var param = new Dictionary<string, string>();
            param["orderID"] = orderID;
            param["text"] = text;
            param["clOrdID"] = clOrdID;
            return Query("DELETE", "/order/", param, true, true);
        }

        /// <summary>
        /// Get all active instruments
        /// </summary>
        /// <returns></returns>
        public override string GetInstrument()
        {
            var param = new Dictionary<string, string>();

            return Query("GET", "/instrument/active/", param, true);
        }

        /// <summary>
        /// Get instruments by symbol
        /// </summary>
        /// <param name="symbol">
        ///     Instrument symbol. Send a bare series (e.g. XBU) to get data for the nearest expiring contract in that series.
        ///     You can also send a timeframe, e.g. XBU:monthly. Timeframes are daily, weekly, monthly, quarterly, and biquarterly.
        /// </param>
        /// <returns></returns>
        public string GetInstrument(string symbol)
        {
            var param = new Dictionary<string, string>();

            param["symbol"] = symbol;

            return Query("GET", "/instrument", param, true);
        }

        /// <summary>
        /// Get list of assets
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        public List<string> GetAssets() { return new List<string>(); }

        // TODO: возвращает минимальный размер ордера биржи
        public string GetMinimalOrder()
        {
            return null;
        }

        /// <summary>
        /// Get previous trades in time buckets.
        /// </summary>
        /// <param name="symbol">
        /// Instrument symbol. Send a bare series (e.g. XBU) to get data for the nearest expiring contract in that series.
        /// You can also send a timeframe, e.g. XBU:monthly. Timeframes are daily, weekly, monthly, quarterly, and biquarterly.
        /// </param>
        /// <param name="binSize">Time interval to bucket by. Available options: [1m,5m,1h,1d].</param>
        /// <param name="partial">If true, will send in-progress (incomplete) bins for the current time period.</param>
        /// <param name="filter">Generic table filter. Send JSON key/value pairs, such as {"key": "value"}. </param>
        /// <param name="columns">Array of column names to fetch. If omitted, will return all columns.</param>
        /// <param name="count">Number of results to fetch.</param>
        /// <param name="start">Starting point for results.</param>
        /// <param name="reverse">If true, will sort results newest first.</param>
        /// <param name="startTime">Starting date filter for results.</param>
        /// <param name="endTime">Ending date filter for results.</param>
        /// <returns>json</returns>
        public string GetPrevTrades(string symbol, string binSize="1d", bool partial=false, string filter=null, string columns=null, int count=100, int? start=null, bool reverse=false, DateTime? startTime=null, DateTime? endTime=null)
        {
            var param = new Dictionary<string, string>();

            param["binSize"] = binSize;
            param["partial"] = partial.ToString();
            param["symbol"] = symbol;
            param["filter"] = filter;
            param["columns"] = columns;
            param["count"] = count.ToString();
            param["start"] = start.ToString();
            param["reverse"] = reverse.ToString();
            param["startTime"] = startTime.ToString();
            param["endTime"] = endTime.ToString();

            return Query("GET", "/trade/bucketed", param, true);
        }

        private byte[] hmacsha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        #region RateLimiter

        private long lastTicks = 0;
        private object thisLock = new object();

        private void RateLimit()
        {
            lock (thisLock)
            {
                long elapsedTicks = DateTime.Now.Ticks - lastTicks;
                var timespan = new TimeSpan(elapsedTicks);
                if (timespan.TotalMilliseconds < rateLimit)
                    Thread.Sleep(rateLimit - (int)timespan.TotalMilliseconds);
                lastTicks = DateTime.Now.Ticks;
            }
        }

        #endregion RateLimiter

        public Candle GetCandle(string symbol, string binSize, int? start = null)
        {
            var bitmex = new BitMEXApi(apiKey, apiSecret);
            var json_data = bitmex.GetPrevTrades(symbol: symbol, binSize: binSize, count: 1, reverse: true, start: start);
            var list_of_objects = JsonConvert.DeserializeObject<List<object>>(json_data);
            Dictionary<string, object> objs_dic;

            var candle = new Candle();

            foreach (var obj in list_of_objects)
            {
                objs_dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());

                candle = new Candle
                {
                    Open = Convert.ToDecimal(objs_dic["open"]),
                    Symbol = objs_dic["symbol"].ToString(),
                    Close = Convert.ToDecimal(objs_dic["close"]),
                    High = Convert.ToDecimal(objs_dic["high"]),
                    Low = Convert.ToDecimal(objs_dic["low"]),
                    Timeframe = binSize,
                    Volume = Convert.ToDecimal(objs_dic["volume"])
                };

                Console.WriteLine(JsonConvert.SerializeObject(candle));
            }
            
            return candle;
        }

        public override bool IsThreeCandleModel(Candle c1, Candle c2, Candle c3)
        {

            //if (c3.Body > (c1.Body + c2.Body))
            //{
            //    if ((c2.Close < c1.Close))
            //    {
            //        if (c3.Close > (c1.Close) & c3.Close > (c2.Close))
            //        {
            //            return true;
            //        }
            //    }
            //}
            //return false;
            return true;
        }

         public override bool IsAbsorptionModel(Candle c1, Candle c2)
        {
            //if (ApproxEqual(c1.Body, c2.Body)) return true;
            //else return false;

            return true;
        }


         public override bool IsBifractalModel(Candle c1, Candle c2, Candle c3, Candle c4, Candle c5)
        {
            
            return true;
        }


        // TODO generic class for markets. Like "Market<Binance>".


    }
}
