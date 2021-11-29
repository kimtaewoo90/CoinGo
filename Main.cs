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
using System.Windows.Forms.DataVisualization.Charting;

namespace CoinGo
{
    public partial class Main : Form
    {

        Thread PositionThread = null;
        Thread OrderbookThread = null;
        Thread LogThread = null;

        Utils util = new Utils();
        delegate void Ctr_Involk(Control ctr, string text);

        public string Orderbook_ShortCode { get; set; } = "KRW-BTC";

        public Dictionary<string, bool> Is_get_200_candle_data = new Dictionary<string, bool>();

        public Main()
        {
            InitializeComponent();

            Series series = Candle_Chart.Series.Add("S_candle1");
            series["PriceUpColor"] = "Red";
            series["PriceDownColor"] = "Blue";

            series.ChartType = SeriesChartType.Candlestick;
            Candle_Chart.AxisViewChanged += ChartAxisChanged;
            Candle_Chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            Candle_Chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            Candle_Chart.ChartAreas[0].CursorX.AutoScroll = true;
            Candle_Chart.ChartAreas[0].CursorY.AutoScroll = true;
            Candle_Chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            Candle_Chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            Candle_Chart.ChartAreas[0].AxisX.IsReversed = true;

            Candle_Chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            Candle_Chart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;

            CoinStart();
        }

        public void CoinStart()
        {

            for (int i = 0; i < 20; i++)
            {
                OrderbookDataGrid.Rows.Add("", "", "");
            }

            UpbitWebSocket WS = new UpbitWebSocket();

            var Markets = Params.upbit.GetMarkets();
            var MarketTickers = Params.upbit.GetMarketTicker(Markets).Item1;
            var MarketTickersKrName = Params.upbit.GetMarketTicker(Markets).Item2;

            for (int i = 0; i < MarketTickers.Count; i++)
            {
                Is_get_200_candle_data[MarketTickers[i]] = true;
            }

            // Make SendMsg for WebSocket
            var sendMsg = WS.MakeSendMsg(MarketTickers, "ticker");
            SendWebSocket(sendMsg);

            PositionThread = new Thread(new ThreadStart(UpdatePosition));
            PositionThread.Start();

            OrderbookThread = new Thread(new ThreadStart(UpdateOrderbook));
            OrderbookThread.Start();

            LogThread = new Thread(new ThreadStart(UpdateLog));
            LogThread.Start();
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
            JObject res = JObject.Parse(requestMsg);

            // Market Rtd data.
            if (res["type"].ToString() == "ticker")
            {
                var code = res["code"].ToString();

                CoinState state = new CoinState(res);
                Params.CoinInfoDict[code] = state;
                DisplayUniverseMarket(res);

                // Candle data 수집
                GetCandleData(code);

                //write_sys_log(Params.CandleDict[code].date_time.ToString(), 0);
                UpdateSignals(res);
            }

            // Orderbook of each coins.
            else if (res["type"].ToString() == "orderbook")
            {
                var code = res["code"].ToString();

                OrderbookState state = new OrderbookState(res);
                Params.CoinOrderbookDict[code] = state;

                if (Orderbook_check.Checked)
                    DisplayOrderbook(res);

            }

            else if (res["type"].ToString() == "trade")
            {
                // Blotter 업데이트
                // Position 업데이트
            }

        }

        public void DisplayUniverseMarket(JObject res)
        {
            var code = res["code"].ToString();
            var curPrice = double.Parse(res["trade_price"].ToString());
            var openingPrice = double.Parse(res["opening_price"].ToString());
            var change = ((curPrice / openingPrice) - 1) * 100;
            var volume = Math.Round(double.Parse(res["acc_trade_volume"].ToString()), 2);

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
                                row.Cells["curPrice"].Value = String.Format("{0:0,0}", curPrice);
                                row.Cells["change"].Value = String.Format("{0:0.#}", change);
                                row.Cells["volume"].Value = String.Format("{0:0,0}", volume);
                                return;
                            }
                        }

