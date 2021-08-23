using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace CoinGo
{

    /*
                "type": "ticker",  
                "code": "KRW-BTC",  
                "opening_price": 57576000.0,  
                "high_price": 57777000.0,  
                "low_price": 57102000.0,  
                "trade_price": 57459000.0,  
                "prev_closing_price": 57553000.0,  
                "acc_trade_price": 121164922908.24561,  
                "change": "FALL",  
                "change_price": 94000.0,  
                "signed_change_price": -94000.0,  
                "change_rate": 0.0016332772,  
                "signed_change_rate": -0.0016332772,  
                "ask_bid": "BID",  
                "trade_volume": 0.01336369,  
                "acc_trade_volume": 2106.0143974,  
                "trade_date": "20210822",  
                "trade_time": "083933",  
                "trade_timestamp": 1629621573000,  
                "acc_ask_volume": 1157.63159926,  
                "acc_bid_volume": 948.38279814,  
                "highest_52_week_price": 81994000.0,  
                "highest_52_week_date": "2021-04-14",  
                "lowest_52_week_price": 11860000.0,  
                "lowest_52_week_date": "2020-09-07",  
                "trade_status": null,  
                "market_state": "ACTIVE",  
                "market_state_for_ios": null,  
                "is_trading_suspended": false,  
                "delisting_date": null,  
                "market_warning": "NONE",  
                "timestamp": 1629621573945,  
                "acc_trade_price_24h": 415794904957.92114, 
                "acc_trade_volume_24h": 7231.16507099,  
                "stream_type": "SNAPSHOT"

    코드
    현재가
    등락률
    거래량
    */
        
    public class CoinState
    {
        
        public JObject coinInfo { get; set; }
        public string code { get; set; }
        public string curPrice { get; set; }
        public string change { get; set; }
        public string volume { get; set; }

        public CoinState(JObject _coinInfo)
        {
            coinInfo = _coinInfo;
            code = coinInfo["code"].ToString();
            curPrice = coinInfo["trade_price"].ToString();
            change = coinInfo["signed_change_rate"].ToString();
            volume = coinInfo["acc_trade_volume"].ToString();
        }

        // 각 코인별 signal
    }
}
