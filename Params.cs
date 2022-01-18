using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CoinGo
{
    public static class Params
    {
        public static string CurTime { get; set; }
        public static DateTime cur_time { get; set; }

        public readonly static string AccessKey = "yTg9B5SL2BYgQxTed5LVbuVs7gXPa2czy7xDDx5m"; //발급받은 AccessKey를 넣어줍니다.
        public readonly static string SecretKey = "dxzr3r5KcJTxMkVCrFefWzhluFWPxdS1MDtAzKk5"; //발급받은 SecretKey를 넣어줍니다.

        public static UpbitAPI upbit = new UpbitAPI(AccessKey, SecretKey);

        public static List<string> ExceptCoinList = new List<string>();

        public static string Orderbook_ShortCode { get; set; } = "KRW-BTC";
        public static List<string> MarketTickers = new List<string>();
        public static int ProfitTimes { get; set; } = 0;
        public static int LosscutTimes { get; set; } = 0;

        //public static List<string> LosscutCode = new List<string>();
        public static Dictionary<string, DateTime> LosscutCode = new Dictionary<string, DateTime>();


        // Display functions
        public static bool IsDisplayPosition { get; set; } = false;
        public static bool IsDisplayOrderbook { get; set; } = false;
        public static bool IsDisplayChart { get; set; } = false;

        // Blotter (Order)
        public static Dictionary<string, double> TotalTradedPriceAtBoughtTime = new Dictionary<string, double>();
        public static Dictionary<string, double> LatestCandleVolume = new Dictionary<string, double>();
        public static Dictionary<string, bool> ForcedSell = new Dictionary<string, bool>();
        public static Dictionary<string, DateTime> FilledTime = new Dictionary<string, DateTime>();
        public static Dictionary<string, double> LimitOrderPrice = new Dictionary<string, double>();

        //public static 
        public static double Strategy_PnL { get; set; } = 0;

        // Coin
        public static Dictionary<string, CoinState> CoinInfoDict = new Dictionary<string, CoinState>();
        public static List<CoinState> CoinInfoList = new List<CoinState>();
        public static Dictionary<string, PositionState> CoinPositionDict = new Dictionary<string, PositionState>();
        public static Dictionary<string, OrderbookState> CoinOrderbookDict = new Dictionary<string, OrderbookState>();

        // CandleData
        public static Dictionary<string, CandleState> CandleDict = new Dictionary<string, CandleState>();
        public static Dictionary<string, string> CompareCandleTime = new Dictionary<string, string>();

        // RSI
        public static Dictionary<string, List<double>> DifferencePrice = new Dictionary<string, List<double>>();
        public static Dictionary<string, List<double>> UpperSide = new Dictionary<string, List<double>>();
        public static Dictionary<string, List<double>> DownSide = new Dictionary<string, List<double>>();
        //public static Dictionary<string, List<double>> RSI_List = new Dictionary<string, List<double>>();

        // Position
        public static double TotalAsset { get; set; }
        public static double CoinAsset { get; set; }
        public static double CashAsset { get; set; }
        public static double PnL { get; set; }
        public static double PnLChange { get; set; }



        // Strategy1 RSI and BB
        public static Dictionary<string, bool> Is_first_down_touch = new Dictionary<string, bool>();
        public static Dictionary<string, bool> Is_second_down_touch= new Dictionary<string, bool>();
        public static Dictionary<string, bool> Is_first_upper_touch = new Dictionary<string, bool>();
        public static Dictionary<string, bool> Is_second_upper_touch = new Dictionary<string, bool>();
        public static Dictionary<string, List<double>> RSI_list = new Dictionary<string, List<double>>();

        // Strategy2 Volume Strategy
        public static Dictionary<string, bool> Is_Start_Strategy2 = new Dictionary<string, bool>();                         // Initial Get Candle
        public static Dictionary<string, double> Avg_Volume_Before_20_Candle = new Dictionary<string, double>();            // 이전 캔들봉의 거래량 평균
        public static Dictionary<string, List<double>> Avg_Volume_Now_Candle = new Dictionary<string, List<double>>();      // 현재 캔들의 거래량 리스트
        public static Dictionary<string, List<double>> Avg_Price_Now_Candle = new Dictionary<string, List<double>>();       // 현재 캔들의 거래대금 리스트
        public static Dictionary<string, string> Candle_Time = new Dictionary<string, string>();                            // 최신 캔들봉의 시간
        public static Dictionary<string, double> BuySignalRatio = new Dictionary<string, double>();                         // 현재 캔들의 거래대금 / 이전캔들의 거래대금평균
        public static Dictionary<string, double> SellSignalRatio = new Dictionary<string, double>();                        // 사용안함
        public static Dictionary<string, double> Avg_Closed_Price = new Dictionary<string, double>();                       // 이전캔들들의 종가 평균
        public static List<string> Oppertunity = new List<string>();                                                        // 사용안함

        public static Dictionary<string, List<double>> TradeVolume = new Dictionary<string, List<double>>();
        public static Dictionary<string, List<double>> HistoricalTickSpeed = new Dictionary<string, List<double>>();
        public static Dictionary<string, double> CurrentTickSpeed = new Dictionary<string, double>();
        public static Dictionary<string, double> SpeedRatio = new Dictionary<string, double>();

        // DB
        public static Dictionary<string, DB_Blt_Table> bltDBclass = new Dictionary<string, DB_Blt_Table>();
    }
}
