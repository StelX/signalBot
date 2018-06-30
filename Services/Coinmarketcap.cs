using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace signalBot
{
    // https://coinmarketcap.com/ru/api/
    sealed class Coinmarketcap:Service
    {

        public Coinmarketcap()
        {
            this.Domain = "https://api.coinmarketcap.com";
            this.ApiVersion = "/v2";
        }
        /// <summary>
        /// Get BTC percentage of market capitalization
        /// </summary>
        /// <returns></returns>
        public string GetBtcDominanceByCap()
        {
            var json_data = Query($"/global");
            var list_of_objects = Newtonsoft.Json.JsonConvert.DeserializeObject<Parser>(json_data);

            return list_of_objects.Data.BitcoinPercentageOfMarketCap.ToString();
        }

        public sealed partial class Parser
        {
            [JsonProperty("data")]
            public Data Data { get; set; }

            [JsonProperty("metadata")]
            public Metadata Metadata { get; set; }
        }

        public sealed partial class Data
        {
            [JsonProperty("active_cryptocurrencies")]
            public long ActiveCryptocurrencies { get; set; }

            [JsonProperty("active_markets")]
            public long ActiveMarkets { get; set; }

            [JsonProperty("bitcoin_percentage_of_market_cap")]
            public double BitcoinPercentageOfMarketCap { get; set; }

            [JsonProperty("quotes")]
            public Quotes Quotes { get; set; }

            [JsonProperty("last_updated")]
            public long LastUpdated { get; set; }
        }

        public sealed partial class Quotes
        {
            [JsonProperty("USD")]
            public Usd Usd { get; set; }
        }

        public sealed partial class Usd
        {
            [JsonProperty("total_market_cap")]
            public double TotalMarketCap { get; set; }

            [JsonProperty("total_volume_24h")]
            public double TotalVolume24H { get; set; }
        }

        public sealed partial class Metadata
        {
            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("error")]
            public object Error { get; set; }
        }

        public sealed partial class Parser
        {
            public static Parser FromJson(string json) => JsonConvert.DeserializeObject<Parser>(json,Converter.Settings);
        }

        //public static class Serialize
        //{
        //    public static string ToJson(this Parser self) => JsonConvert.SerializeObject(self, Converter.Settings);
        //}

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
            };
        }
    }
}
