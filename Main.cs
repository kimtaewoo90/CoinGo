﻿using System;
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

            for (int i=0; i < 20; i++)
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
            if( res["type"].ToString() == "ticker")
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

                DisplayOrderbook(res);     
               
            }

        }

        public void DisplayUniverseMarket(JObject res)
        {
            var code = res["code"].ToString();
            var curPrice = double.Parse(res["trade_price"].ToString());
            var openingPrice = double.Parse(res["opening_price"].ToString());
            var change = Math.Round((curPrice / openingPrice) - 1, 2);
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
                                row.Cells["change"].Value = String.Format("{0:0,0}", change);
                                row.Cells["volume"].Value = String.Format("{0:0,0}", volume);
                                return;
                            }
                        }

                        UniverseDataGrid.Rows.Add(code, String.Format("{0:0,0}", curPrice), String.Format("{0:0,0}", change), String.Format("{0:0,0}", volume));
                    }

                    else UniverseDataGrid.Rows.Add(code, String.Format("{0:0,0}", curPrice), String.Format("{0:0,0}", change), String.Format("{0:0,0}", volume));

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
                            row.Cells["change"].Value = String.Format("{0:0,0}", change);
                            row.Cells["volume"].Value = String.Format("{0:0,0}", volume);
                            return;
                        }
                    }

                    UniverseDataGrid.Rows.Add(code, String.Format("{0:0,0}", curPrice), String.Format("{0:0,0}", change), String.Format("{0:0,0}", volume));
                }

                else UniverseDataGrid.Rows.Add(code, String.Format("{0:0,0}", curPrice), String.Format("{0:0,0}", change), String.Format("{0:0,0}", volume));
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

                        OrderbookDataGrid.Rows[0].Cells[1].Value = orderbook["orderbook_units"][9]["ask_price"];
                        OrderbookDataGrid.Rows[0].Cells[0].Value = orderbook["orderbook_units"][9]["ask_size"];

                        OrderbookDataGrid.Rows[1].Cells[1].Value = orderbook["orderbook_units"][8]["ask_price"];
                        OrderbookDataGrid.Rows[1].Cells[0].Value = orderbook["orderbook_units"][8]["ask_size"];

                        OrderbookDataGrid.Rows[2].Cells[1].Value = orderbook["orderbook_units"][7]["ask_price"];
                        OrderbookDataGrid.Rows[2].Cells[0].Value = orderbook["orderbook_units"][7]["ask_size"];

                        OrderbookDataGrid.Rows[3].Cells[1].Value = orderbook["orderbook_units"][6]["ask_price"];
                        OrderbookDataGrid.Rows[3].Cells[0].Value = orderbook["orderbook_units"][6]["ask_size"];

                        OrderbookDataGrid.Rows[4].Cells[1].Value = orderbook["orderbook_units"][5]["ask_price"];
                        OrderbookDataGrid.Rows[4].Cells[0].Value = orderbook["orderbook_units"][5]["ask_size"];

                        OrderbookDataGrid.Rows[5].Cells[1].Value = orderbook["orderbook_units"][4]["ask_price"];
                        OrderbookDataGrid.Rows[5].Cells[0].Value = orderbook["orderbook_units"][4]["ask_size"];

                        OrderbookDataGrid.Rows[6].Cells[1].Value = orderbook["orderbook_units"][3]["ask_price"];
                        OrderbookDataGrid.Rows[6].Cells[0].Value = orderbook["orderbook_units"][3]["ask_size"];

                        OrderbookDataGrid.Rows[7].Cells[1].Value = orderbook["orderbook_units"][2]["ask_price"];
                        OrderbookDataGrid.Rows[7].Cells[0].Value = orderbook["orderbook_units"][2]["ask_size"];

                        OrderbookDataGrid.Rows[8].Cells[1].Value = orderbook["orderbook_units"][1]["ask_price"];
                        OrderbookDataGrid.Rows[8].Cells[0].Value = orderbook["orderbook_units"][1]["ask_size"];

                        OrderbookDataGrid.Rows[9].Cells[1].Value = orderbook["orderbook_units"][0]["ask_price"];
                        OrderbookDataGrid.Rows[9].Cells[0].Value = orderbook["orderbook_units"][0]["ask_size"];

                        OrderbookDataGrid.Rows[10].Cells[1].Value = orderbook["orderbook_units"][0]["bid_price"];
                        OrderbookDataGrid.Rows[10].Cells[2].Value = orderbook["orderbook_units"][0]["bid_size"];

                        OrderbookDataGrid.Rows[11].Cells[1].Value = orderbook["orderbook_units"][1]["bid_price"];
                        OrderbookDataGrid.Rows[11].Cells[2].Value = orderbook["orderbook_units"][1]["bid_size"];

                        OrderbookDataGrid.Rows[12].Cells[1].Value = orderbook["orderbook_units"][2]["bid_price"];
                        OrderbookDataGrid.Rows[12].Cells[2].Value = orderbook["orderbook_units"][2]["bid_size"];

                        OrderbookDataGrid.Rows[13].Cells[1].Value = orderbook["orderbook_units"][3]["bid_price"];
                        OrderbookDataGrid.Rows[13].Cells[2].Value = orderbook["orderbook_units"][3]["bid_size"];

                        OrderbookDataGrid.Rows[14].Cells[1].Value = orderbook["orderbook_units"][4]["bid_price"];
                        OrderbookDataGrid.Rows[14].Cells[2].Value = orderbook["orderbook_units"][4]["bid_size"];

                        OrderbookDataGrid.Rows[15].Cells[1].Value = orderbook["orderbook_units"][5]["bid_price"];
                        OrderbookDataGrid.Rows[15].Cells[2].Value = orderbook["orderbook_units"][5]["bid_size"];

                        OrderbookDataGrid.Rows[16].Cells[1].Value = orderbook["orderbook_units"][6]["bid_price"];
                        OrderbookDataGrid.Rows[16].Cells[2].Value = orderbook["orderbook_units"][6]["bid_size"];

                        OrderbookDataGrid.Rows[17].Cells[1].Value = orderbook["orderbook_units"][7]["bid_price"];
                        OrderbookDataGrid.Rows[17].Cells[2].Value = orderbook["orderbook_units"][7]["bid_size"];

                        OrderbookDataGrid.Rows[18].Cells[1].Value = orderbook["orderbook_units"][8]["bid_price"];
                        OrderbookDataGrid.Rows[18].Cells[2].Value = orderbook["orderbook_units"][8]["bid_size"];

                        OrderbookDataGrid.Rows[19].Cells[1].Value = orderbook["orderbook_units"][9]["bid_price"];
                        OrderbookDataGrid.Rows[19].Cells[2].Value = orderbook["orderbook_units"][9]["bid_size"];

                    }));
                }
                else
                {
                    Orderbook_Code.Text = code;

                    OrderbookDataGrid.Rows[0].Cells[1].Value = orderbook["orderbook_units"][9]["ask_price"];
                    OrderbookDataGrid.Rows[0].Cells[0].Value = orderbook["orderbook_units"][9]["ask_size"];

                    OrderbookDataGrid.Rows[1].Cells[1].Value = orderbook["orderbook_units"][8]["ask_price"];
                    OrderbookDataGrid.Rows[1].Cells[0].Value = orderbook["orderbook_units"][8]["ask_size"];

                    OrderbookDataGrid.Rows[2].Cells[1].Value = orderbook["orderbook_units"][7]["ask_price"];
                    OrderbookDataGrid.Rows[2].Cells[0].Value = orderbook["orderbook_units"][7]["ask_size"];

                    OrderbookDataGrid.Rows[3].Cells[1].Value = orderbook["orderbook_units"][6]["ask_price"];
                    OrderbookDataGrid.Rows[3].Cells[0].Value = orderbook["orderbook_units"][6]["ask_size"];

                    OrderbookDataGrid.Rows[4].Cells[1].Value = orderbook["orderbook_units"][5]["ask_price"];
                    OrderbookDataGrid.Rows[4].Cells[0].Value = orderbook["orderbook_units"][5]["ask_size"];

                    OrderbookDataGrid.Rows[5].Cells[1].Value = orderbook["orderbook_units"][4]["ask_price"];
                    OrderbookDataGrid.Rows[5].Cells[0].Value = orderbook["orderbook_units"][4]["ask_size"];

                    OrderbookDataGrid.Rows[6].Cells[1].Value = orderbook["orderbook_units"][3]["ask_price"];
                    OrderbookDataGrid.Rows[6].Cells[0].Value = orderbook["orderbook_units"][3]["ask_size"];

                    OrderbookDataGrid.Rows[7].Cells[1].Value = orderbook["orderbook_units"][2]["ask_price"];
                    OrderbookDataGrid.Rows[7].Cells[0].Value = orderbook["orderbook_units"][2]["ask_size"];

                    OrderbookDataGrid.Rows[8].Cells[1].Value = orderbook["orderbook_units"][1]["ask_price"];
                    OrderbookDataGrid.Rows[8].Cells[0].Value = orderbook["orderbook_units"][1]["ask_size"];

                    OrderbookDataGrid.Rows[9].Cells[1].Value = orderbook["orderbook_units"][0]["ask_price"];
                    OrderbookDataGrid.Rows[9].Cells[0].Value = orderbook["orderbook_units"][0]["ask_size"];

                    OrderbookDataGrid.Rows[10].Cells[1].Value = orderbook["orderbook_units"][0]["bid_price"];
                    OrderbookDataGrid.Rows[10].Cells[2].Value = orderbook["orderbook_units"][0]["bid_size"];

                    OrderbookDataGrid.Rows[11].Cells[1].Value = orderbook["orderbook_units"][1]["bid_price"];
                    OrderbookDataGrid.Rows[11].Cells[2].Value = orderbook["orderbook_units"][1]["bid_size"];

                    OrderbookDataGrid.Rows[12].Cells[1].Value = orderbook["orderbook_units"][2]["bid_price"];
                    OrderbookDataGrid.Rows[12].Cells[2].Value = orderbook["orderbook_units"][2]["bid_size"];

                    OrderbookDataGrid.Rows[13].Cells[1].Value = orderbook["orderbook_units"][3]["bid_price"];
                    OrderbookDataGrid.Rows[13].Cells[2].Value = orderbook["orderbook_units"][3]["bid_size"];

                    OrderbookDataGrid.Rows[14].Cells[1].Value = orderbook["orderbook_units"][4]["bid_price"];
                    OrderbookDataGrid.Rows[14].Cells[2].Value = orderbook["orderbook_units"][4]["bid_size"];

                    OrderbookDataGrid.Rows[15].Cells[1].Value = orderbook["orderbook_units"][5]["bid_price"];
                    OrderbookDataGrid.Rows[15].Cells[2].Value = orderbook["orderbook_units"][5]["bid_size"];

                    OrderbookDataGrid.Rows[16].Cells[1].Value = orderbook["orderbook_units"][6]["bid_price"];
                    OrderbookDataGrid.Rows[16].Cells[2].Value = orderbook["orderbook_units"][6]["bid_size"];

                    OrderbookDataGrid.Rows[17].Cells[1].Value = orderbook["orderbook_units"][7]["bid_price"];
                    OrderbookDataGrid.Rows[17].Cells[2].Value = orderbook["orderbook_units"][7]["bid_size"];

                    OrderbookDataGrid.Rows[18].Cells[1].Value = orderbook["orderbook_units"][8]["bid_price"];
                    OrderbookDataGrid.Rows[18].Cells[2].Value = orderbook["orderbook_units"][8]["bid_size"];

                    OrderbookDataGrid.Rows[19].Cells[1].Value = orderbook["orderbook_units"][9]["bid_price"];
                    OrderbookDataGrid.Rows[19].Cells[2].Value = orderbook["orderbook_units"][9]["bid_size"];

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
                                    row.Cells["code"].Value = currency;
                                    row.Cells["Quantity"].Value = balance;
                                    row.Cells["buyPrice"].Value = buy_price;
                                    row.Cells["curPrice_position"].Value = cur_price;
                                    row.Cells["rate"].Value = rate;
                                    row.Cells["tradingPnL"].Value = pnl;
                                    return;
                                }
                            }

                            positionDataGrid.Rows.Add(currency, balance, buy_price, cur_price, rate, pnl);
                        }

                        else positionDataGrid.Rows.Add(currency, balance, buy_price, cur_price, rate, pnl);

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
                            row.Cells["code"].Value = currency;
                            row.Cells["Quantity"].Value = balance;
                            row.Cells["buyPrice"].Value = buy_price;
                            row.Cells["curPrice_position"].Value = cur_price;
                            row.Cells["rate"].Value = rate;
                            row.Cells["tradingPnL"].Value = pnl;
                            return;
                        }
                    }

                    positionDataGrid.Rows.Add(currency, balance, buy_price, cur_price, rate, pnl);
                }

                else positionDataGrid.Rows.Add(currency, balance, buy_price, cur_price, rate, pnl);
            }
        }

        // LogThread
        public void UpdateLog()
        {

        }

        // PositionThread
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


                    var rate = $"{Math.Round(((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100, 2)} %";
                    var tradingPnL = $"{Math.Round((double.Parse(cur_price) - double.Parse(avg_buy_price)) * double.Parse(balance), 2)} 원";
                    var unit_currency = Result[i].GetValue("unit_currency").ToString().Trim();

                    PositionState state = new PositionState(currency.ToString(), balance.ToString(), locked.ToString(), avg_buy_price.ToString(), unit_currency.ToString());
                    Params.CoinPositionDict[currency] = state;

                    DisplayTargetCoins(currency, balance, avg_buy_price, cur_price, rate, tradingPnL);
                    //write_sys_log("Displayed Coin position", 0);
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
        }

        public void GetCandleData(string ticker)
        {
            var CandleData = "";
            CandleState candle = new CandleState("");
            if(Is_get_200_candle_data[ticker] is true)
            {
                // Create Dictionary
                CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._3, count:200);
                util.delay(200);
                
                candle = new CandleState(CandleData);
                Params.CandleDict[ticker] = candle;

                Params.CompareCandleTime[ticker] = Params.CandleDict[ticker].date_time[Params.CandleDict[ticker].date_time.Count-1];

                Params.DifferencePrice[ticker] = new List<double>();
                Params.UpperSide[ticker] = new List<double>();
                Params.DownSide[ticker] = new List<double>();
                Params.RSI_List[ticker] = new List<double>();

                // RSi 계산
                CalculateRSI(candle, ticker, isFirst:true);

                Is_get_200_candle_data[ticker] = false;
            }
            else if(Is_get_200_candle_data[ticker] is false)
            {
                CandleData = Params.upbit.GetCandles_Minute(ticker, UpbitAPI.UpbitMinuteCandleType._3, count:1);
                util.delay(200);

                var count = Params.CandleDict[ticker].code.Count;

                // 캔들 같은 봉
                if (Params.CompareCandleTime[ticker] == CandleData.Split(new char[] { ',' })[2].Substring(24, 19))
                {
                    // New High price > Old High price
                    if (double.Parse(CandleData.Split(new char[] { ',' })[4].Split(new char[] { ':' })[1]) >
                        double.Parse(Params.CandleDict[ticker].high_price[count - 1]))
                        Params.CandleDict[ticker].high_price[count-1] = (CandleData.Split(new char[] { ',' })[4].Split(new char[] { ':' })[1]);
                    // New High price < Old High price
                    else Params.CandleDict[ticker].high_price[count-1] = (Params.CandleDict[ticker].high_price[count - 1]);

                    // New Low price < Old Low price
                    if (double.Parse(CandleData.Split(new char[] { ',' })[5].Split(new char[] { ':' })[1]) <
                        double.Parse(Params.CandleDict[ticker].low_price[count - 1]))
                        Params.CandleDict[ticker].low_price[count-1] = (CandleData.Split(new char[] { ',' })[5].Split(new char[] { ':' })[1]);
                    // New Low price > Old Low price
                    else Params.CandleDict[ticker].low_price[count-1] = (Params.CandleDict[ticker].low_price[count - 1]);

                    // Trade price
                    Params.CandleDict[ticker].trade_price[count - 1] = CandleData.Split(new char[] { ',' })[6].Split(new char[] { ':' })[1];
                    Params.CandleDict[ticker].total_trading_price[count-1] = (CandleData.Split(new char[] { ',' })[8].Split(new char[] { ':' })[1]);
                    Params.CandleDict[ticker].total_trading_volume[count-1] = (CandleData.Split(new char[] { ',' })[9].Split(new char[] { ':' })[1]);
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
                }
            

                // Real Time Candle Chart
                if(ticker == Orderbook_ShortCode)
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

        public void CalculateRSI(CandleState candle, string ticker, bool isFirst)
        {
            // RSI 계산
            if (isFirst is true)
            {
                for (int i = 1; i < candle.trade_price.Count; i++)
                {
                    Params.DifferencePrice[ticker].Add((double.Parse(candle.trade_price[i]) - double.Parse(candle.trade_price[i - 1])));
                }

                Params.UpperSide[ticker] = Params.DifferencePrice[ticker];
                Params.DownSide[ticker] = Params.DifferencePrice[ticker];

                //Params.UpperSide[ticker].Where(x => x < 0).All(x => x=0) ;
                Params.UpperSide[ticker].ForEach(x => { if (x < 0) { x = 0; } });
                Params.DownSide[ticker].ForEach(x => { if (x > 0) { x = 0; } });

                var au = Params.UpperSide[ticker].Average();
                var ad = Params.DownSide[ticker].Average();

                var rs = au / ad;
                var rsi = rs / (1 + rs) * 100;
                Params.RSI_List[ticker].Add(rsi);

                write_sys_log($"{ticker} : {rsi.ToString()}", 0);
            }
            else if(isFirst is false)
            {

            } 
        }

        public void ChartAxisChanged(object sender, ViewEventArgs e)
        {
            if (sender.Equals(Candle_Chart))
            {
                int startPosition = (int)e.Axis.ScaleView.ViewMinimum;
                int endPosition = (int)e.Axis.ScaleView.ViewMaximum;
                double min = (double)e.ChartArea.AxisY.ScaleView.ViewMinimum;
                double max = (double)e.ChartArea.AxisY.ScaleView.ViewMaximum;

                for(int i = startPosition - 1; i < endPosition - 1; i++)
                {
                    if (i > 200) break;
                    if (i < 0) i = 0;

                    if(double.Parse(Params.CandleDict[Orderbook_ShortCode].high_price[i]) > max)
                    {
                        max = double.Parse(Params.CandleDict[Orderbook_ShortCode].high_price[i]) + 2;
                    }
                    if(double.Parse(Params.CandleDict[Orderbook_ShortCode].low_price[i]) < min)
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

            for (int i=0; i < Params.CandleDict[Orderbook_ShortCode].code.Count; i++)
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
                CreateCandleChart();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
    }
}
