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
        public string Coin_Code { get; set; }
        public double Bid_1_Price { get; set; }
        public double Bid_2_Price { get; set; }
        public double Bid_3_Price { get; set; }
        public double Bid_4_Price { get; set; }
        public double Bid_5_Price { get; set; }
        public double Bid_6_Price { get; set; }
        public double Bid_7_Price { get; set; }
        public double Bid_8_Price { get; set; }
        public double Bid_9_Price { get; set; }
        public double Bid_10_Price { get; set; }
        public double Ask_1_Price { get; set; }
        public double Ask_2_Price { get; set; }
        public double Ask_3_Price { get; set; }
        public double Ask_4_Price { get; set; }
        public double Ask_5_Price { get; set; }
        public double Ask_6_Price { get; set; }
        public double Ask_7_Price { get; set; }
        public double Ask_8_Price { get; set; }
        public double Ask_9_Price { get; set; }
        public double Ask_10_Price { get; set; }
        public double Total_Ask_Size { get; set; }
        public double Total_Bid_Size { get; set; }
        public string TimeStamp { get; set; }

        public OrderbookState(JObject res)
        {
            Coin_Code = res["code"].ToString();

            Bid_1_Price = double.Parse(res["orderbook_units"][0]["bid_price"].ToString());
            Ask_1_Price = double.Parse(res["orderbook_units"][0]["ask_price"].ToString());

            Bid_2_Price = double.Parse(res["orderbook_units"][1]["bid_price"].ToString());
            Ask_2_Price = double.Parse(res["orderbook_units"][1]["ask_price"].ToString());

            Bid_3_Price = double.Parse(res["orderbook_units"][2]["bid_price"].ToString());
            Ask_3_Price = double.Parse(res["orderbook_units"][2]["ask_price"].ToString());

            Bid_4_Price = double.Parse(res["orderbook_units"][3]["bid_price"].ToString());
            Ask_4_Price = double.Parse(res["orderbook_units"][3]["ask_price"].ToString());

            Bid_5_Price = double.Parse(res["orderbook_units"][4]["bid_price"].ToString());
            Ask_5_Price = double.Parse(res["orderbook_units"][4]["ask_price"].ToString());

            Bid_6_Price = double.Parse(res["orderbook_units"][5]["bid_price"].ToString());
            Ask_6_Price = double.Parse(res["orderbook_units"][5]["ask_price"].ToString());

            Bid_7_Price = double.Parse(res["orderbook_units"][6]["bid_price"].ToString());
            Ask_7_Price = double.Parse(res["orderbook_units"][6]["ask_price"].ToString());

            Bid_8_Price = double.Parse(res["orderbook_units"][7]["bid_price"].ToString());
            Ask_8_Price = double.Parse(res["orderbook_units"][7]["ask_price"].ToString());

            Bid_9_Price = double.Parse(res["orderbook_units"][8]["bid_price"].ToString());
            Ask_9_Price = double.Parse(res["orderbook_units"][8]["ask_price"].ToString());

            Bid_10_Price = double.Parse(res["orderbook_units"][9]["bid_price"].ToString());
            Ask_10_Price = double.Parse(res["orderbook_units"][9]["ask_price"].ToString());

            Total_Bid_Size = double.Parse(res["total_bid_size"].ToString());
            Total_Ask_Size = double.Parse(res["total_ask_size"].ToString());

            TimeStamp = res["timestamp"].ToString();
        }

    }
}
