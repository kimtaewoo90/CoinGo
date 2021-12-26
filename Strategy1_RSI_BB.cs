using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinGo
{
    public class Strategy1_RSI_BB
    {
        public string ticker { get; set; }

        public Strategy1_RSI_BB(string code)
        {
            ticker = code;
        }

        public void GetCandleData(string ticker)
        {
            var CandleData = "";
            CandleState candle = new CandleState("");

            //write_sys_log(candle.)
            if (Params.Is_get_200_candle_data[ticker] is true)
            {
                // Create Dictionary
                CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._3, count: 200);
                //util.delay(200);

                candle = new CandleState(CandleData);
                Params.CandleDict[ticker] = candle;

                Params.CompareCandleTime[ticker] = Params.CandleDict[ticker].date_time[Params.CandleDict[ticker].date_time.Count - 1];

                Params.DifferencePrice[ticker] = new List<double>();
                Params.UpperSide[ticker] = new List<double>();
                Params.DownSide[ticker] = new List<double>();
                Params.RSI_List[ticker] = new List<double>();

                // RSi 계산
                Params.RSI_List[ticker].Add(CalculateRSI(candle, ticker, isFirst: true, IsSame: true));

                Params.Is_get_200_candle_data[ticker] = false;
            }
            else if (Params.Is_get_200_candle_data[ticker] is false)
            {
                try
                {
                    CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._3, count: 1);
                    //util.delay(200);
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.ToString());
                }


                var count = Params.CandleDict[ticker].code.Count;

                // 캔들 같은 봉
                if (Params.CompareCandleTime[ticker] == CandleData.Split(new char[] { ',' })[2].Substring(24, 19))
                {
                    // New High price > Old High price
                    if (double.Parse(CandleData.Split(new char[] { ',' })[4].Split(new char[] { ':' })[1]) >
                        double.Parse(Params.CandleDict[ticker].high_price[count - 1]))
                        Params.CandleDict[ticker].high_price[count - 1] = (CandleData.Split(new char[] { ',' })[4].Split(new char[] { ':' })[1]);
                    // New High price < Old High price
                    else Params.CandleDict[ticker].high_price[count - 1] = (Params.CandleDict[ticker].high_price[count - 1]);

                    // New Low price < Old Low price
                    if (double.Parse(CandleData.Split(new char[] { ',' })[5].Split(new char[] { ':' })[1]) <
                        double.Parse(Params.CandleDict[ticker].low_price[count - 1]))
                        Params.CandleDict[ticker].low_price[count - 1] = (CandleData.Split(new char[] { ',' })[5].Split(new char[] { ':' })[1]);
                    // New Low price > Old Low price
                    else Params.CandleDict[ticker].low_price[count - 1] = (Params.CandleDict[ticker].low_price[count - 1]);

                    // Trade price
                    Params.CandleDict[ticker].trade_price[count - 1] = CandleData.Split(new char[] { ',' })[6].Split(new char[] { ':' })[1];
                    Params.CandleDict[ticker].total_trading_price[count - 1] = (CandleData.Split(new char[] { ',' })[8].Split(new char[] { ':' })[1]);
                    Params.CandleDict[ticker].total_trading_volume[count - 1] = (CandleData.Split(new char[] { ',' })[9].Split(new char[] { ':' })[1]);

                    // RSI 업데이트
                    Params.RSI_List[ticker][Params.RSI_List[ticker].Count - 1] = CalculateRSI(Params.CandleDict[ticker], ticker, isFirst: false, IsSame: true);

                    //if (ticker == "KRW-BTC") write_sys_log($"Same candle : {Params.RSI_List[ticker][Params.RSI_List[ticker].Count-1]}", 0);
                }

                // 캔들 다른 봉
                else
                {
                    Params.CompareCandleTime[ticker] = CandleData.Split(new char[] { ',' })[2].Substring(24, 19);

                    Params.CandleDict[ticker].code.Add(CandleData.Split(new char[] { ',' })[0].Split(new char[] { ':' })[1]);
                    Params.CandleDict[ticker].date_time.Add(CandleData.Split(new char[] { ',' })[2].Substring(24, 19));
                    Params.CandleDict[ticker].opening_price.Add(CandleData.Split(new char[] { ',' })[3].Split(new char[] { ':' })[1]);
                    Params.CandleDict[ticker].high_price.Add(CandleData.Split(new char[] { ',' })[4].Split(new char[] { ':' })[1]);
                    Params.CandleDict[ticker].low_price.Add(CandleData.Split(new char[] { ',' })[5].Split(new char[] { ':' })[1]);
                    Params.CandleDict[ticker].trade_price.Add(CandleData.Split(new char[] { ',' })[6].Split(new char[] { ':' })[1]);
                    Params.CandleDict[ticker].total_trading_price.Add(CandleData.Split(new char[] { ',' })[8].Split(new char[] { ':' })[1]);
                    Params.CandleDict[ticker].total_trading_volume.Add(CandleData.Split(new char[] { ',' })[9].Split(new char[] { ':' })[1]);

                    Params.RSI_List[ticker].Add(CalculateRSI(Params.CandleDict[ticker], ticker, isFirst: false, IsSame: false));

                    //if (ticker == "KRW-BTC") write_sys_log($"other candle : {Params.RSI_List[ticker][Params.RSI_List[ticker].Count -1]}", 0);

                }


                // Real Time Candle Chart
                if (ticker == Params.Orderbook_ShortCode)
                {
                    //CreateCandleChart();
                }

            }
        }

        public double CalculateRSI(CandleState candle, string ticker, bool isFirst, bool IsSame)
        {
            // RSI 계산

            // Update DifferentPrice & UpperSide & DownSide
            if (IsSame && isFirst == false)
            {
                Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1] =
                    double.Parse(candle.trade_price[candle.trade_price.Count - 1]) - double.Parse(candle.trade_price[candle.trade_price.Count - 2]);

                if (Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1] > 0)
                {
                    Params.UpperSide[ticker][Params.UpperSide[ticker].Count - 1] = Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1];
                    Params.DownSide[ticker][Params.DownSide[ticker].Count - 1] = 0;
                }
                else if (Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1] < 0)
                {
                    Params.UpperSide[ticker][Params.UpperSide[ticker].Count - 1] = 0;
                    Params.DownSide[ticker][Params.DownSide[ticker].Count - 1] = Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1];
                }
            }

            // Add DifferentPrice & UpperSide & DownSide
            else if (IsSame == false && isFirst == false)
            {

                Params.DifferencePrice[ticker].Add((
                    double.Parse(candle.trade_price[candle.trade_price.Count - 1]) -
                    double.Parse(candle.trade_price[candle.trade_price.Count - 2])));


                if (Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1] > 0)
                {
                    Params.UpperSide[ticker].Add(Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1]);
                    Params.DownSide[ticker].Add(0);
                }
                else if (Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1] < 0)
                {
                    Params.UpperSide[ticker].Add(0);
                    Params.DownSide[ticker].Add(-Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1]);
                }

            }

            // Initial add DifferentPrice & UpperSide & DownSide
            else if (isFirst)
            {
                for (int i = 1; i < candle.trade_price.Count; i++)
                {
                    Params.DifferencePrice[ticker].Add((double.Parse(candle.trade_price[i]) - double.Parse(candle.trade_price[i - 1])));
                }

                for (int i = 0; i < Params.DifferencePrice[ticker].Count; i++)
                {
                    if (Params.DifferencePrice[ticker][i] > 0)
                    {
                        Params.UpperSide[ticker].Add(Params.DifferencePrice[ticker][i]);
                        Params.DownSide[ticker].Add(0);
                    }
                    else if (Params.DifferencePrice[ticker][i] < 0)
                    {
                        Params.UpperSide[ticker].Add(0);
                        Params.DownSide[ticker].Add(-Params.DifferencePrice[ticker][i]);
                    }
                }
            }


            var au = Params.UpperSide[ticker].Average();
            var ad = Params.DownSide[ticker].Average();

            var rs = au / ad;
            var rsi = rs / (1 + rs) * 100;


            return rsi;

        }

        // SignalThread
        public void UpdateSignals(Newtonsoft.Json.Linq.JObject res)
        {
            var signal = false;
            if (!Params.Signal_1.ContainsKey(ticker)) Params.Signal_1[ticker] = false;
            if (!Params.Signal_2.ContainsKey(ticker)) Params.Signal_2[ticker] = false;
            var first_signal = Params.Signal_1[ticker];
            var second_signal = Params.Signal_2[ticker];


            if (Params.RSI_List[ticker][Params.RSI_List[ticker].Count - 1] < 30) first_signal = true;
            if (first_signal is true && Params.RSI_List[ticker][Params.RSI_List[ticker].Count - 1] > 30) second_signal = true;

            if (first_signal && second_signal) signal = true;

            if (signal is true)
            {
                // 매수 진행
            }

        }
    }
}
