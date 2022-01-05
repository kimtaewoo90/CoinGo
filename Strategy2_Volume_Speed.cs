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
            var CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._1, count: 30);
            CandleState candle = new CandleState(CandleData);

            Params.CandleDict[ticker] = candle;

            Params.Avg_Volume_Before_20_Candle[ticker] = candle.total_trading_volume.Average(x => double.Parse(x));
            Params.Candle_Time[ticker] = candle.date_time[candle.date_time.Count - 1].Substring(14,2);
            Params.LatestCandleVolume[ticker] = double.Parse(candle.total_trading_volume[candle.total_trading_volume.Count - 1]);

            // Refresh traded volume list
            Params.Avg_Volume_Now_Candle[ticker] = new List<double>();
        }

        public bool Strategy2_Signals()
        {
            var signal = false;

            if (Params.Avg_Volume_Now_Candle[ticker].Count == 0) return false;


            if (Params.Avg_Volume_Now_Candle[ticker].Sum() > Params.Avg_Volume_Before_20_Candle[ticker] * 5 &&
                double.Parse(res["signed_change_price"].ToString()) > 0 &&
                Params.Avg_Volume_Now_Candle[ticker].Sum() > 50000000000 &&
                Params.Avg_Volume_Now_Candle[ticker].Count > 180)
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
