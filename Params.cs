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

        public readonly static string AccessKey = "yTg9B5SL2BYgQxTed5LVbuVs7gXPa2czy7xDDx5m"; //발급받은 AccessKey를 넣어줍니다.
        public readonly static string SecretKey = "dxzr3r5KcJTxMkVCrFefWzhluFWPxdS1MDtAzKk5"; //발급받은 SecretKey를 넣어줍니다.

        public static UpbitAPI upbit = new UpbitAPI(AccessKey, SecretKey);

        public static string Orderbook_ShortCode { get; set; } = "KRW-BTC";
        public static List<string> MarketTickers = new List<string>();

        // Display functions
        public static bool IsDisplayPosition { get; set; } = false;
        public static bool IsDisplayOrderbook { get; set; } = false;
        public static bool IsDisplayChart { get; set; } = false;

        // Blotter (Order)
        public static Dictionary<string, double> TotalTradedPriceAtBoughtTime = new Dictionary<string, double>();
        public static Dictionary<string, double> LatestCandleVolume = new Dictionary<string, double>();
        public static Dictionary<string, bool> ForcedSell = new Dictionary<string, bool>();

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
        public static Dictionary<string, List<double>> RSI_List = new Dictionary<string, List<double>>();

        // Position
        public static double TotalAsset { get; set; }
        public static double CoinAsset { get; set; }
        public static double CashAsset { get; set; }
        public static double PnL { get; set; }
        public static double PnLChange { get; set; }



        // Strategy1 RSI and BB
        public static Dictionary<string, bool> Is_get_200_candle_data = new Dictionary<string, bool>();

        public static Dictionary<string, bool> Signal_1 = new Dictionary<string, bool>();
        public static Dictionary<string, bool> Signal_2 = new Dictionary<string, bool>();

        public static Dictionary<string, List<double>> CoinTickData = new Dictionary<string, List<double>>();

        // Strategy2 Volume Strategy
        public static Dictionary<string, bool> Is_Start_Strategy2 = new Dictionary<string, bool>();
        public static Dictionary<string, double> Avg_Volume_Before_20_Candle = new Dictionary<string, double>();
        public static Dictionary<string, List<double>> Avg_Volume_Now_Candle = new Dictionary<string, List<double>>();
        public static Dictionary<string, string> Candle_Time = new Dictionary<string, string>();

        public static List<string> Oppertunity = new List<string>();
    }
}
