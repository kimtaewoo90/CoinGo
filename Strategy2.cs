
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



        public Strategy2(string code, JObject _res)
        {
            ticker = code;
            res = _res;

        }


        public void Get_Avg_Volume_Before_Candle()
        {

            var CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._3, count: 10);
            CandleState candle = new CandleState(CandleData);

            Delay(100);

            Params.CandleDict[ticker] = candle;

            Params.Avg_Volume_Before_20_Candle[ticker] = candle.total_trading_volume.Average(x => double.Parse(x));
            Params.Candle_Time[ticker] = candle.date_time[candle.date_time.Count - 1].Substring(0, 4) +
                                         candle.date_time[candle.date_time.Count - 1].Substring(5, 2) +
                                         candle.date_time[candle.date_time.Count - 1].Substring(8, 2) +
                                         candle.date_time[candle.date_time.Count - 1].Substring(11, 2) +
                                         candle.date_time[candle.date_time.Count - 1].Substring(14, 2) +
                                         candle.date_time[candle.date_time.Count - 1].Substring(17, 2);
            Params.LatestCandleVolume[ticker] = double.Parse(candle.total_trading_volume[candle.total_trading_volume.Count - 1]);
            Params.Avg_Closed_Price[ticker] = candle.trade_price.Average(x => double.Parse(x));


        }

        public bool RequestLongSignal()
        {
            var signal = false;

            var cur_price = "0.0";
            if (Params.CoinInfoDict.ContainsKey(ticker))
                cur_price = Params.CoinInfoDict[ticker].curPrice.ToString();

            if (Params.Avg_Price_Now_Candle.ContainsKey(ticker) &&
                Params.Avg_Volume_Now_Candle.ContainsKey(ticker) &&
                Params.Avg_Volume_Before_20_Candle.ContainsKey(ticker) &&
                Params.LatestCandleVolume.ContainsKey(ticker))
            {
                if (Params.Avg_Volume_Now_Candle[ticker].Sum() > Params.LatestCandleVolume[ticker] &&                   // 현재Candle의 거래량 > 이전Candle의 거래량 보다 크다.
                    Params.Avg_Volume_Now_Candle[ticker].Sum() > Params.Avg_Volume_Before_20_Candle[ticker] * 1.7 &&    // 현재Candle의 거래량 > 이전 10개 Candle 거래량의 평균보다 2.5배 크다.
                    Params.Avg_Price_Now_Candle[ticker].Sum() > 550000000 &&  // 3분봉 5.5억                            // 현재Candle의 거래대금 > 5.5억
                    Params.Avg_Volume_Now_Candle[ticker].Count > 200 &&                                                 // 현재Candle의 체결 갯수
                    double.Parse(Params.CoinInfoDict[ticker].change) > 0 &&                                             // change > 0 
                    Params.bidVolume[ticker].Sum() / Params.askVolume[ticker].Sum() > 2 &&                              // 현재 Candle의 매수/매도 정도 ratio
                    (double.Parse(cur_price) > 1000 && double.Parse(cur_price) < 1000000))                              // 타겟 코인의 Price Range
                {
                    if (Params.Avg_Closed_Price[ticker] < double.Parse(cur_price) &&                                      // 현재가 > 이전 10개 Candle의 종가 평균보다 크다
                         double.Parse(cur_price) - Params.Avg_Closed_Price[ticker] > 2 * Params.upbit.GetHogaTick(double.Parse(cur_price)))
                    {
                        signal = true;
                    }
                }
                else
                {
                    signal = false;
                }

                return signal;
            }

            else return false;

        }

        public bool RequestShortSignal()
        {

            var signal = false;

            var avg_buy_price = Params.CoinPositionDict[ticker].avg_buy_price.ToString();

            // 매수 후 30초 동안은 매도 금지
            if (Params.FilledTime.ContainsKey(ticker) &&
                (DateTime.Now - Params.FilledTime[ticker]).Seconds < 30)
                return false;

            TimeSpan ts = DateTime.Now - DateTime.Now;
            if (Params.FilledTime.ContainsKey(ticker))
                ts = DateTime.Now - Params.FilledTime[ticker];

            // TODO : 변수로 받을지 값을 가져올지 고민.
            var cur_price = "0.0";
            if (Params.CoinInfoDict.ContainsKey(ticker))
                cur_price = Params.CoinInfoDict[ticker].curPrice.ToString();

            // 매도 주문
            if ( (((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100 > 1.05 ||
                 (((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100 < -1.95)) &&
                 double.Parse(cur_price) > 0.0
               )
            {
                // 익절
                if (((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100 > 1.05)
                {
                    Params.LimitOrderPrice[ticker] = double.Parse(cur_price);

                    signal = true;
                }

                // 손절
                else if (((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100 < -1.95)
                {
                    Params.LosscutCode[ticker] = DateTime.Now;

                    signal = true;
                }

                return signal;
            }

            else if (ts.Minutes >= 30)
            {
                if (((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100 > 0.05)
                {
                    Params.ProfitTimes += 1;
                    signal = true;
                }
                else
                {
                    Params.LosscutTimes += 1;
                    Params.LosscutCode[ticker] = DateTime.Now;

                    signal = true;
                }

                return signal;
            }

            else
                return signal;         
        }

        public string SendLongOrder()
        {

            var result = "";
            try
            {
                var changeResult = JObject.Parse(Params.upbit.GetOrderChance(ticker));

                Delay(100);

                if (changeResult != null && !changeResult.ContainsKey("error"))
                {

                    var accntBalance = double.Parse(changeResult["bid_account"]["balance"].ToString());
                    var orderVolume = Convert.ToDecimal(Math.Ceiling(accntBalance / double.Parse(Params.CoinInfoDict[ticker].curPrice)));

                    if (accntBalance > 5000 && ticker != "KRW-BORA" && ticker != "KRW-HUM")
                    {

                        // 시장가 매수 주문
                        result = Params.upbit.MakeOrder(market: ticker, side: UpbitAPI.UpbitOrderSide.bid, volume: Convert.ToDecimal(accntBalance * 0.95), ord_type: UpbitAPI.UpbitOrderType.price);

                        // Update BuyInfo
                        Params.TotalTradedPriceAtBoughtTime[ticker] = Params.Avg_Price_Now_Candle[ticker].Sum();
                        Params.FilledTime[ticker] = DateTime.Now;

                        //DB_Blt_Table bltDB = new DB_Blt_Table(DateTime.Now, ticker, )

                    }
                    else
                    {
                        return "failed";
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"[In SendLongOrder] {ex.Message.ToString()}");
                //write_sys_log($"SendLongOrder : {ex.Message.ToString()}", 0);

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

                var result = "";

                if (Params.LimitOrderPrice.ContainsKey(ticker))
                {
                    // 지정가 매도 주문(익절용)  
                    result = Params.upbit.MakeLimitOrder(market: ticker, side: UpbitAPI.UpbitOrderSide.ask, volume: Convert.ToDecimal(balance), price: Convert.ToDecimal(Params.LimitOrderPrice[ticker]));
                    Params.ProfitTimes += 1;

                    // * 익절도 시장가 매도로 변경.
                    //result = Params.upbit.MakeOrder(market: ticker, side: UpbitAPI.UpbitOrderSide.ask, volume: Convert.ToDecimal(balance), ord_type: UpbitAPI.UpbitOrderType.market);

                    Params.LimitOrderPrice.Remove(ticker);
                }
                else
                {
                    // 시장가 매도 주문(손절용)
                    result = Params.upbit.MakeOrder(market: ticker, side: UpbitAPI.UpbitOrderSide.ask, volume: Convert.ToDecimal(balance), ord_type: UpbitAPI.UpbitOrderType.market);
                    Params.LosscutTimes += 1;

                }


                Params.CoinPositionDict.Remove(ticker);
                Params.TotalTradedPriceAtBoughtTime.Remove(ticker);

                // Refresh all Data
                Params.Avg_Volume_Before_20_Candle = new Dictionary<string, double>();
                Params.Avg_Volume_Now_Candle = new Dictionary<string, List<double>>();
                Params.Avg_Price_Now_Candle = new Dictionary<string, List<double>>();

                return result;

            }

            catch (Exception ex)
            {
                MessageBox.Show($"[In SendShortOrder] {ex.Message.ToString()}");
                return null;
            }

        }

        public void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
        }
    }


}
