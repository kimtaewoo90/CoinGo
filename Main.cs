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
    public partial class Main : Form
    {

        Thread PositionThread = null;
        Thread OrderbookThread = null;

        public Main()
        {
            InitializeComponent();

            CoinStart();
        }

        public void CoinStart()
        {

            PositionThread = new Thread(new ThreadStart(UpdatePosition));
            PositionThread.Start();

            OrderbookThread = new Thread(new ThreadStart(UpdateOrderbook));
            OrderbookThread.Start();

            UpbitWebSocket WS = new UpbitWebSocket();

            var Markets = Params.upbit.GetMarkets();
            var MarketTickers = Params.upbit.GetMarketTicker(Markets).Item1;
            var MarketTickersKrName = Params.upbit.GetMarketTicker(Markets).Item2;

            // Make SendMsg for WebSocket
            var sendMsg = WS.MakeSendMsg(MarketTickers, "ticker");
            SendWebSocket(sendMsg);
        }

        /* WebSocket Thread for Coin Information */
        public void SendWebSocket(string sendMsg)
        {
            WebSocket webSocket = new WebSocket("wss://api.upbit.com/websocket/v1");
            webSocket.OnMessage += Ws_TickerMessage;
            webSocket.Connect();
            if (webSocket.ReadyState == WebSocketState.Open)
            {
                webSocket.Send(sendMsg);
            }
        }

        private void Ws_TickerMessage(object sender, MessageEventArgs e)
        {
            string requestMsg = Encoding.UTF8.GetString(e.RawData);
            // Display Market Universe
            JObject res = JObject.Parse(requestMsg);
            
            if( res["type"].ToString() == "ticker")
            {
                var code = res["code"].ToString();

                CoinState state = new CoinState(res);
                Params.CoinInfoDict[code] = state;
                displayUniverseMarket(res);
            }

            else if (res["type"].ToString() == "orderbook")
            {
                var code = res["code"].ToString();

                OrderbookState state = new OrderbookState(res);
                Params.CoinOrderbookDict[code] = state;

                write_sys_log($"{code} {res["orderbook_units"][0]["ask_price"]}/{res["orderbook_units"][0]["bid_price"]}", 0);
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

        public void UpdatePosition()
        {
            List<JObject> Result = Params.upbit.GetAccount();
            while(true)
            {
                // Display Position
                for (int i = 0; i < Result.Count; i++)
                {
                    var currency = Result[i].GetValue("currency").ToString().Trim();
                    var balance = Result[i].GetValue("balance").ToString().Trim();
                    var locked = Result[i].GetValue("locked").ToString().Trim();
                    var avg_buy_price = Result[i].GetValue("avg_buy_price").ToString().Trim();
                    var cur_price = "0.0";

                    //string[] codes = currency.Split('-');
                    string code = $"KRW-{currency}";
                    if (Params.CoinInfoDict.Count != 0)
                    {
                        if (Params.CoinInfoDict.ContainsKey(code))
                            cur_price = Params.CoinInfoDict[code].curPrice;   
                    }


                    var rate = $"{1 - double.Parse(cur_price) / double.Parse(avg_buy_price)} %";
                    var tradingPnL = $"{(double.Parse(cur_price) - double.Parse(avg_buy_price)) * double.Parse(balance)} 원";
                    var unit_currency = Result[i].GetValue("unit_currency").ToString().Trim();

                    PositionState state = new PositionState(currency.ToString(), balance.ToString(), locked.ToString(), avg_buy_price.ToString(), unit_currency.ToString());
                    Params.CoinPositionDict[currency] = state;

                    DisplayTargetCoins(currency, balance, avg_buy_price, cur_price, rate, tradingPnL);
                    write_sys_log("Displayed Coin position", 0);
                }

            }

        }


        /* WebSocket Thread for Orderbook */
        public void UpdateOrderbook()
        {
            var Markets = Params.upbit.GetMarkets();
            var MarketTickers = Params.upbit.GetMarketTicker(Markets).Item1;

            UpbitWebSocket WS_Orderbook = new UpbitWebSocket();

            var sendOrderbookMsg = WS_Orderbook.MakeSendMsg(MarketTickers, "orderbook");
            SendWebSocket(sendOrderbookMsg);
            
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




    }
}
