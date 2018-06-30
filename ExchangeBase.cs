using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace signalBot
{
    // TODO rework to non-abs class
    public abstract class ExchangeBase:Service
    {
        private readonly string apiKey; public string ApiKey { get; set; }
        private readonly string apiSecret; public string ApiSecret { get; set; }

        abstract public bool IsThreeCandleModel(Candle c1, Candle c2, Candle c3);
        abstract public bool IsAbsorptionModel(Candle c1, Candle c2);
        abstract public bool IsBifractalModel(Candle c1, Candle c2, Candle c3, Candle c4, Candle c5);
        abstract public string GetInstrument();        

        sealed public class Candle
        {

            private decimal? open, close, high, low, volume;
            public decimal? Open { get => open; set => open = value; }
            public decimal? Close { get => close; set => close = value; }
            public decimal? High { get => high; set => high = value; }
            public decimal? Low { get => low; set => low = value; }
            public decimal? Volume { get => volume; set => volume = value; }

            private string symbol, timeframe;
            public string Symbol { get => symbol; set => symbol = value; }
            public string Timeframe { get => timeframe; set => timeframe = value; }


            private decimal? range;
            public decimal? Range
            {
                get { this.range = high - low; return range; }
            }

            private decimal? top_shadow_range;
            public decimal? Top_shadow_range
            {
                get { this.top_shadow_range = (open < close) ? high - close : high - open; return top_shadow_range; }
            }

            private decimal? bot_shadow_range;
            public decimal? Bot_shadow_range
            {
                get { this.bot_shadow_range = (open < close) ? open - low : close - low; return bot_shadow_range; }
            }

            private decimal? body;
            public decimal? Body
            {
                get { this.body = (open < close) ? close - open : open - close; return body; }
            }




            public Candle(string symbol = null, decimal? open = null, decimal? close = null, decimal? high = null, decimal? low = null, string timeframe = null, decimal? volume = null)
            {
                this.symbol = symbol;
                this.timeframe = timeframe;
                this.volume = volume;
                this.open = open;
                this.close = close;
                this.high = high;
                this.low = low;



            }

            //public Candle()
            //{
            //    this.open = 0;
            //    this.close = 0;
            //    this.high = 0;
            //    this.low = 0;
            //}



            public bool IsFat()
            {
                // TODO
                // getSettings();
                var timeframe = this.timeframe;

                // 
                // if candle more then avg from some past candles then true | ATR, EMA
                // TODO Exchange method
                // do this customizable
                return true;
            }

            public bool IsDodge()
            {
                // TODO
                return true;
            }

            public bool IsPin()
            {
                // TODO
                //if(this.Top_shadow_range>this.Body)
                //    if (this.isSmall())
                return true;
            }

        }
    }
}
