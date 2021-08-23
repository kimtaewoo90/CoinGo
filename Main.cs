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

namespace CoinGo
{
    public partial class Main : Form
    {
        
        public Main()
        {
            InitializeComponent();

            CoinStart();
        }

        public void CoinStart()
        {

            // Universe
            // string Type, string StockCode, string StockName, string Price, string Change, string TickSpeed

            string AccessKey = "yTg9B5SL2BYgQxTed5LVbuVs7gXPa2czy7xDDx5m"; //발급받은 AccessKey를 넣어줍니다.
            string SecretKey = "dxzr3r5KcJTxMkVCrFefWzhluFWPxdS1MDtAzKk5"; //발급받은 SecretKey를 넣어줍니다.

            UpbitAPI upbit = new UpbitAPI(AccessKey, SecretKey);
            UpbitWebSocket WS = new UpbitWebSocket();

            // {{  "currency": "BTT",  "balance": "40683.95140533",  "locked": "0.0",  "avg_buy_price": "5.59",  "avg_buy_price_modified": false,  "unit_currency": "KRW"}}
            List<JObject> Result = upbit.GetAccount();

            // Display Position
            for (int i = 0; i < Result.Count; i++)
            {
                var currency = Result[i].GetValue("currency").ToString().Trim();
                var balance = Result[i].GetValue("balance").ToString().Trim();
                var locked = Result[i].GetValue("locked").ToString().Trim();
                var avg_buy_price = Result[i].GetValue("avg_buy_price").ToString().Trim();
                var cur_price = "0.0";
                if (!Params.CoinInfoDict.ContainsKey(currency))
                    cur_price = "0.0";
                else cur_price = Params.CoinInfoDict[currency].curPrice.ToString().Trim();
                var rate = $"{1 - double.Parse(cur_price) / double.Parse(avg_buy_price)} %";
                var tradingPnL = $"{(double.Parse(cur_price) - double.Parse(avg_buy_price)) * double.Parse(balance)} 원";
                var unit_currency = Result[i].GetValue("unit_currency").ToString().Trim();

                PositionState state = new PositionState(currency.ToString(), balance.ToString(), locked.ToString(), avg_buy_price.ToString(), unit_currency.ToString());
                Params.CoinPositionDict[currency] = state;

                DisplayTargetCoins(currency, balance, avg_buy_price, cur_price, rate, tradingPnL);
                write_sys_log("Displayed Coin position", 0);
            }


            var Markets = upbit.GetMarkets();
            var MarketTickers = upbit.GetMarketTicker(Markets).Item1;
            var MarketTickersKrName = upbit.GetMarketTicker(Markets).Item2;

            // Make SendMsg for WebSocket
            var sendMsg = WS.MakeSendMsg(MarketTickers, "ticker");
            SendToWebSocket(sendMsg);
        }

        public void write_sys_log(String text, int is_Clear)
        {
            DateTime cur_time;
            String cur_dt;
            String cur_tm;
            String cur_dtm;

            cur_dt = "";
            cur_tm = "";

            cur_time = DateTime.Now;
            cur_dt = cur_time.ToString("yyyy-") + cur_time.ToString("MM-") + cur_time.ToString("dd");
            cur_tm = Params.CurTime;

            cur_dtm = "[" + cur_dt + " " + cur_tm + "]";

            if (is_Clear == 1)
            {
                if (LogBox.InvokeRequired)
                {
                    LogBox.BeginInvoke(new Action(() => LogBox.Clear()));
                }
                else
                {
                    this.LogBox.Clear();
                }
            }

            else
            {
                if (this.LogBox.InvokeRequired)
                {
                    LogBox.BeginInvoke(new Action(() => LogBox.AppendText("\n" + cur_dtm + text + Environment.NewLine)));
                }

                else
                {
                    this.LogBox.AppendText("\n" + cur_dtm + text + Environment.NewLine);
                    // log 기록

                    //File.AppendAllText(BotParams.Path + BotParams.LogFileName, cur_dtm + text + Environment.NewLine, Encoding.Default);
                }
            }
        }

        public void displayUniverseMarket(JObject res)
        {
            var code = res["code"].ToString();
            var curPrice = res["trade_price"].ToString();
            var change = res["signed_change_rate"].ToString();
            var volume = res["acc_trade_volume"].ToString();

            if (UniverseDataGrid.InvokeRequired)
            {
                UniverseDataGrid.Invoke(new MethodInvoker(delegate ()
                {
                    if (UniverseDataGrid.Rows.Count > 1)
                    {
                        foreach (DataGridViewRow row in UniverseDataGrid.Rows)
                        {
                            if (row.Cells["ticker"].Value == null)
                                break;

                            
                            if (row.Cells["ticker"].Value.ToString() == code || row.Cells["ticker"].Value == null)
                            {
                                row.Cells["ticker"].Value = code;
                                row.Cells["curPrice"].Value = curPrice;
                                row.Cells["change"].Value = change;
                                row.Cells["volume"].Value = volume;
                                return;
                            }
                        }

                        UniverseDataGrid.Rows.Add(code, curPrice, change, volume);
                    }

                    else UniverseDataGrid.Rows.Add(code, curPrice, change, volume);

                }));
            }

            else
            {
                if (UniverseDataGrid.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in UniverseDataGrid.Rows)
                    {
                        if (row.Cells["ticker"].Value.ToString() == code || row.Cells["ticker"].Value == null)
                        {
                            row.Cells["ticker"].Value = code;
                            row.Cells["curPrice"].Value = curPrice;
                            row.Cells["change"].Value = change;
                            row.Cells["volume"].Value = volume;
                            return;
                        }
                    }

                    UniverseDataGrid.Rows.Add(code, curPrice, change, volume);
                }

                else UniverseDataGrid.Rows.Add(code, curPrice, change, volume);
            }
        }


        public void SendToWebSocket(string sendMsg)
        {
            WebSocket webSocket = new WebSocket("wss://api.upbit.com/websocket/v1");
            webSocket.OnMessage += Ws_OnMessage;
            webSocket.Connect();
            if (webSocket.ReadyState == WebSocketState.Open)
            {
                webSocket.Send(sendMsg);
            }
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            string requestMsg = Encoding.UTF8.GetString(e.RawData);
            // Display Market Universe
            JObject res = JObject.Parse(requestMsg);
            var code = res["code"].ToString();

            CoinState state = new CoinState(res);
            Params.CoinInfoDict[code] = state;

            /*
             {{ "type": "ticker",  
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
             }}
             */

            //TODO : coin state batch 돌리고 State Dictionary 계속 업데이트 하는 방식으로 ㄱ
            //logs.write_sys_log($"{res["code"].ToString()} {res["trade_price"].ToString()}" , 0);

            displayUniverseMarket(res);

            string[] codes = code.Split('-');
            if (Params.CoinPositionDict.ContainsKey(codes[1]))
            {
                var balance = Params.CoinPositionDict[codes[1]].balance.ToString();
                var buyPrice = Params.CoinPositionDict[codes[1]].avg_buy_price.ToString();
                var curPrice = res["trade_price"].ToString();
                var rate = $"{1 - double.Parse(curPrice) / double.Parse(buyPrice)} %";
                var tradingPnL = $"{(double.Parse(curPrice) - double.Parse(buyPrice)) * double.Parse(balance)} 원";
                DisplayTargetCoins(code, balance, buyPrice, curPrice, rate, tradingPnL);
            }
        }

    }
}
