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


        // Coin
        public static Dictionary<string, CoinState> CoinInfoDict = new Dictionary<string, CoinState>();
        public static List<CoinState> CoinInfoList = new List<CoinState>();
        public static Dictionary<string, PositionState> CoinPositionDict = new Dictionary<string, PositionState>();
        public static List<string> MarketTickers = new List<string>();
    }
}
