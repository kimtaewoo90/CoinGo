
/*
 
 전략소개

 
 */



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
    class Strategy2
    {
        public string ticker { get; set; }
        public JObject res { get; set; }

        public Utils util { get; set; }

        public Strategy2(string code, JObject _res, Utils _util)
        {
            ticker = code;
            res = _res;
            util = _util;
        }

        public void MainLogic()
        {
            if (Params.Is_Start_Strategy2[ticker] is true)
            {
                // Request candle data
                Get_Avg_Volume_Before_20_Candle();
                Params.Is_Start_Strategy2[ticker] = false;
                util.delay(200);
            }

            else
            {
                // Change Candle
                if (double.Parse(res["trade_time"].ToString().Substring(2, 2)) - double.Parse(Params.Candle_Time[ticker]) >= 1 ||
                    double.Parse(res["trade_time"].ToString().Substring(2, 2)) - double.Parse(Params.Candle_Time[ticker]) <= 0)
                {
                    Get_Avg_Volume_Before_20_Candle();
                    util.delay(200);

                    // 매수 후 거래량이 줄어들 때 강제 매도
                    if (Params.TotalTradedPriceAtBoughtTime.ContainsKey(ticker))
                    {
                        if (Params.LatestCandleVolume[ticker] * 3 < Params.TotalTradedPriceAtBoughtTime[ticker])
                        {
                            Params.ForcedSell[ticker] = true;
                        }
                    }
                }
                else
                {
                    Params.Avg_Volume_Now_Candle[ticker].Add(Math.Abs(double.Parse(res["trade_price"].ToString())));
                }
            }


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

        public bool RequestLongSignal()
        {
            var signal = false;

            if (Params.Avg_Volume_Now_Candle[ticker].Count == 0) return false;


            if (Params.Avg_Volume_Now_Candle[ticker].Sum() > Params.Avg_Volume_Before_20_Candle[ticker] * 5 &&
                double.Parse(res["signed_change_price"].ToString()) > 0 &&
                Params.Avg_Volume_Now_Candle[ticker].Sum() > 4000000000 &&  // 500억 => 40억 (1분봉 내 40억 거래대금)
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

        public bool RequestShortSignal()
        {

            var signal = false;

            var code = Params.CoinPositionDict[ticker].code.ToString();
            var avg_buy_price = Params.CoinPositionDict[ticker].avg_buy_price.ToString();
            
            // TODO : 변수로 받을지 값을 가져올지 고민.
            var cur_price = Params.CoinPositionDict[ticker].cur_price.ToString();

            // 매도 주문
            if (((((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100 > 1.0 ||
                ((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100 < -1.5) &&
                (code.Substring(4, code.Length - 4) != "BORA" && code.Substring(4, code.Length - 4) != "HUM") &&
                double.Parse(cur_price) > 0.0) || Params.ForcedSell[ticker])

            {


                signal = true;

            }

            return signal;
        }

        public string SendLongOrder()
        {

            var result = "";
            try
            {
                var changeResult = JObject.Parse(Params.upbit.GetOrderChance(ticker));

                if (changeResult != null)
                {

                    var accntBalance = double.Parse(changeResult["bid_account"]["balance"].ToString());
                    var orderVolume = Math.Ceiling(accntBalance / double.Parse(Params.CoinInfoDict[ticker].curPrice));

                    if (accntBalance > 5000 && ticker != "KRW-BORA" && ticker != "KRW-HUM")
                    {
                        // 매수 주문
                        result = Params.upbit.MakeOrder(market: ticker, side: UpbitAPI.UpbitOrderSide.bid, volume: Convert.ToDecimal(accntBalance * 0.95), ord_type: UpbitAPI.UpbitOrderType.price);

                        // Update BuyInfo
                        Params.TotalTradedPriceAtBoughtTime[ticker] = Params.Avg_Volume_Now_Candle[ticker].Sum();

                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //write_sys_log(ex.ToString(), 0);
                return null;
            }
        }

        public string SendShortOrder()
        {
            try
            {
                var balance = Params.CoinPositionDict[ticker].balance.ToString();
                var locked = Params.CoinPositionDict[ticker].locked.ToString();

                var result = Params.upbit.MakeOrder(market: ticker, side: UpbitAPI.UpbitOrderSide.ask, volume: Convert.ToDecimal(balance), ord_type: UpbitAPI.UpbitOrderType.market);
                Params.CoinPositionDict.Remove(ticker);

                return result;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }

        }
    }


}