                        UniverseDataGrid.Rows.Add(code, String.Format("{0:0,0}", curPrice), String.Format("{0:0.#}", change), String.Format("{0:0,0}", volume));
                    }

                    else UniverseDataGrid.Rows.Add(code, String.Format("{0:0,0}", curPrice), String.Format("{0:0.#}", change), String.Format("{0:0,0}", volume));

                }));
            }

            else
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
                            row.Cells["curPrice"].Value = String.Format("{0:0,0}", curPrice);
                            row.Cells["change"].Value = String.Format("{0:0.#}", change);
                            row.Cells["volume"].Value = String.Format("{0:0,0}", volume);
                            return;
                        }
                    }

                    UniverseDataGrid.Rows.Add(code, String.Format("{0:0,0}", curPrice), String.Format("{0:0.#}", change), String.Format("{0:0,0}", volume));
                }

                else UniverseDataGrid.Rows.Add(code, String.Format("{0:0,0}", curPrice), String.Format("{0:0.#}", change), String.Format("{0:0,0}", volume));
            }
        }

        public void DisplayOrderbook(JObject orderbook)
        {
            var code = orderbook["code"].ToString();

            if (code == Orderbook_ShortCode)
            {
                if (OrderbookDataGrid.InvokeRequired)
                {
                    OrderbookDataGrid.Invoke(new MethodInvoker(delegate ()
                    {
                        Orderbook_Code.Text = code;

                        OrderbookDataGrid.Rows[0].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["ask_price"]) ;
                        OrderbookDataGrid.Rows[0].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["ask_size"]);

                        OrderbookDataGrid.Rows[1].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["ask_price"]);
                        OrderbookDataGrid.Rows[1].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["ask_size"]);

                        OrderbookDataGrid.Rows[2].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["ask_price"]);
                        OrderbookDataGrid.Rows[2].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["ask_size"]);

                        OrderbookDataGrid.Rows[3].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["ask_price"]);
                        OrderbookDataGrid.Rows[3].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["ask_size"]);

                        OrderbookDataGrid.Rows[4].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["ask_price"]);
                        OrderbookDataGrid.Rows[4].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["ask_size"]);

                        OrderbookDataGrid.Rows[5].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["ask_price"]);
                        OrderbookDataGrid.Rows[5].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["ask_size"]);

                        OrderbookDataGrid.Rows[6].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["ask_price"]);
                        OrderbookDataGrid.Rows[6].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["ask_size"]);

                        OrderbookDataGrid.Rows[7].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["ask_price"]);
                        OrderbookDataGrid.Rows[7].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["ask_size"]);

                        OrderbookDataGrid.Rows[8].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["ask_price"]);
                        OrderbookDataGrid.Rows[8].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["ask_size"]);

                        OrderbookDataGrid.Rows[9].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["ask_price"]);
                        OrderbookDataGrid.Rows[9].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["ask_size"]);

                        OrderbookDataGrid.Rows[10].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["bid_price"]);
                        OrderbookDataGrid.Rows[10].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["bid_size"]);

                        OrderbookDataGrid.Rows[11].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["bid_price"]);
                        OrderbookDataGrid.Rows[11].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["bid_size"]);

                        OrderbookDataGrid.Rows[12].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["bid_price"]);
                        OrderbookDataGrid.Rows[12].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["bid_size"]);

                        OrderbookDataGrid.Rows[13].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["bid_price"]);
                        OrderbookDataGrid.Rows[13].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["bid_size"]);

                        OrderbookDataGrid.Rows[14].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["bid_price"]);
                        OrderbookDataGrid.Rows[14].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["bid_size"]);

                        OrderbookDataGrid.Rows[15].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["bid_price"]);
                        OrderbookDataGrid.Rows[15].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["bid_size"]);

                        OrderbookDataGrid.Rows[16].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["bid_price"]);
                        OrderbookDataGrid.Rows[16].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["bid_size"]);

                        OrderbookDataGrid.Rows[17].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["bid_price"]);
                        OrderbookDataGrid.Rows[17].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["bid_size"]);

                        OrderbookDataGrid.Rows[18].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["bid_price"]);
                        OrderbookDataGrid.Rows[18].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["bid_size"]);

                        OrderbookDataGrid.Rows[19].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["bid_price"]);
                        OrderbookDataGrid.Rows[19].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["bid_size"]);

                    }));
                }
                else
                {
                    Orderbook_Code.Text = code;

                    OrderbookDataGrid.Rows[0].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["ask_price"]);
                    OrderbookDataGrid.Rows[0].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["ask_size"]);

                    OrderbookDataGrid.Rows[1].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["ask_price"]);
                    OrderbookDataGrid.Rows[1].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["ask_size"]);

                    OrderbookDataGrid.Rows[2].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["ask_price"]);
                    OrderbookDataGrid.Rows[2].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["ask_size"]);

                    OrderbookDataGrid.Rows[3].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["ask_price"]);
                    OrderbookDataGrid.Rows[3].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["ask_size"]);

                    OrderbookDataGrid.Rows[4].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["ask_price"]);
                    OrderbookDataGrid.Rows[4].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["ask_size"]);

                    OrderbookDataGrid.Rows[5].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["ask_price"]);
                    OrderbookDataGrid.Rows[5].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["ask_size"]);

                    OrderbookDataGrid.Rows[6].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["ask_price"]);
                    OrderbookDataGrid.Rows[6].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["ask_size"]);

                    OrderbookDataGrid.Rows[7].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["ask_price"]);
                    OrderbookDataGrid.Rows[7].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["ask_size"]);

                    OrderbookDataGrid.Rows[8].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["ask_price"]);
                    OrderbookDataGrid.Rows[8].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["ask_size"]);

                    OrderbookDataGrid.Rows[9].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["ask_price"]);
                    OrderbookDataGrid.Rows[9].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["ask_size"]);

                    OrderbookDataGrid.Rows[10].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["bid_price"]);
                    OrderbookDataGrid.Rows[10].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["bid_size"]);

                    OrderbookDataGrid.Rows[11].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["bid_price"]);
                    OrderbookDataGrid.Rows[11].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["bid_size"]);

                    OrderbookDataGrid.Rows[12].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["bid_price"]);
                    OrderbookDataGrid.Rows[12].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["bid_size"]);

                    OrderbookDataGrid.Rows[13].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["bid_price"]);
                    OrderbookDataGrid.Rows[13].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["bid_size"]);

                    OrderbookDataGrid.Rows[14].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["bid_price"]);
                    OrderbookDataGrid.Rows[14].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["bid_size"]);

                    OrderbookDataGrid.Rows[15].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["bid_price"]);
                    OrderbookDataGrid.Rows[15].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["bid_size"]);

                    OrderbookDataGrid.Rows[16].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["bid_price"]);
                    OrderbookDataGrid.Rows[16].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["bid_size"]);

                    OrderbookDataGrid.Rows[17].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["bid_price"]);
                    OrderbookDataGrid.Rows[17].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["bid_size"]);

                    OrderbookDataGrid.Rows[18].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["bid_price"]);
                    OrderbookDataGrid.Rows[18].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["bid_size"]);

                    OrderbookDataGrid.Rows[19].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["bid_price"]);
                    OrderbookDataGrid.Rows[19].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["bid_size"]);

                }
            }
        }

        public void DisplayTargetCoins(string currency, string balance, string buy_price, string cur_price, string rate, string pnl)
        {


            if (positionDataGrid.InvokeRequired)
            {
                positionDataGrid.Invoke(new MethodInvoker(delegate ()
                {
                    positionDataGrid.Invoke(new MethodInvoker(delegate ()
                    {
                        if (positionDataGrid.Rows.Count > 1)
                        {
                            foreach (DataGridViewRow row in positionDataGrid.Rows)
                            {
                                if (row.Cells["code"].Value == null)
                                    break;


                                if (row.Cells["code"].Value.ToString() == currency || row.Cells["code"].Value == null)
                                {
                                    row.Cells["code"].Value = String.Format("{0:0,0}", currency);
                                    row.Cells["Quantity"].Value = String.Format("{0:0,0}", balance);
                                    row.Cells["buyPrice"].Value = String.Format("{0:0,0}", buy_price);
                                    row.Cells["curPrice_position"].Value = String.Format("{0:0,0}", cur_price);
                                    row.Cells["rate"].Value = rate;
                                    row.Cells["tradingPnL"].Value = String.Format("{0:0,0}", pnl);
                                    return;
                                }
                            }

                            positionDataGrid.Rows.Add(String.Format("{0:0,0}", currency), 
                                                      String.Format("{0:0,0}", balance),
                                                      String.Format("{0:0,0}", buy_price),
                                                      String.Format("{0:0,0}", cur_price), 
                                                      rate,
                                                      String.Format("{0:0,0}", pnl));
                        }

                        else positionDataGrid.Rows.Add(String.Format("{0:0,0}", currency),
                                                        String.Format("{0:0,0}", balance),
                                                        String.Format("{0:0,0}", buy_price),
                                                        String.Format("{0:0,0}", cur_price), 
                                                        rate,
                                                        String.Format("{0:0,0}", pnl));

                    }));
                }));
            }

            else
            {
                if (positionDataGrid.Rows.Count > 1)
                {
                    foreach (DataGridViewRow row in positionDataGrid.Rows)
                    {
                        if (row.Cells["code"].Value == null)
                            break;


                        if (row.Cells["code"].Value.ToString() == currency || row.Cells["code"].Value == null)
                        {
                            row.Cells["code"].Value = String.Format("{0:0,0}", currency);
                            row.Cells["Quantity"].Value = String.Format("{0:0,0}", balance);
                            row.Cells["buyPrice"].Value = String.Format("{0:0,0}", buy_price);
                            row.Cells["curPrice_position"].Value = String.Format("{0:0,0}", cur_price);
                            row.Cells["rate"].Value = rate;
                            row.Cells["tradingPnL"].Value = String.Format("{0:0,0}", pnl);
                            return;
                        }
                    }

                    positionDataGrid.Rows.Add(String.Format("{0:0,0}", currency),
                                              String.Format("{0:0,0}", balance),
                                              String.Format("{0:0,0}", buy_price),
                                              String.Format("{0:0,0}", cur_price),
                                              rate,
                                              String.Format("{0:0,0}", pnl));
                }

                else positionDataGrid.Rows.Add(String.Format("{0:0,0}", currency),
                                                String.Format("{0:0,0}", balance),
                                                String.Format("{0:0,0}", buy_price),
                                                String.Format("{0:0,0}", cur_price),
                                                rate,
                                                String.Format("{0:0,0}", pnl));
            }
        }

        // LogThread
        public void UpdateLog()
        {

        }

        // PositionThread
        public void UpdatePosition()
        {
            while (true)
            {
                List<JObject> Result = Params.upbit.GetAccount();
                util.delay(200);

                // Initialize Position
                Params.TotalAsset = 0.0;
                Params.CoinAsset = 0.0;
                Params.CashAsset = 0.0;
                Params.PnL = 0.0;
                Params.PnLChange = 0.0;
                var cash = 0.0;

                // Display Position
                for (int i = 0; i < Result.Count; i++)
                {
                    var currency = Result[i].GetValue("currency").ToString().Trim();
                    var balance = Result[i].GetValue("balance").ToString().Trim();
                    var locked = Result[i].GetValue("locked").ToString().Trim();
                    var avg_buy_price = Result[i].GetValue("avg_buy_price").ToString().Trim();
                    var cur_price = "0.0";

                    if (currency == "KRW") cash = double.Parse(balance);

                    //string[] codes = currency.Split('-');
                    string code = $"KRW-{currency}";
                    if (Params.CoinInfoDict.Count != 0)
                    {
                        if (Params.CoinInfoDict.ContainsKey(code))
                            cur_price = Params.CoinInfoDict[code].curPrice;
                    }


                    var rate = $"{Math.Round(((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100, 2)} %";
                    var tradingPnL = $"{Math.Round((double.Parse(cur_price) - double.Parse(avg_buy_price)) * double.Parse(balance), 2)} 원";
                    var unit_currency = Result[i].GetValue("unit_currency").ToString().Trim();

                    PositionState state = new PositionState(currency.ToString(), balance.ToString(), locked.ToString(), avg_buy_price.ToString(), unit_currency.ToString());
                    Params.CoinPositionDict[currency] = state;

                    Params.TotalAsset = Params.TotalAsset + double.Parse(balance) * double.Parse(cur_price);
                    Params.CoinAsset = Params.CoinAsset + double.Parse(balance) * double.Parse(cur_price);
                    Params.PnL = Params.PnL + (double.Parse(cur_price) - double.Parse(avg_buy_price)) * double.Parse(balance);

                    DisplayTargetCoins(currency, balance, avg_buy_price, cur_price, rate, tradingPnL);
                }

                Params.TotalAsset = Params.TotalAsset + cash;
                Params.CashAsset = cash;
                
                // Display Total Position display
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        Total_Asset.Text = String.Format("{0:0,0}", Math.Round(Params.TotalAsset));
                        Cash_Asset.Text = String.Format("{0:0,0}", Params.CashAsset);
                        Coin_Asset.Text = String.Format("{0:0,0}", Params.CoinAsset);
                        PnL.Text = String.Format("{0:0,0}", Params.PnL);
                        PnL_Change.Text = Params.PnLChange.ToString();
                    }));
                }
                else
                {
                    Total_Asset.Text = String.Format("{0:0,0}", Params.TotalAsset);
                    Cash_Asset.Text = String.Format("{0:0,0}", Params.CashAsset);
                    Coin_Asset.Text = String.Format("{0:0,0}", Params.CoinAsset);
                    PnL.Text = String.Format("{0:0,0}", Params.PnL);
                    PnL_Change.Text = Params.PnLChange.ToString();
                }

            }

        }


        // OrderbookThread
        public void UpdateOrderbook()
        {
            var Markets = Params.upbit.GetMarkets();
            var MarketTickers = Params.upbit.GetMarketTicker(Markets).Item1;

            UpbitWebSocket WS_Orderbook = new UpbitWebSocket();

            var sendOrderbookMsg = WS_Orderbook.MakeSendMsg(MarketTickers, "orderbook");
            SendWebSocket(sendOrderbookMsg);

        }

        // SignalThread
        public void UpdateSignals(JObject res)
        {
            var data = res;
            var ticker = data["code"].ToString();
            // Strategy1
            Strategy1_RSI_BB strategy1 = new Strategy1_RSI_BB(ticker);
            var signal = strategy1.Is_Signal();

            if(signal == true)
            {
                // 매수
                write_sys_log($"{ticker} signal is true", 0);
            }
        }

        public void GetCandleData(string ticker)
        {
            var CandleData = "";
            CandleState candle = new CandleState("");
            if (Is_get_200_candle_data[ticker] is true)
            {
                // Create Dictionary
                CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._3, count: 200);
                util.delay(200);

                candle = new CandleState(CandleData);
                Params.CandleDict[ticker] = candle;

                Params.CompareCandleTime[ticker] = Params.CandleDict[ticker].date_time[Params.CandleDict[ticker].date_time.Count - 1];

                Params.DifferencePrice[ticker] = new List<double>();
                Params.UpperSide[ticker] = new List<double>();
                Params.DownSide[ticker] = new List<double>();
                Params.RSI_List[ticker] = new List<double>();

                // RSi 계산
                Params.RSI_List[ticker].Add(CalculateRSI(candle, ticker, isFirst: true, IsSame:true));

                Is_get_200_candle_data[ticker] = false;
            }
            else if (Is_get_200_candle_data[ticker] is false)
            {
                try
                {
                    CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._3, count: 1);
                    util.delay(200);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
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
                    Params.RSI_List[ticker][Params.RSI_List[ticker].Count - 1] = CalculateRSI(Params.CandleDict[ticker], ticker, isFirst:false, IsSame:true);

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

                    Params.RSI_List[ticker].Add(CalculateRSI(Params.CandleDict[ticker], ticker, isFirst:false, IsSame:false));

                    //if (ticker == "KRW-BTC") write_sys_log($"other candle : {Params.RSI_List[ticker][Params.RSI_List[ticker].Count -1]}", 0);

                }


                // Real Time Candle Chart
                if (ticker == Orderbook_ShortCode)
                {
                    //CreateCandleChart();
                }
                // New Candle
                //if (double.Parse(CandleData.Split(new char[] { ',' })[2].Split(new char[] { ':' })[1]) > 
                //    double.Parse(Params.CandleDict[ticker].date_time[Params.CandleDict[ticker].date_time.Count - 1]))
                // {

                // }
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

                if(Params.DifferencePrice[ticker][Params.DifferencePrice[ticker].Count - 1] > 0)
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

        public void ChartAxisChanged(object sender, ViewEventArgs e)
        {
            if (sender.Equals(Candle_Chart))
            {
                int startPosition = (int)e.Axis.ScaleView.ViewMinimum;
                int endPosition = (int)e.Axis.ScaleView.ViewMaximum;
                double min = (double)e.ChartArea.AxisY.ScaleView.ViewMinimum;
                double max = (double)e.ChartArea.AxisY.ScaleView.ViewMaximum;

                for (int i = startPosition - 1; i < endPosition - 1; i++)
                {
                    if (i > 200) break;
                    if (i < 0) i = 0;

                    if (double.Parse(Params.CandleDict[Orderbook_ShortCode].high_price[i]) > max)
                    {
                        max = double.Parse(Params.CandleDict[Orderbook_ShortCode].high_price[i]) + 2;
                    }
                    if (double.Parse(Params.CandleDict[Orderbook_ShortCode].low_price[i]) < min)
                    {
                        min = double.Parse(Params.CandleDict[Orderbook_ShortCode].low_price[i]) - 2;
                    }
                }

                Candle_Chart.ChartAreas[0].AxisY.Maximum = max;
                Candle_Chart.ChartAreas[0].AxisY.Minimum = min;

            }
        }

        public void CreateCandleChart()
        {

            Candle_Chart.Series["S_candle1"].Points.Clear();

            Candle_Chart.ChartAreas[0].AxisY.Maximum = Params.CandleDict[Orderbook_ShortCode].high_price.Select(f => double.Parse(f)).ToList().Max();
            Candle_Chart.ChartAreas[0].AxisY.Minimum = Params.CandleDict[Orderbook_ShortCode].low_price.Select(f => double.Parse(f)).ToList().Min();

            for (int i = 0; i < Params.CandleDict[Orderbook_ShortCode].code.Count; i++)
            {
                Candle_Chart.Series["S_candle1"].Points.AddXY(Params.CandleDict[Orderbook_ShortCode].date_time[i],
                                                             double.Parse(Params.CandleDict[Orderbook_ShortCode].high_price[i]));
                Candle_Chart.Series["S_candle1"].Points[i].YValues[1] = double.Parse(Params.CandleDict[Orderbook_ShortCode].low_price[i]);
                Candle_Chart.Series["S_candle1"].Points[i].YValues[2] = double.Parse(Params.CandleDict[Orderbook_ShortCode].opening_price[i]);
                Candle_Chart.Series["S_candle1"].Points[i].YValues[3] = double.Parse(Params.CandleDict[Orderbook_ShortCode].trade_price[i]);

            }
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

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void UniverseDataGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Orderbook_ShortCode = UniverseDataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();

                if(Chart_check.Checked)
                {
                    CreateCandleChart();
                }
                else
                {
                    MessageBox.Show("차트보기가 체크되어있지 않습니다");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void UniverseDataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Orderbook_ShortCode = UniverseDataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
            
        }
    }
}
