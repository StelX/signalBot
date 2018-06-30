using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace signalBot
{
    sealed class Blockchain_com:Service
    {
        public Blockchain_com()
        {
            this.Domain = "https://api.blockchain.info";
            this.ApiVersion = "";
        }
        /// <summary>
        /// USD market cap (based on 24 hour weighted price)
        /// </summary>
        /// <returns></returns>
        public String GetMarketcap24h()
        {
            return Query("/q/marketcap");
        }

        /// <summary>
        /// Number of transactions in the past 24 hours
        /// </summary>
        /// <returns></returns>
        public string GetTransactions24h()
        {
            return Query("/q/24hrtransactioncount");
        }
        /// <summary>
        /// 24 hour weighted price from the largest exchanges
        /// </summary>
        /// <returns></returns>
        public string GetPrice24h()
        {
            return Query("/q/24hrprice");
        }

        /// <summary>
        /// Number of btc sent in the last 24 hours (in satoshi)
        /// </summary>
        /// <returns></returns>
        public string GetBtcSent24h()
        {
            return Query("/q/24hrbtcsent");
        }

    }
}
