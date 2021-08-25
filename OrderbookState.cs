using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace CoinGo
{
    public class OrderbookState
    {
        public double Bid_1_Price { get; set; }
        public double Bid_2_Price { get; set; }
        public double Bid_3_Price { get; set; }
        public double Bid_4_Price { get; set; }
        public double Bid_5_Price { get; set; }
        public double Bid_6_Price { get; set; }
        public double Bid_7_Price { get; set; }
        public double Bid_8_Price { get; set; }
        public double Bid_9_Price { get; set; }
        public double Bid_10_Pprice { get; set; }
        public double Ask_1_price { get; set; }
        public double Ask_2_price { get; set; }
        public double Ask_3_price { get; set; }
        public double Ask_4_price { get; set; }
        public double Ask_5_price { get; set; }
        public double Ask_6_price { get; set; }
        public double Ask_7_price { get; set; }
        public double Ask_8_price { get; set; }
        public double Ask_9_price { get; set; }
        public double Ask_10_price { get; set; }
        public double TradingVolume { get; set; }
        public double CurrentPrice { get; set; }

        public OrderbookState(JObject res)
        {

        }

    }
}
