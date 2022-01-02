using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using System.Threading;

namespace CoinGo
{
    class Strategy2_Volume_Speed
    {
        public string ticker { get; set; }
        public JObject res { get; set; }

        public Strategy2_Volume_Speed(string code, JObject _res)
        {
            ticker = code;
            res = _res;
        }

        public void Get_Avg_Volume_Before_20_Candle()
        {         
            var CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._3, count: 20);
            CandleState candle = new CandleState(CandleData);

            Params.CandleDict[ticker] = candle;

            Params.Avg_Volume_Before_20_Candle[ticker] = candle.total_trading_volume.Average(x => Math.Abs(double.Parse(x)));
            Params.Candle_Time[ticker] = candle.date_time[candle.date_time.Count - 1].Substring(14,2);

            // Refresh traded volume list
            Params.Avg_Volume_Now_Candle[ticker] = new List<double>();
        }

        public bool Strategy2_Signals()
        {
            var signal = false;

            if (Params.Avg_Volume_Now_Candle[ticker].Count == 0) return false;


            if (Params.Avg_Volume_Now_Candle[ticker].Average(x => Math.Abs(x)) > Params.Avg_Volume_Before_20_Candle[ticker] &&
                double.Parse(res["signed_change_price"].ToString()) > 0 &&
                double.Parse(res["acc_trade_price"].ToString()) > 5000000000 &&
                Params.Avg_Volume_Now_Candle[ticker].Count > 50)
            {
                signal = true;                
            }
            else
            {
                signal = false;
            }

            return signal;
        }
    }
}
