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

        // Coin
        public static Dictionary<string, CoinState> CoinInfoDict = new Dictionary<string, CoinState>();
        public static List<CoinState> CoinInfoList = new List<CoinState>();
        public static Dictionary<string, PositionState> CoinPositionDict = new Dictionary<string, PositionState>();
        public static Dictionary<string, OrderbookState> CoinOrderbookDict = new Dictionary<string, OrderbookState>();
        public static List<string> MarketTickers = new List<string>();

        // CandleData
        public static Dictionary<string, CandleState> CandleDict = new Dictionary<string, CandleState>();
        public static Dictionary<string, string> CompareCandleTime = new Dictionary<string, string>();

        // RSI
        public static Dictionary<string, List<double>> DifferencePrice = new Dictionary<string, List<double>>();
        public static Dictionary<string, List<double>> UpperSide = new Dictionary<string, List<double>>();
        public static Dictionary<string, List<double>> DownSide = new Dictionary<string, List<double>>();
        public static Dictionary<string, List<double>> RSI_List = new Dictionary<string, List<double>>();


        public static Dictionary<string, List<double>> CoinTickData = new Dictionary<string, List<double>>();
    }
}
